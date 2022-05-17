using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using static Unity.Entities.SystemAPI;

[BurstCompile]
partial struct FiremanMovementSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
    }

    public void OnDestroy(ref SystemState state)
    {

    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {

        foreach (var worker in Query<FiremanAspect>())
        {
            //var workerPos = m_LocalToWorldFromEntity[worker.Self].Position;
            //m_TransformFromEntity[worker.Self].LookAt(worker.Destination);
            
            // Do stuff; 
            worker.Update(state.Time.DeltaTime);
        }
    }
}
