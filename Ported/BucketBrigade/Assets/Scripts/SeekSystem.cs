using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using Random = UnityEngine.Random;

public class SeekSystem : SystemBase
{
    protected override void OnCreate()
    {
        base.OnCreate();
    }

    protected override void OnUpdate()
    {
        float dt = (float)Time.DeltaTime;
        
        // Due to use of EntityManager this has to run on the main thread.
        // is there a better way? Collect all carrying / non-carrying entities in a separate list on main thread, then process in job?
        Entities.WithoutBurst().ForEach((Entity e, ref Translation t, ref Rotation r, ref SeekPosition follower, in Agent agent) =>
        {
            float agentMaxVelocity = agent.MaxVelocity;
            // move entity from current location

            float3 distance = follower.TargetPos - t.Value;
            distance.y = 0; // ignore heights.

            //if (math.lengthsq(distance) > 0.1f)
            {
                // look at direction
                r.Value = quaternion.LookRotation(math.normalize(distance),new float3(Vector3.up.x, Vector3.up.y, Vector3.up.z));

                // reduce speed when carrying something
                bool isCarrying = agent.CarriedEntity != Entity.Null;
                if (isCarrying)
                {
                    // are we carrying a bucket?
                    if (EntityManager.HasComponent<Bucket>(agent.CarriedEntity))
                    {
                        Intensity bucketWaterValume = EntityManager.GetComponentData<Intensity>(agent.CarriedEntity);
                        agentMaxVelocity *= 0.5f * (bucketWaterValume.Value / 3.0f);
                    }
                    else
                    {
                        // todo! carrying other things?
                        agentMaxVelocity *= 0.1f;
                    }
                }
                
                // seek
                follower.Velocity = agentMaxVelocity;
                t.Value += math.normalize(distance) * follower.Velocity;// * dt;
                
                // update carried entity
                if (isCarrying)
                {
                    Translation carriedObjectPos = EntityManager.GetComponentData<Translation>(agent.CarriedEntity);
                    carriedObjectPos.Value = t.Value + new float3(0.0f, 1.0f, 0.0f);
                }
            }
            

        }).Run();
    }
}
