using System.Numerics;
using ArcadianEngine.Components;
using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;

namespace ArcadianEngine.Systems;

public class TransformPropagationSystem : QuerySystem<Transform2D>
{
    // Separate query for entities that have children
    private ArchetypeQuery<Transform2D, TreeNode>? _parentQuery;

    protected override void OnAddStore(EntityStore store)
    {
        _parentQuery = store.Query<Transform2D, TreeNode>();
    }

    protected override void OnUpdate()
    {
        // Only bother with hierarchy propagation if any parented entities exist
        if (_parentQuery?.Count == 0)
        {
            Query.ForEachEntity((ref Transform2D transform, Entity _) =>
            {
                transform.GlobalPosition = transform.Position;
                transform.GlobalScale = transform.Scale;
                transform.GlobalRotation = transform.Rotation;
                transform.GlobalZIndex = transform.ZIndex;
            });
            return;
        }

        Query.ForEachEntity((ref Transform2D _, Entity entity) =>
        {
            if (entity.Parent.IsNull)
                PropagateDown(entity, null);
        });
    }

    private void PropagateDown(Entity entity, Transform2D? parentTransform)
    {
        if (!entity.TryGetComponent<Transform2D>(out var transform)) return;
        // if (!entity.TryGetComponent<GlobalTransform2D>(out var global)) return;

        if (parentTransform == null)
        {
            transform.GlobalPosition = transform.Position;
            transform.GlobalScale = transform.Scale;
            transform.GlobalRotation = transform.Rotation;
            transform.GlobalZIndex = transform.ZIndex;
        }
        else
        {
            var cos = MathF.Cos(parentTransform.Value.Rotation);
            var sin = MathF.Sin(parentTransform.Value.Rotation);

            transform.GlobalPosition = parentTransform.Value.Position + new Vector2(
                transform.Position.X * cos - transform.Position.Y * sin,
                transform.Position.X * sin + transform.Position.Y * cos
            );

            transform.GlobalScale = parentTransform.Value.Scale * transform.Scale;
            transform.GlobalRotation = parentTransform.Value.Rotation + transform.Rotation;
            transform.GlobalZIndex = parentTransform.Value.ZIndex + transform.ZIndex;
        }

        entity.Set(transform);

        foreach (var child in entity.ChildEntities)
            PropagateDown(child, transform);
    }
}
