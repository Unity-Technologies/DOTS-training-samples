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

    [BurstCompile]
    public struct RaycastJob : IJob
    {
        [ReadOnly] public Entity PreviewArrow;
        public RaycastInput RaycastInput;
        public EntityCommandBuffer Ecb;
        [ReadOnly] public bool PlayerInput;
        [ReadOnly] public float2 CellSize;
        [ReadOnly] public EntityArchetype ArrowRequestArchetype;
        [ReadOnly] public PhysicsWorld World;

        public void Execute()
        {
            if (World.CastRay(RaycastInput, out RaycastHit hit))
            {
                float2 worldPos = new float2(hit.Position.x, hit.Position.z);

                GridDirection direction = GridDirection.EAST;
                float2 localCellPos = new float2(worldPos.x % CellSize.x - CellSize.x / 2, worldPos.y % CellSize.y - CellSize.y / 2);
                if (math.abs(localCellPos.y) > math.abs(localCellPos.x))
                    direction = localCellPos.y < 0 ? GridDirection.SOUTH : GridDirection.NORTH;
                else
                    direction = localCellPos.x < 0 ? GridDirection.WEST : GridDirection.EAST;

                Ecb.SetComponent(PreviewArrow, new Position2D { Value = worldPos - localCellPos });
                Ecb.SetComponent(PreviewArrow, new Rotation2D { Value = Utility.DirectionToAngle(direction) });
                if (PlayerInput)
                {
                    Entity arrowRequest = Ecb.CreateEntity(ArrowRequestArchetype);

                    Ecb.SetComponent(arrowRequest, new ArrowRequest
                    {
                        Direction = direction,
                        Position = Utility.WorldPositionToGridCoordinates(worldPos, CellSize),
                        OwnerID = 0
                    });
                }
            }
        }
    }

    protected override void OnCreate()
    {
        commandBufferSystem = World.GetExistingSystem<EndInitializationEntityCommandBufferSystem>();
        arrowRequestArchetype = EntityManager.CreateArchetype(typeof(ArrowRequest));
    }

    protected override void OnUpdate()
    {
        if (previewArrow == Entity.Null)
        {
            previewArrow = EntityManager.Instantiate(GetSingleton<PrefabReferenceComponent>().ArrowPrefab);
        }
        else
        {
            var ecb = commandBufferSystem.CreateCommandBuffer();

            UnityEngine.Ray ray = UnityEngine.Camera.main.ScreenPointToRay(UnityEngine.Input.mousePosition);
            RaycastInput raycastInput = new RaycastInput { Start = ray.origin, End = ray.origin + ray.direction * float.MaxValue, Filter = CollisionFilter.Default };

            bool playerInput = UnityEngine.Input.GetMouseButtonDown(0);

            var job = new RaycastJob
            {
                PreviewArrow = previewArrow,
                RaycastInput = raycastInput,
                Ecb = ecb,
                PlayerInput = playerInput,
                CellSize = new float2(ConstantData.Instance.CellSize.x, ConstantData.Instance.CellSize.y),
                ArrowRequestArchetype = arrowRequestArchetype,
                World = World.GetExistingSystem<BuildPhysicsWorld>().PhysicsWorld,
            };

            job.Run();
        }
    }
}