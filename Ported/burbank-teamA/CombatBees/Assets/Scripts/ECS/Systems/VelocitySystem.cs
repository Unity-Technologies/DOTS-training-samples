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
            .ForEach((ref Translation translation,/*ref NonUniformScale s,*/ in Velocity velocity) => { 
                translation.Value += velocity.Value * dt;
                /*s.Value = math.normalize(velocity.Value);*/
            })
            .Schedule(inputDependencies);
    }
}