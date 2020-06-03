using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace DefaultNamespace
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateBefore(typeof(Movement))]
    public class Thrower : SystemBase
    {
        protected override void OnCreate()
        {
        }

        protected override void OnUpdate()
        {
            var fireGridEntity = GetSingletonEntity<FireGrid>();
            var fireGridBuffer = GetBufferFromEntity<FireGridCell>();
            
            var config = GetSingleton<BucketBrigadeConfig>();
            var chainComponent = GetComponentDataFromEntity<Chain>();

            // TODO: we are not sure this is required.
            //Dependency = JobHandle.CombineDependencies(Dependency, World.DefaultGameObjectInjectionWorld.GetExistingSystem<Scooper>().MyLastDependency);
            
            Entities.ForEach((ref ThrowerState state, ref TargetPosition agentTargetPosition, ref TargetFire targetFire, in Translation agentPosition, in Agent agent)
                =>
            {
                switch (state.State)
                {
                    case EThrowerState.FindFire:
                        var fireGrid = fireGridBuffer[fireGridEntity];
                        if (TryFindNearestCellOnFire(config, fireGrid, agentPosition, out var nearestFirePosition, out var nearestFireCell))
                        {
                            // Set the target cell.
                            targetFire.GridIndex = nearestFireCell;
                            
                            // Set the end of the chain.
                            var chain = chainComponent[agent.MyChain];
                            chain.ChainEndPosition = nearestFirePosition;
                            chainComponent[agent.MyChain] = chain;
                            
                            // Set the agents target position
                            agentTargetPosition.Target = nearestFirePosition;
                        }
                        break;
                }
            }).WithReadOnly(fireGridBuffer).WithNativeDisableParallelForRestriction(chainComponent).ScheduleParallel();
        }

        private static bool TryFindNearestCellOnFire(BucketBrigadeConfig config, in DynamicBuffer<FireGridCell> fireGrid, Translation agentPosition, out float3 closestPosition, out int2 closestGridPosition)
        {
            float closestPositionSq = float.MaxValue;
            closestPosition = default;
            closestGridPosition = default;
            bool found = false;
            for (int i = 0; i < fireGrid.Length; ++i)
            {
                var cell = fireGrid[i];
                if (cell.Temperature > config.Flashpoint)
                {
                    int cellRowIndex = i / config.GridDimensions.x;
                    int cellColumnIndex = i % config.GridDimensions.x;
                    var fireGridIndex = math.int2(cellRowIndex, cellColumnIndex);
                    var fireGridPosition = FireGridInit.CellToWorldSpace(fireGridIndex, config);
                    var distanceSq = math.distancesq(fireGridPosition, agentPosition.Value);
                    if (distanceSq < closestPositionSq)
                    {
                        closestPosition = fireGridPosition;
                        closestGridPosition = fireGridIndex;
                        found = true;
                    }
                }
            }
            return found;
        }
    }
}