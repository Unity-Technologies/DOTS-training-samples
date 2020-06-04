using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;

class PlayerInputSystem : SystemBase
{
    private EntityCommandBufferSystem commandBufferSystem;
    private EntityArchetype arrowRequestArchetype;
    private Entity previewArrow;

    protected override void OnCreate()
    {
        commandBufferSystem = World.GetExistingSystem<EndInitializationEntityCommandBufferSystem>();
        arrowRequestArchetype = EntityManager.CreateArchetype(typeof(ArrowRequest));
    }

    protected override void OnUpdate()
    {
        if (previewArrow == Entity.Null)
        {
            previewArrow = EntityManager.Instantiate(GetSingleton<PrefabReferenceComponent>().PreviewArrowPrefab);
        }
        else
        {
            var ecb = commandBufferSystem.CreateCommandBuffer();

            UnityEngine.Ray ray = UnityEngine.Camera.main.ScreenPointToRay(UnityEngine.Input.mousePosition);
            RaycastInput raycastInput = new RaycastInput { Start = ray.origin, End = ray.origin + ray.direction * float.MaxValue, Filter = CollisionFilter.Default };
            bool playerInput = UnityEngine.Input.GetMouseButtonDown(0);
            PhysicsWorld physicsWorld = World.GetExistingSystem<BuildPhysicsWorld>().PhysicsWorld;
            float2 cellSize = new float2(ConstantData.Instance.CellSize.x, ConstantData.Instance.CellSize.y);
            Entity tmpPreviewArrow = previewArrow;
            EntityArchetype tmpArrowRequestArchetype = arrowRequestArchetype;

            Job.WithCode(() =>
            {
                if (physicsWorld.CastRay(raycastInput, out RaycastHit hit))
                {
                    float2 worldPos = new float2(hit.Position.x, hit.Position.z);

                    GridDirection direction = GridDirection.EAST;
                    float2 localCellPos = new float2(worldPos.x % cellSize.x - cellSize.x / 2, worldPos.y % cellSize.y - cellSize.y / 2);
                    if (math.abs(localCellPos.y) > math.abs(localCellPos.x))
                        direction = localCellPos.y < 0 ? GridDirection.SOUTH : GridDirection.NORTH;
                    else
                        direction = localCellPos.x < 0 ? GridDirection.WEST : GridDirection.EAST;

                    ecb.SetComponent(tmpPreviewArrow, new Position2D { Value = worldPos - localCellPos });
                    ecb.SetComponent(tmpPreviewArrow, new Rotation2D { Value = Utility.DirectionToAngle(direction) });
                    if (playerInput)
                    {
                        Entity arrowRequest = ecb.CreateEntity(tmpArrowRequestArchetype);

                        ecb.SetComponent(arrowRequest, new ArrowRequest
                        {
                            Direction = direction,
                            Position = Utility.WorldPositionToGridCoordinates(worldPos, cellSize),
                            OwnerID = 0
                        });
                    }
                }
            }).Run();
        }
    }
}
