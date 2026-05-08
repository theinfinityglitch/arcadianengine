using System.Numerics;

using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;

using ArcadianEngine.Components;

namespace ArcadianEngine.Systems;

public class TransformPropagationSystem : QuerySystem<Transform2D, GlobalTransform2D>
{
    // Separate query for entities that have children
    private ArchetypeQuery<Transform2D, GlobalTransform2D, TreeNode>? _parentQuery;

    protected override void OnAddStore(EntityStore store)
    {
        _parentQuery = store.Query<Transform2D, GlobalTransform2D, TreeNode>();
    }

    protected override void OnUpdate()
    {
        // Only bother with hierarchy propagation if any parented entities exist
        if (_parentQuery?.Count == 0)
        {
            Query.ForEachEntity((ref Transform2D local, ref GlobalTransform2D global, Entity _) =>
            {
                global.Position = local.Position;
                global.Scale = local.Scale;
                global.Rotation = local.Rotation;
                global.ZIndex = local.ZIndex;
            });
            return;
        }

        Query.ForEachEntity((ref Transform2D _, ref GlobalTransform2D _, Entity entity) =>
        {
            if (entity.Parent.IsNull)
                PropagateDown(entity, null);
        });
    }

    private void PropagateDown(Entity entity, GlobalTransform2D? parentGlobal)
    {
        if (!entity.TryGetComponent<Transform2D>(out var local)) return;
        if (!entity.TryGetComponent<GlobalTransform2D>(out var global)) return;

        if (parentGlobal == null)
        {
            global.Position = local.Position;
            global.Scale = local.Scale;
            global.Rotation = local.Rotation;
            global.ZIndex = local.ZIndex;
        }
        else
        {
            float cos = MathF.Cos(parentGlobal.Value.Rotation);
            float sin = MathF.Sin(parentGlobal.Value.Rotation);

            global.Position = parentGlobal.Value.Position + new Vector2(
                local.Position.X * cos - local.Position.Y * sin,
                local.Position.X * sin + local.Position.Y * cos
            );

            global.Scale = parentGlobal.Value.Scale * local.Scale;
            global.Rotation = parentGlobal.Value.Rotation + local.Rotation;
            global.ZIndex = parentGlobal.Value.ZIndex + local.ZIndex;
        }

        entity.Set(global);

        foreach (var child in entity.ChildEntities)
            PropagateDown(child, global);
    }
}
