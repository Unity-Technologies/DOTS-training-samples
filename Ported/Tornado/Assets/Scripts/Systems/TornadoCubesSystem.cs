using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial class TornadoCubesSystem : SystemBase
{
    public float spinRate = 37;
    public float upwardSpeed = 6;

    private float internalTime = 0.0f;
    public float TornadoSway(float y)
    {
        return math.sin(y / 5f + internalTime / 4f) * 3f;
    }
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

        internalTime += Time.DeltaTime;

        tornadoComponent.tornadoX = math.cos(internalTime / 6f) * 30f;
        tornadoComponent.tornadoZ = math.sin(internalTime / 6f * 1.618f) * 30f;

        Entities
            .ForEach((ref Tornado tornado, ref Translation translation) =>
            {
                tornado.tornadoX = tornadoComponent.tornadoX;
                tornado.tornadoZ = tornadoComponent.tornadoZ;

            }).Run();


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
            
            
            float3 tornadoPos = new float3(tornadoComponent.tornadoX+TornadoSway(translation.Value.y),translation.Value.y, tornadoComponent.tornadoZ);
            
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
