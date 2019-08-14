using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;

public class FindAssignNewTargetSystem : JobComponentSystem
{
    private EntityQuery query;
    protected override void OnCreate()
    {
        query = GetEntityQuery(new EntityQueryDesc()
        {
            All = new []{ComponentType.ReadWrite<Translation>(), ComponentType.ReadOnly<MovementComponent>()},
            None = new []{ComponentType.ReadOnly<FindTargetComponent>() }
        });
    }

    struct MoveJob : IJobForEach<Translation, MovementComponent>
    {
        
        public void Execute(ref Translation translation, ref MovementComponent movement)
        {
            //TODO: run this after moveSystem
            //check if reaches the target
            // add find new target component
            
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        //1. get the direction
        //2. move to the position
        var job = new MoveJob
        {
        
        };
        return job.Schedule(query, inputDeps);
    }
}
