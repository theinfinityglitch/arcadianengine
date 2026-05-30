using System.Numerics;
using System.Reflection;
using System.Text.RegularExpressions;
using ArcadianEngine.Math;
using ArcadianEngine.Utils;
using Friflo.Engine.ECS;
using IconFonts;
using ImGuiNET;
using Raylib_cs;

namespace ArcadianEngine.Resources;

public partial class WorldHierarchyDebug<TG>(GameContext<TG> cx) where TG : class, IArcadianGame<TG>
{
    private readonly EntityStore _world = cx.game.World;
    private readonly Dictionary<(int, Type), bool> _openInspectors = [];
    private readonly Dictionary<Type, Action<Entity, object>> _setterCache = [];
    private int? _selectedEntityId;
    private Type? _selectedResourceType;

    public void Draw()
    {
        // Hierarchy window
        if (ImGui.Begin($"{Lucide.Earth} World Hierarchy"))
        {
            var mainFlags = ImGuiTreeNodeFlags.SpanFullWidth | ImGuiTreeNodeFlags.FramePadding;

            // Resources node
            var resources = cx.GetAllResources();
            if (ImGui.TreeNodeEx($"{Lucide.Package} Resources", mainFlags))
            {
                foreach (var (type, _) in resources)
                {
                    var itemFlags = ImGuiTreeNodeFlags.Leaf | ImGuiTreeNodeFlags.NoTreePushOnOpen | ImGuiTreeNodeFlags.FramePadding | ImGuiTreeNodeFlags.SpanFullWidth;
                    var isSelected = _selectedResourceType == type;
                    var name = type.GetFriendlyName();

                    if (isSelected) itemFlags |= ImGuiTreeNodeFlags.Selected;

                    ImGui.TreeNodeEx($"{Lucide.Database} {name}", itemFlags);

                    if (ImGui.IsItemClicked(ImGuiMouseButton.Left))
                    {
                        _selectedResourceType = type;
                        _selectedEntityId = null;
                    }
                }
                ImGui.TreePop();
            }

            // Entities node
            if (ImGui.TreeNodeEx($"{Lucide.Boxes} Entities", mainFlags))
            {
                foreach (var entity in _world.Entities)
                {
                    if (!entity.IsNull && entity.Parent.IsNull)
                        DrawEntity(entity);
                }
                ImGui.TreePop();
            }
        }
        ImGui.End();

        DrawEntityInspector();

        foreach (var key in _openInspectors.Keys.ToList())
        {
            var (entityId, type) = key;
            var open = _openInspectors[key];

            if (!open) continue;

            // Find the entity
            var entity = _world.Entities.FirstOrDefault(e => e.Id == entityId);
            if (entity.IsNull)
            {
                _openInspectors.Remove(key);
                continue;
            }

            string entityName;

            if (entity.TryGetComponent<EntityName>(out var name)) entityName = name.value;
            else entityName = $"Entity {entity.Id}";

            var title = $"{Lucide.Settings} {type.Name} ({entityName})";
            ImGui.SetNextWindowSize(new Vector2(300, 200), ImGuiCond.FirstUseEver);

            if (ImGui.Begin(title, ref open))
                DrawComponentInspector(type.Name, entity, type);

            ImGui.End();

            _openInspectors[key] = open; // sync close button
        }
    }

    private void DrawEntity(Entity entity)
    {
        var hasChildren = entity.ChildEntities.Count > 0;
        var hasAny = hasChildren || entity.Components.Count > 0 || entity.Tags.Count > 0;
        var isSelected = _selectedEntityId == entity.Id;

        var entityFlags = ImGuiTreeNodeFlags.OpenOnArrow | ImGuiTreeNodeFlags.OpenOnDoubleClick | ImGuiTreeNodeFlags.SpanFullWidth | ImGuiTreeNodeFlags.FramePadding;
        var groupFlags = ImGuiTreeNodeFlags.SpanFullWidth | ImGuiTreeNodeFlags.FramePadding;
        var itemFlags = ImGuiTreeNodeFlags.Leaf | ImGuiTreeNodeFlags.NoTreePushOnOpen | ImGuiTreeNodeFlags.FramePadding | ImGuiTreeNodeFlags.SpanFullWidth;

        if (!hasAny) entityFlags |= ImGuiTreeNodeFlags.Leaf | ImGuiTreeNodeFlags.NoTreePushOnOpen;
        if (isSelected) entityFlags |= ImGuiTreeNodeFlags.Selected;
        string entityName;

        if (entity.TryGetComponent<EntityName>(out var name)) entityName = name.value;
        else entityName = $"Entity {entity.Id}";

        var opened = ImGui.TreeNodeEx($"{Lucide.Box} {entityName}", entityFlags);

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
                if (ImGui.TreeNodeEx($"{Lucide.ServerCog} Components", groupFlags))
                {
                    foreach (var component in entity.Components)
                    {
                        var type = component.Type.Type;
                        var key = (entity.Id, type);

                        ImGui.TreeNodeEx(
                            $"{Lucide.Settings} {type.Name}",
                            itemFlags
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
                if (ImGui.TreeNodeEx($"{Lucide.Tags} Tags", groupFlags))
                {
                    foreach (var tag in entity.Tags)
                    {
                        var type = tag.Type;

                        ImGui.TreeNodeEx(
                            $"{Lucide.Tag} {type.Name}",
                            itemFlags
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

        string entityName;

        if (entity.TryGetComponent<EntityName>(out var name)) entityName = name.value;
        else entityName = $"Entity {entity.Id}";

        ImGui.Text($"{Lucide.Box} {entityName}");
        ImGui.Separator();

        if (entity.Components.Count == 0 && entity.Tags.Count == 0)
        {
            ImGui.TextDisabled("No components or tags.");
            return;
        }

        // Components section
        if (entity.Components.Count > 0)
        {
            if (ImGui.TreeNodeEx($"{Lucide.ServerCog} Components", ImGuiTreeNodeFlags.FramePadding | ImGuiTreeNodeFlags.DefaultOpen | ImGuiTreeNodeFlags.SpanFullWidth))
            {
                ImGui.Unindent();
                foreach (var component in entity.Components)
                {
                    var type = component.Type.Type;
                    var key = (entity.Id, type);

                    ImGui.SetNextItemAllowOverlap();

                    var fields = type
                        .GetFields(BindingFlags.Public | BindingFlags.Instance)
                        .Where(f => f.GetCustomAttribute<ExportAttribute>() != null).ToArray();
                    var open = fields.Length != 0
                        ? ImGui.CollapsingHeader($"{Lucide.Settings} {type.Name}", ImGuiTreeNodeFlags.SpanFullWidth)
                        : ImGui.CollapsingHeader($"{Lucide.Settings} {type.Name}", ImGuiTreeNodeFlags.Leaf | ImGuiTreeNodeFlags.SpanFullWidth);

                    ImGui.SameLine(ImGui.GetContentRegionAvail().X - 20);
                    if (ImGui.SmallButton($"{Lucide.ExternalLink}##{type.Name}"))
                        _openInspectors[key] = true;
                    ImGui.SetItemTooltip("Make floating");

                    if (open)
                    {
                        ImGui.Indent();
                        DrawComponentInspector(type.Name, entity, type);
                        ImGui.Unindent();
                    }
                }
                ImGui.Indent();
                ImGui.TreePop();
            }
        }

        // Tags section
        if (entity.Tags.Count > 0)
        {
            ImGui.Separator();
            if (ImGui.TreeNodeEx($"{Lucide.Tags} Tags", ImGuiTreeNodeFlags.FramePadding | ImGuiTreeNodeFlags.DefaultOpen | ImGuiTreeNodeFlags.SpanFullWidth))
            {
                ImGui.Unindent();

                foreach (var tag in entity.Tags)
                {
                    ImGui.TreeNodeEx($"{Lucide.Tag} {tag.Type.Name}", ImGuiTreeNodeFlags.Leaf | ImGuiTreeNodeFlags.FramePadding | ImGuiTreeNodeFlags.SpanFullWidth);
                    ImGui.TreePop();
                }

                ImGui.Indent();
                ImGui.TreePop();
            }
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

        var resourceName = _selectedResourceType!.GetFriendlyName();
        ImGui.Text($"{Lucide.Database} {resourceName}");
        ImGui.Separator();

        var fields = _selectedResourceType!
            .GetFields(BindingFlags.Public | BindingFlags.Instance)
            .Where(f => f.GetCustomAttribute<ExportAttribute>() != null)
            .ToArray();
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
            var result = DrawEditableField(resourceName, field.Name, value);
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

    private void DrawComponentInspector(string componentName, Entity entity, Type componentType)
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
        var fields = componentType
            .GetFields(BindingFlags.Public | BindingFlags.Instance)
            .Where(f => f.GetCustomAttribute<ExportAttribute>() != null);
        if (component == null) return;

        var modified = false;

        foreach (var field in fields)
        {
            var value = field.GetValue(component);
            var result = DrawEditableField(componentName, field.Name, value);

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
        var method = typeof(WorldHierarchyDebug<TG>)
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

    private (bool modified, object? value) DrawEditableField(string differentialName, string name, object? value)
    {
        // 1. Insert spaces before every capital letter
        var spaced = FieldRegex().Replace(name, " $1").Trim();

        // 2. Capitalize the first letter of the entire string
        var fieldName = char.ToUpper(spaced[0]) + spaced[1..];
        fieldName += $"##{differentialName}";

        switch (value)
        {
            case int i:
                {
                    ImGui.DragInt(fieldName, ref i);
                    return (ImGui.IsItemEdited(), i);
                }
            case float f:
                {
                    ImGui.DragFloat(fieldName, ref f, 0.1f, default, default, "%.3f");
                    return (ImGui.IsItemEdited(), f);
                }
            case bool b:
                {
                    ImGui.Checkbox(fieldName, ref b);
                    return (ImGui.IsItemEdited(), b);
                }
            case string s:
                {
                    ImGui.InputText(fieldName, ref s, 256);
                    return (ImGui.IsItemEdited(), s);
                }
            case Vector2 v:
                {
                    Vector2 imVec = new(v.X, v.Y);
                    ImGui.DragFloat2(fieldName, ref imVec);
                    return (ImGui.IsItemEdited(), new Vector2(imVec.X, imVec.Y));
                }
            case Vector3 v:
                {
                    Vector3 imVec = new(v.X, v.Y, v.Z);
                    ImGui.DragFloat3(fieldName, ref imVec);
                    // bool changed = ImGui.IsItemDeactivatedAfterEdit();
                    return (ImGui.IsItemEdited(), new Vector3(imVec.X, imVec.Y, imVec.Z));
                }
            case Vector2I v:
                {
                    int[] vir = [v.X, v.Y];
                    ImGui.DragInt2(fieldName, ref vir[0]);
                    return (ImGui.IsItemEdited(), new Vector2I(vir[0], vir[1]));
                }
            case List<int> li:
                {
                    var selectedIdx = 0;
                    if (ImGui.BeginListBox(fieldName))
                    {
                        foreach (var i in li)
                        {
                            var isSelected = selectedIdx == i;
                            if (ImGui.Selectable($"{i}", isSelected))
                                selectedIdx = i;

                            if (isSelected)
                                ImGui.SetItemDefaultFocus();
                        }
                        ImGui.EndListBox();
                    }

                    return (false, li);
                }
            case Color c:
                {
                    Vector4 colorVec = new(
                        c.R / 255f, c.G / 255f, c.B / 255f, c.A / 255f
                    );
                    ImGui.ColorEdit4(fieldName, ref colorVec);

                    return (ImGui.IsItemEdited(), new Color(
                        (byte)(colorVec.X * 255),
                        (byte)(colorVec.Y * 255),
                        (byte)(colorVec.Z * 255),
                        (byte)(colorVec.W * 255))
                    );
                }
            default:
                ImGui.TextDisabled($"{fieldName}: {value?.ToString() ?? "null"}");
                return (false, value);
        }
    }

    [GeneratedRegex(@"([A-Z])")]
    private static partial Regex FieldRegex();
}
