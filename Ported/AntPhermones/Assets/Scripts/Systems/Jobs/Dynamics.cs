using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;

public partial struct Spawner : ISystem
{
    JobHandle DoDynamics()
    {
        //foreach (var position in SystemAPI.Query<RefRW<Position>>().WithAll<Ant>())
        //{ }

        return new();
    }

    struct Dynamics : IJobParallelFor
    {
        public void Execute(int index)
        {
            // Here we do the work
            throw new System.NotImplementedException();
        }

    }
}
