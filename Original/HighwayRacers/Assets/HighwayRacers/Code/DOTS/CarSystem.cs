using Unity.Entities;
using Unity.Jobs;

public class CarSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        // 1. Put cars {Entity, CarPosition} into different lanes
        // 2. Sort the list of cars for each lane => {Entity, index}
        // 3. Car logic {Entity, index, velocity, state }
        // 4. Compose the matrices for each mesh instance for correct rendering.
        return inputDeps;
    }
}
