using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
partial struct BeeBehaviourSystem : ISystem
{
    float aggressiveThreshold;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        aggressiveThreshold = 0.6f; // Some hard-coded value. If the bee's scale is above it, the bee will be aggressive and attack.

        foreach (var (transform, entity) in SystemAPI.Query<TransformAspect>().WithAll<BeeState>().WithEntityAccess())
        {
            EntityManager em = state.EntityManager;
            if(transform.WorldScale > aggressiveThreshold)
            {
                em.SetComponentData(entity, new BeeState { beeState = BeeStateEnumerator.Attacking });
            }
            else
            {
                em.SetComponentData(entity, new BeeState { beeState = BeeStateEnumerator.Gathering });
            }
        }
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        /*EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.TempJob);
        EntityCommandBuffer.ParallelWriter parallelEcb = ecb.AsParallelWriter();*/
        //parallelEcb.set
        
        
        // The amount of rotation around Y required to do 360 degrees in 2 seconds.
        var rotation = quaternion.RotateY(SystemAPI.Time.DeltaTime * math.PI);





        // The classic C# foreach is what we often refer to as "Idiomatic foreach" (IFE).
        // Aspects provide a higher level interface than directly accessing component data.
        // Using IFE with aspects is a powerful and expressive way of writing main thread code.
        foreach (var (transform, entity) in SystemAPI.Query<TransformAspect>().WithAll<BeeState>().WithEntityAccess())
        {
            EntityManager em = state.EntityManager;
            //em.SetComponentData(entity, new BeeState { beeState = (BeeStateEnumerator)UnityEngine.Random.Range(0, 3) });

            transform.RotateWorld(rotation);
            //parallelEcb.SetComponent<BeeState>(entity, new BeeState { beeState = (BeeStateEnumerator)UnityEngine.Random.Range(0, 3) };)
        }
    }
}