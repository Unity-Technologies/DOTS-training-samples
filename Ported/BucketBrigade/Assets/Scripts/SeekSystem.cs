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

        Entities.ForEach((Entity e, ref Translation t, ref Rotation r, ref SeekPosition follower) =>
        {
            // move entity from current location

            float3 distance = follower.TargetPos - t.Value;
            distance.y = 0; // ignore heights.

            if (math.dot(distance, distance) > 1.0f) // square distance (NB - there's a math.lengthsq too, didn't see that earlier)
            {
                // look at direction
                quaternion lookDir = quaternion.LookRotation(math.normalize(distance),
                    new float3(Vector3.up.x, Vector3.up.y, Vector3.up.z));

                r.Value = lookDir;//math.normalize(math.mul(r.Value, lookDir));
                //r.Value = new quaternion(0,elapsedTime, 0, 1);

                // seek.
                t.Value += math.normalize(distance) * follower.MaxVelocity * 5.0f * dt;
            }
            else
            {
                //follower.TargetPos = new float3(Random.Range(0.0f, 20.0f), follower.TargetPos.y, Random.Range(0.0f, 20.0f));
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
