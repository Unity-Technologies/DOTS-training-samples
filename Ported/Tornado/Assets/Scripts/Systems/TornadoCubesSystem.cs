using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public partial class TornadoCubesSystem : SystemBase
{
    public float spinRate = 37;
    public float upwardSpeed = 6;

    protected override void OnUpdate()
    {
        // Assign values to local variables captured in your job here, so that it has
        // everything it needs to do its work when it runs later.
        // For example,
        //     float deltaTime = Time.DeltaTime;

        // This declares a new kind of job, which is a unit of work to do.
        // The job is declared as an Entities.ForEach with the target components as parameters,
        // meaning it will process all entities in the world that have both
        // Translation and Rotation components. Change it to process the component
        // types you want.

        var tornadoEntity = GetSingletonEntity<Tornado>();
        var tornadoComponent = GetSingleton<Tornado>();
        
        
        
        Entities
            .WithoutBurst()
            .WithStructuralChanges()
            .ForEach((ref Cube tag, ref Translation translation) => {
            // Implement the work to perform for each entity here.
            // You should only access data that is local or that is a
            // field on this job. Note that the 'rotation' parameter is
            // marked as 'in', which means it cannot be modified,
            // but allows this job to run in parallel with other jobs
            // that want to read Rotation component data.
            // For example,
            //     translation.Value += math.mul(rotation.Value, new float3(0, 0, 1)) * deltaTime;
            
            
            float3 tornadoPos = new float3(PointManager.tornadoX+PointManager.TornadoSway(translation.Value.y),translation.Value.y,PointManager.tornadoZ);
            
            float3 delta = (tornadoPos - translation.Value);
            float dist = math.length( delta );
            delta /= dist;
            float inForce = dist - math.clamp(tornadoPos.y / 50f, 0,1)*30f*tag.radius+2f;

            translation.Value +=  new float3(-delta.z*spinRate+delta.x*inForce,upwardSpeed,delta.x*spinRate+delta.z*inForce)*Time.DeltaTime;
            
            if (translation.Value.y>50f) {
                translation.Value = new float3(translation.Value.x,0f,translation.Value.z);
            }

        }).Run();
    }
}
