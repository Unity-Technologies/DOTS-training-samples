using Unity.Entities;
using Unity.Jobs;

namespace Pathfinding
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public class DistanceFieldSystem : JobComponentSystem
    {
        
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            return inputDeps;
        }
    }
}