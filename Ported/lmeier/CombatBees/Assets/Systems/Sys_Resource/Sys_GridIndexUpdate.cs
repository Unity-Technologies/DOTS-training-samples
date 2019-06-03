using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using static Unity.Mathematics.math;

public class Sys_GridIndexUpdate : JobComponentSystem
{
    [BurstCompile]
    struct Sys_GridIndexUpdateJob : IJobForEach<C_GridIndex, Translation>
    {
        [ReadOnly] public float2 MinGridPos;
        [ReadOnly] public float2 GridSize;
        [ReadOnly] public int2 GridCounts;

        public void Execute(ref C_GridIndex Grid, [ReadOnly] ref Translation Pos)
        {
            int gridX = (int)floor((Pos.Value.x - MinGridPos.x + GridSize.x * 0.5f) / GridSize.x);
            int gridY = (int)floor((Pos.Value.z - MinGridPos.y + GridSize.y * 0.5f) / GridSize.y);

            Grid.x = clamp(gridX, 0, GridCounts.x - 1);
            Grid.y = clamp(gridY, 0, GridCounts.y - 1);
        }
    }

    public EntityQuery m_group;

    protected override void OnCreate()
    {
        m_group = GetEntityQuery(typeof(C_GridIndex), ComponentType.ReadOnly<Translation>());
    }

    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        m_group.SetFilterChanged(typeof(Translation));

        var job = new Sys_GridIndexUpdateJob()
        {
            MinGridPos = ResourceManager.S.MinGridPos,
            GridSize = ResourceManager.S.GridSize,
            GridCounts = ResourceManager.S.GridCounts
        };

        return job.Schedule(m_group, inputDependencies);
    }
}