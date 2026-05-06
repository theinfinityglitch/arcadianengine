using System.Numerics;
using System.Reflection;
using System.Text.RegularExpressions;

using Friflo.Engine.ECS;
using ImGuiNET;
using IconFonts;
using Raylib_cs;

using ArcadianEngine.Utils;

namespace ArcadianEngine.Resources;

public partial class WorldHierarchyDebug<G>(GameContext<G> cx) where G : class, IArcadianGame<G>
{
    private readonly EntityStore _world = cx.Game.world;
    private readonly Dictionary<(int, Type), bool> _openInspectors = [];
    private readonly Dictionary<Type, Action<Entity, object>> _setterCache = [];
    private int? _selectedEntityId = null;
    private Type? _selectedResourceType = null;

    public void Draw()
    {
        // Hierarchy window
        if (ImGui.Begin($"{Lucide.Earth} World Hierarchy"))
        {
            ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Vector2(ImGui.GetStyle().FramePadding.X, 4.0f * Raylib.GetWindowScaleDPI().X));
            var main_flags = ImGuiTreeNodeFlags.SpanFullWidth | ImGuiTreeNodeFlags.FramePadding;

            // Resources node
            var resources = cx.GetAllResources();
            if (ImGui.TreeNodeEx($"{Lucide.Package} Resources", main_flags))
            {
                foreach (var (type, _) in resources)
                {
                    var item_flags = ImGuiTreeNodeFlags.Leaf | ImGuiTreeNodeFlags.NoTreePushOnOpen | ImGuiTreeNodeFlags.FramePadding | ImGuiTreeNodeFlags.SpanFullWidth;
                    var isSelected = _selectedResourceType == type;
                    var name = type.GetFriendlyName();

                    if (isSelected) item_flags |= ImGuiTreeNodeFlags.Selected;

                    ImGui.TreeNodeEx($"{Lucide.Database} {name}", item_flags);

                    if (ImGui.IsItemClicked(ImGuiMouseButton.Left))
                    {
                        _selectedResourceType = type;
                        _selectedEntityId = null;
                    }
                }
                ImGui.TreePop();
            }

            // Entities node
            if (ImGui.TreeNodeEx($"{Lucide.Boxes} Entities", main_flags))
            {
                foreach (var entity in _world.Entities)
                {
                    if (!entity.IsNull && entity.Parent.IsNull)
                        DrawEntity(entity);
                }
                ImGui.TreePop();
            }

            ImGui.PopStyleVar();
        }
        ImGui.End();

        DrawEntityInspector();

        foreach (var key in _openInspectors.Keys.ToList())
        {
            var (entityId, type) = key;
            bool open = _openInspectors[key];

            if (!open) continue;

            // Find the entity
            var entity = _world.Entities.FirstOrDefault(e => e.Id == entityId);
            if (entity.IsNull)
            {
                _openInspectors.Remove(key);
                continue;
            }

            var title = $"{Lucide.Settings} {type.Name} (Entity {entityId})";
            ImGui.SetNextWindowSize(new Vector2(300, 200), ImGuiCond.FirstUseEver);

            if (ImGui.Begin(title, ref open))
                DrawComponentInspector(entity, type);

            ImGui.End();

            _openInspectors[key] = open; // sync close button
        }
    }

    private void DrawEntity(Entity entity)
    {
        var hasChildren = entity.ChildEntities.Count > 0;
        var hasAny = hasChildren || entity.Components.Count > 0 || entity.Tags.Count > 0;
        var isSelected = _selectedEntityId == entity.Id;

        var entity_flags = ImGuiTreeNodeFlags.OpenOnArrow | ImGuiTreeNodeFlags.OpenOnDoubleClick | ImGuiTreeNodeFlags.SpanFullWidth | ImGuiTreeNodeFlags.FramePadding;
        var group_flags = ImGuiTreeNodeFlags.SpanFullWidth | ImGuiTreeNodeFlags.FramePadding;
        var item_flags = ImGuiTreeNodeFlags.Leaf | ImGuiTreeNodeFlags.NoTreePushOnOpen | ImGuiTreeNodeFlags.FramePadding | ImGuiTreeNodeFlags.SpanFullWidth;

        if (!hasAny) entity_flags |= ImGuiTreeNodeFlags.Leaf | ImGuiTreeNodeFlags.NoTreePushOnOpen;
        if (isSelected) entity_flags |= ImGuiTreeNodeFlags.Selected;

        bool opened = ImGui.TreeNodeEx($"{Lucide.Box} Entity {entity.Id}", entity_flags);

        // Single click — select entity
        if (ImGui.IsItemClicked(ImGuiMouseButton.Left))
        {
            _selectedEntityId = entity.Id;
            _selectedResourceType = null;
        }

        if (opened && hasAny)
        {
            if (entity.Components.Count > 0)
            {
                if (ImGui.TreeNodeEx($"{Lucide.ServerCog} Components", group_flags))
                {
                    foreach (var component in entity.Components)
                    {
                        var type = component.Type.Type;
                        var key = (entity.Id, type);

                        ImGui.TreeNodeEx(
                            $"{Lucide.Settings} {type.Name}",
                            item_flags
                        );

                        // Double click — open floating inspector for this specific component
                        if (ImGui.IsItemHovered() && ImGui.IsMouseDoubleClicked(ImGuiMouseButton.Left))
                            _openInspectors[key] = true;
                    }
                    ImGui.TreePop();
                }
            }

            if (entity.Tags.Count > 0)
            {
                if (ImGui.TreeNodeEx($"{Lucide.Tags} Tags", group_flags))
                {
                    foreach (var tag in entity.Tags)
                    {
                        var type = tag.Type;

                        ImGui.TreeNodeEx(
                            $"{Lucide.Tag} {type.Name}",
                            item_flags
                        );
                    }
                    ImGui.TreePop();
                }
            }

            foreach (var child in entity.ChildEntities)
                DrawEntity(child);

            ImGui.TreePop();
        }
    }

    private void DrawEntityInspector()
    {
        ImGui.SetNextWindowSize(new Vector2(300, 400), ImGuiCond.FirstUseEver);

        if (!ImGui.Begin("Inspector"))
        {
            ImGui.End();
            return;
        }

        if (_selectedEntityId != null)
            DrawSelectedEntity();
        else if (_selectedResourceType != null)
            DrawSelectedResource();
        else
            ImGui.TextDisabled("Nothing selected.");

        ImGui.End();
    }

    private void DrawSelectedEntity()
    {
        var entity = _world.Entities.FirstOrDefault(e => e.Id == _selectedEntityId);

        if (entity.IsNull)
        {
            ImGui.TextDisabled("Entity no longer exists.");
            _selectedEntityId = null;
            return;
        }

        ImGui.Text($"{Lucide.Box} Entity {entity.Id}");
        ImGui.Separator();

        if (entity.Components.Count == 0 && entity.Tags.Count == 0)
        {
            ImGui.TextDisabled("No components or tags.");
            return;
        }

        // Components section
        if (entity.Components.Count > 0)
        {
            ImGui.Text($"{Lucide.ServerCog} Components");
            foreach (var component in entity.Components)
            {
                var type = component.Type.Type;
                var key = (entity.Id, type);

                ImGui.SetNextItemAllowOverlap();

                bool open = component.Type.Type.GetFields(BindingFlags.Public | BindingFlags.Instance).Length != 0
                    ? ImGui.CollapsingHeader($"{Lucide.Settings} {type.Name}", ImGuiTreeNodeFlags.SpanFullWidth)
                    : ImGui.CollapsingHeader($"{Lucide.Settings} {type.Name}", ImGuiTreeNodeFlags.Leaf | ImGuiTreeNodeFlags.SpanFullWidth);

                ImGui.SameLine(ImGui.GetContentRegionAvail().X - 20);
                if (ImGui.SmallButton($"{Lucide.ExternalLink}##{type.Name}"))
                    _openInspectors[key] = true;
                ImGui.SetItemTooltip("Make floating");

                if (open)
                {
                    ImGui.Indent();
                    DrawComponentInspector(entity, type);
                    ImGui.Unindent();
                }
            }
        }

        // Tags section
        if (entity.Tags.Count > 0)
        {
            ImGui.Separator();
            ImGui.Text($"{Lucide.Tags} Tags");

            foreach (var tag in entity.Tags)
                ImGui.TreeNodeEx($"{Lucide.Tag} {tag.Type.Name}", ImGuiTreeNodeFlags.Leaf | ImGuiTreeNodeFlags.FramePadding | ImGuiTreeNodeFlags.SpanFullWidth);
            ImGui.TreePop();
        }
    }

    private void DrawSelectedResource()
    {
        var resources = cx.GetAllResources();

        if (!resources.TryGetValue(_selectedResourceType!, out var resource))
        {
            ImGui.TextDisabled("Resource no longer exists.");
            _selectedResourceType = null;
            return;
        }

        var resource_name = _selectedResourceType!.GetFriendlyName();
        ImGui.Text($"{Lucide.Database} {resource_name}");
        ImGui.Separator();

        var fields = _selectedResourceType!.GetFields(BindingFlags.Public | BindingFlags.Instance);
        var props = _selectedResourceType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        if (fields.Length == 0 && props.Length == 0)
        {
            ImGui.TextDisabled("No inspectable data.");
            return;
        }

        // Resources are classes so we can edit directly without boxing tricks
        foreach (var field in fields)
        {
            var value = field.GetValue(resource);
            var result = DrawEditableField(field.Name, value);
            if (result.modified)
                field.SetValue(resource, result.value);
        }

        // Properties (read-only for now since setters may have side effects)
        foreach (var prop in props)
        {
            if (!prop.CanRead) continue;
            var value = prop.GetValue(resource);
            ImGui.TextDisabled($"{prop.Name}: {value?.ToString() ?? "null"}");
        }
    }

    private void DrawComponentInspector(Entity entity, Type componentType)
    {
        var getMethod = typeof(Entity)
            .GetMethods()
            .First(m =>
                m.Name == "GetComponent" &&
                m.IsGenericMethodDefinition &&
                m.GetGenericArguments().Length == 1 &&
                m.GetParameters().Length == 0
            )
            .MakeGenericMethod(componentType);

        var component = getMethod.Invoke(entity, null);
        if (component == null) return;

        bool modified = false;

        foreach (var field in componentType.GetFields(BindingFlags.Public | BindingFlags.Instance))
        {
            var value = field.GetValue(component);
            var result = DrawEditableField(field.Name, value);

            if (result.modified)
            {
                field.SetValue(component, result.value); // modify the boxed copy
                modified = true;
            }
        }

        if (modified)
            GetSetter(componentType)(entity, component!);
    }

    private Action<Entity, object> GetSetter(Type componentType)
    {
        if (_setterCache.TryGetValue(componentType, out var setter))
            return setter;

        // Build a typed lambda that handles the struct correctly
        var method = typeof(WorldHierarchyDebug<G>)
            .GetMethod(nameof(TypedSet), BindingFlags.NonPublic | BindingFlags.Static)!
            .MakeGenericMethod(componentType);

        setter = (entity, component) => method.Invoke(null, [entity, component]);
        _setterCache[componentType] = setter;
        return setter;
    }

    private static void TypedSet<T>(Entity entity, object component) where T : struct, IComponent
    {
        var typed = (T)component;
        entity.Set(typed); // called generically, no ref issues
    }

    private (bool modified, object? value) DrawEditableField(string name, object? value)
    {
        // 1. Insert spaces before every capital letter
        string spaced = FieldRegex().Replace(name, " $1").Trim();

        // 2. Capitalize the first letter of the entire string
        string field_name = char.ToUpper(spaced[0]) + spaced.Substring(1);

        switch (value)
        {
            case int i:
                {
                    ImGui.DragInt(field_name, ref i);
                    return (ImGui.IsItemEdited(), i);
                }
            case float f:
                {
                    ImGui.DragFloat(field_name, ref f, 0.1f, default, default, "%.3f");
                    return (ImGui.IsItemEdited(), f);
                }
            case bool b:
                {
                    ImGui.Checkbox(field_name, ref b);
                    return (ImGui.IsItemEdited(), b);
                }
            case Vector2 v:
                {
                    Vector2 imVec = new(v.X, v.Y);
                    ImGui.DragFloat2(field_name, ref imVec);
                    return (ImGui.IsItemEdited(), new Vector2(imVec.X, imVec.Y));
                }
            case Vector3 v:
                {
                    Vector3 imVec = new(v.X, v.Y, v.Z);
                    ImGui.DragFloat3(field_name, ref imVec);
                    // bool changed = ImGui.IsItemDeactivatedAfterEdit();
                    return (ImGui.IsItemEdited(), new Vector3(imVec.X, imVec.Y, imVec.Z));
                }
            case Math.Vector2i v:
                {
                    int[] vir = [v.X, v.Y];
                    ImGui.DragInt2(field_name, ref vir[0]);
                    return (ImGui.IsItemEdited(), new Math.Vector2i(vir[0], vir[1]));
                }
            case List<int> li:
                {
                    int selected_idx = 0;
                    if (ImGui.BeginListBox(field_name))
                    {
                        foreach (int i in li)
                        {
                            bool is_selected = selected_idx == i;
                            if (ImGui.Selectable($"{i}", is_selected))
                                selected_idx = i;

                            if (is_selected)
                                ImGui.SetItemDefaultFocus();
                        }
                        ImGui.EndListBox();
                    }

                    return (false, li);
                }
            default:
                ImGui.TextDisabled($"{field_name}: {value?.ToString() ?? "null"}");
                return (false, value);
        }
    }

    [GeneratedRegex(@"([A-Z])")]
    private static partial Regex FieldRegex();
}
