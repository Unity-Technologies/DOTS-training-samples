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


    [BurstCompile]
    public struct RaycastJob : IJob
    {
        public RaycastInput RaycastInput;
        public EntityCommandBuffer Ecb;
        [ReadOnly] public bool PlayerInput;
        [ReadOnly] public float2 CellSize;
        [ReadOnly] public EntityArchetype ArrowRequestArchetype;
        [ReadOnly] public PhysicsWorld World;

        public void Execute()
        {
            if (PlayerInput && World.CastRay(RaycastInput, out RaycastHit hit))
            {
                Entity arrowRequest = Ecb.CreateEntity(ArrowRequestArchetype);

                Ecb.SetComponent(arrowRequest, new ArrowRequest
                {
                    Direction = GridDirection.EAST,
                    Position = Utility.WorldPositionToGridCoordinates(new float2(hit.Position.x, hit.Position.y), CellSize),
                    OwnerID = 0
                });
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
        var ecb = commandBufferSystem.CreateCommandBuffer();
        
        UnityEngine.Ray ray = UnityEngine.Camera.main.ScreenPointToRay(UnityEngine.Input.mousePosition);
        RaycastInput raycastInput = new RaycastInput { Start = ray.origin, End = ray.origin + ray.direction * float.MaxValue, Filter = CollisionFilter.Default };
        
        bool playerInput = UnityEngine.Input.GetMouseButtonDown(0);

        var job = new RaycastJob
        {
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