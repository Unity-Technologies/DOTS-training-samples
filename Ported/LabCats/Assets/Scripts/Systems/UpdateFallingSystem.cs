using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;

public class UpdateFallingSystem : SystemBase
{
    const float kFallingSpeed = 6.0f;
    const float kTimeToDespawn = 3.0f;

    private EntityCommandBufferSystem m_EcbSystem;
    protected override void OnCreate()
    {
        RequireSingletonForUpdate<GameStartedTag>();
        m_EcbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var ecb = m_EcbSystem.CreateCommandBuffer().AsParallelWriter();

        var board = GetSingletonEntity<BoardDefinition>();
        var boardDefinition = GetComponent<BoardDefinition>(board);
        var cellSize = boardDefinition.CellSize;
        var firstCellPosition = GetComponent<FirstCellPosition>(board);
        var deltaTime = Time.DeltaTime;

        Entities
            .WithName("UpdateFallingObjectPosition")
            .WithAll<Speed>()
            .ForEach((Entity entity, int entityInQueryIndex, ref Translation translation, ref FallingTime fallingTime, in GridPosition gridPosition) =>
            {
                fallingTime.Value += deltaTime;

                if (fallingTime.Value > kTimeToDespawn)
                {
                    ecb.DestroyEntity(entityInQueryIndex, entity);
                }

                var xOffset = gridPosition.X * cellSize;
                var yOffset = gridPosition.Y * cellSize;
                translation.Value = firstCellPosition.Value + new float3(yOffset, 0.5f - fallingTime.Value*kFallingSpeed, xOffset);
            }).ScheduleParallel();
    }
}
