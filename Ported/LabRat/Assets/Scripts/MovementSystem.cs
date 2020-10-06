using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class MovementSystem : SystemBase
{
    private const byte North = 0b0000_0001;
    private const byte South = 0b0000_0010;
    private const byte East = 0b0000_0100;
    private const byte West = 0b0000_1000;

    private EntityCommandBufferSystem m_ECBSystem;

    protected override void OnCreate()
    {
        m_ECBSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var deltaTime = Time.DeltaTime;
        var ecb = m_ECBSystem.CreateCommandBuffer().AsParallelWriter();

        //TODO: Replace SomeTempTag with the TileCheckTag
        Entities.WithNone<SomeTempTag>().ForEach(
            (Entity entity, int entityInQueryIndex, ref Position position, ref Translation translation, in Speed speed,
                in Direction direction) =>
            {
                var forward = float2.zero;
                //Convert direction to forward
                if ((direction.Value & North) == 1)
                {
                    forward = new float2(0, 1);
                }
                else if ((direction.Value & South) == 2)
                {
                    forward = new float2(0, -1);
                }
                else if ((direction.Value & East) == 4)
                {
                    forward = new float2(1, 0);
                }
                else if ((direction.Value & West) == 8)
                {
                    forward = new float2(-1, 0);
                }

                var prevTileX = (int) position.Value.x;
                var prevTileY = (int) position.Value.y;

                //Add direction * speed * deltaTime to position
                var deltaX = math.mul(math.mul(forward.x, speed.Value), deltaTime);
                var deltaY = math.mul(math.mul(forward.y, speed.Value), deltaTime);
                position.Value += new float2(deltaX, deltaY);

                if ((int) position.Value.x != prevTileX || (int) position.Value.y != prevTileY)
                {
                    //Add Tile Check Tag
                    ecb.AddComponent<SomeTempTag>(entityInQueryIndex, entity);
                }

                //Set position in ltw
                translation.Value = new float3(position.Value.x, 0, position.Value.y);
            }).ScheduleParallel();

        m_ECBSystem.AddJobHandleForProducer(Dependency);
    }
}