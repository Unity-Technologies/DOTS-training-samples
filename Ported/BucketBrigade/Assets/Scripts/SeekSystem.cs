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

            if (math.dot(distance, distance) > 0.1f) // square distance (NB - there's a math.lengthsq too, didn't see that earlier)
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
            else
            {
                // arrive
                // otherwise, select new target
                follower.Velocity *= 0.99f;
                follower.TargetPos = new float3(Random.Range(0.0f, 20.0f), follower.TargetPos.y, Random.Range(0.0f, 20.0f));
            }

        }).Run();
    }
    /*
    private bool MoveTowards(float3 _dest, float movementSpeed)
    {
        float3 _POS = new float3(t.position.x, t.position.y, t.position.z);
        bool arrivedX = false;
        bool arrivedZ = false;

        /*
        if (carrying != null)
        {
            if (carrying.bucketFull)
            {
                movementSpeed *= fireSim.waterCarryAffect;
            }
        }
        
        // X POSITION
        if (_POS.x < _dest.x - arriveThreshold)
        {
            _POS.x += movementSpeed;
        }
        else if (_POS.x > _dest.x + arriveThreshold)
        {
            _POS.x -= movementSpeed;
        }
        else
        {
            arrivedX = true;
        }

        // Z POSITION
        if (_POS.z < _dest.z - arriveThreshold)
        {
            _POS.z += movementSpeed;
        }
        else if (_POS.z > _dest.z + arriveThreshold)
        {
            _POS.z -= movementSpeed;
        }
        else
        {
            arrivedZ = true;
        }

        if (arrivedX && arrivedZ)
        {
            return true;
        }
        else
        {
            t.position = _POS;
            return false;
        }
    }*/
}
