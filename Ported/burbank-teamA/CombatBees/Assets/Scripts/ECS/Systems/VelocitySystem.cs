using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Mathematics;
public class VelocitySystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        var dt = Time.deltaTime;
        
        return Entities
            .ForEach((ref Translation translation, ref NonUniformScale s, in Velocity velocity) => { 
                translation.Value += velocity.Value * dt;
                //s.Value = math.max(math.normalize(math.abs(velocity.Value))*2.0f, new float3(0.5f, 0.5f, 0.5f));
                s.Value.x = math.min(math.abs(velocity.Value.x)/15.0f, 2.0f);
            })
            .Schedule(inputDependencies);
    }
}