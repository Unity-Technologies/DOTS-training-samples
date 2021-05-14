#define DONT_EAT_MOUSE
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;

public class CatCollisionSystem : SystemBase
{
    EntityQuery m_CatQuery;
    private EntityCommandBufferSystem CommandBufferSystem;

    protected override void OnCreate()
    {
        CommandBufferSystem
            = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        RequireSingletonForUpdate<GameStartedTag>();
        m_CatQuery = GetEntityQuery(
            new EntityQueryDesc
            {
                All = new ComponentType[]
                {
                    ComponentType.ReadOnly<CatTag>(),
                    ComponentType.ReadOnly<GridPosition>(),
                    ComponentType.ReadOnly<CellOffset>(),
                    ComponentType.ReadOnly<Direction>(),
                    ComponentType.ReadOnly<ColliderSize>()
                },
                // Prevent cats from eating all the mice when they are falling to their death
                None = new ComponentType[]
                {
                    ComponentType.ReadOnly<FallingTime>()
                }
            });
    }

    protected override void OnUpdate()
    {
        // Grab all the Cats and put their positions in a dynamic buffer
        // TODO: I have no idea if this will work
        var catEntities = m_CatQuery.ToEntityArray(Allocator.Temp);
        var catPositions = m_CatQuery.ToComponentDataArray<GridPosition>(Allocator.Temp);
        var catOffsets = m_CatQuery.ToComponentDataArray<CellOffset>(Allocator.Temp);
        var catDirections = m_CatQuery.ToComponentDataArray<Direction>(Allocator.Temp);
        var catColliders = m_CatQuery.ToComponentDataArray<ColliderSize>(Allocator.Temp);

        var ecb = CommandBufferSystem.CreateCommandBuffer().AsParallelWriter();
        // Foreach on all the mice and check if they collide
        Entities
            .WithNone<CatTag>()
            .ForEach((Entity entity, int entityInQueryIndex, in GridPosition gridPosition, in CellOffset cellOffset, in Direction direction, in ColliderSize colliderSize) =>
            {
                var offsetX = 0;
                var offsetY = 0;

                UpdateTransformSystem.GetOffsetDirs(ref offsetX, ref offsetY, in direction);

                var mouseX = gridPosition.X + (cellOffset.Value - 0.5f) * offsetX;
                var mouseY = gridPosition.Y - (cellOffset.Value - 0.5f) * offsetY;

                // TODO: Check the distance to each cat
                for (int i = 0; i < catPositions.Length; i++)
                {
                    var catDir = catDirections[i];
                    UpdateTransformSystem.GetOffsetDirs(ref offsetX, ref offsetY, in catDir);

                    var catX = catPositions[i].X + (catOffsets[i].Value - 0.5f) * offsetX;
                    var catY = catPositions[i].Y - (catOffsets[i].Value - 0.5f) * offsetY;

                    var distance = math.distance(new float2(mouseX, mouseY), new float2(catX, catY));

                    if (distance < colliderSize.Value * .5f + catColliders[i].Value * .5f)
                    {
                        ecb.AddComponent(entityInQueryIndex, catEntities[i],
                            new BounceScaleAnimationProperties(){ AccumulatedTime = 0.0f, AnimationDuration = 0.5f, OriginalScale = 1.0f, TargetScale = 1.4f});

                        #if !DONT_EAT_MOUSE
                            ecb.DestroyEntity(entityInQueryIndex,entity);
                        #endif
                    }
                }
            }).Run();
    }
}
