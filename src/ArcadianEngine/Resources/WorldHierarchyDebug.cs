using System.Numerics;
using System.Reflection;
using Friflo.Engine.ECS;
using ImGuiNET;
using IconFonts;

namespace ArcadianEngine.Resources;

public class WorldHierarchyDebug<G>(GameContext<G> cx) where G : class, IArcadianGame<G>
{
    private readonly EntityStore _world = cx.Game.world;
    private int? _selectedEntityId = null;
    private readonly Dictionary<(int, Type), bool> _openInspectors = [];
    private readonly Dictionary<Type, Action<Entity, object>> _setterCache = new();

    public void Draw()
    {
        // Hierarchy window
        if (ImGui.Begin($"{FontAwesome5.Cubes} World Hierarchy"))
        {
            foreach (var entity in _world.Entities)
            {
                if (!entity.IsNull && entity.Parent.IsNull)
                    DrawEntity(entity);
            }
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

            var title = $"{FontAwesome5.Cog} {type.Name} (Entity {entityId})";
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
        var hasAny = hasChildren || entity.Components.Count > 0;
        var isSelected = _selectedEntityId == entity.Id;

        var flags = ImGuiTreeNodeFlags.OpenOnArrow | ImGuiTreeNodeFlags.SpanAvailWidth;

        if (!hasAny) flags |= ImGuiTreeNodeFlags.Leaf | ImGuiTreeNodeFlags.NoTreePushOnOpen;
        if (isSelected) flags |= ImGuiTreeNodeFlags.Selected;

        bool opened = ImGui.TreeNodeEx($"{FontAwesome5.Cube} Entity {entity.Id}", flags);

        // Single click — select entity
        if (ImGui.IsItemClicked(ImGuiMouseButton.Left))
            _selectedEntityId = entity.Id;

        if (opened && hasAny)
        {
            if (entity.Components.Count > 0)
            {
                if (ImGui.TreeNodeEx($"{FontAwesome5.Cogs} Components", ImGuiTreeNodeFlags.SpanAvailWidth))
                {
                    foreach (var component in entity.Components)
                    {
                        var type = component.Type.Type;
                        var key = (entity.Id, type);

                        ImGui.TreeNodeEx(
                            $"{FontAwesome5.Cog} {type.Name}",
                            ImGuiTreeNodeFlags.Leaf | ImGuiTreeNodeFlags.NoTreePushOnOpen
                        );

                        // Double click — open floating inspector for this specific component
                        if (ImGui.IsItemHovered() && ImGui.IsMouseDoubleClicked(ImGuiMouseButton.Left))
                            _openInspectors[key] = true;
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

        if (_selectedEntityId == null)
        {
            ImGui.TextDisabled("No entity selected.");
            ImGui.End();
            return;
        }

        var entity = _world.Entities.FirstOrDefault(e => e.Id == _selectedEntityId);

        if (entity.IsNull)
        {
            ImGui.TextDisabled("Entity no longer exists.");
            _selectedEntityId = null;
            ImGui.End();
            return;
        }

        ImGui.Text($"{FontAwesome5.Cube} Entity {entity.Id}");
        ImGui.Separator();

        if (entity.Components.Count == 0)
        {
            ImGui.TextDisabled("No components.");
            ImGui.End();
            return;
        }

        // List all components as collapsible headers
        foreach (var component in entity.Components)
        {
            var type = component.Type.Type;
            var key = (entity.Id, type);

            // Allow the button to overlap the header
            ImGui.SetNextItemAllowOverlap();
            bool open = ImGui.CollapsingHeader($"{FontAwesome5.Cog} {type.Name}", ImGuiTreeNodeFlags.DefaultOpen);

            // Position button on the right side of the header
            ImGui.SameLine(ImGui.GetContentRegionAvail().X - 20);
            if (ImGui.SmallButton($"{FontAwesome5.ExternalLinkAlt}##{type.Name}"))
                _openInspectors[key] = true;
            ImGui.SetItemTooltip("Make floating");

            if (open)
            {
                ImGui.Indent();
                DrawComponentInspector(entity, type);
                ImGui.Unindent();
            }
        }

        ImGui.End();
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
        switch (value)
        {
            case int i:
                {
                    ImGui.DragInt(name, ref i);
                    return (ImGui.IsItemEdited(), i);
                }
            case float f:
                {
                    ImGui.DragFloat(name, ref f, 0.1f, default, default, "%.3f");
                    return (ImGui.IsItemEdited(), f);
                }
            case bool b:
                {
                    ImGui.Checkbox(name, ref b);
                    return (ImGui.IsItemDeactivatedAfterEdit(), b);
                }
            case Vector2 v:
                {
                    Vector2 imVec = new(v.X, v.Y);
                    ImGui.InputFloat2(name, ref imVec);
                    bool changed = ImGui.IsItemDeactivatedAfterEdit();
                    return (changed, new Vector2(imVec.X, imVec.Y));
                }
            case Vector3 v:
                {
                    Vector3 imVec = new(v.X, v.Y, v.Z);
                    ImGui.InputFloat3(name, ref imVec);
                    bool changed = ImGui.IsItemDeactivatedAfterEdit();
                    return (changed, new Vector3(imVec.X, imVec.Y, imVec.Z));
                }
            case List<int> li:
                {
                    int selected_idx = 0;
                    if (ImGui.BeginListBox(name))
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
                ImGui.TextDisabled($"{name}: {value?.ToString() ?? "null"}");
                return (false, value);
        }
    }
}
