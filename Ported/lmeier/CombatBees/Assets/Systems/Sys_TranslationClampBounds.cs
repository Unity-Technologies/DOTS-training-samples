using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using static Unity.Mathematics.math;
using UnityEngine;

public static class Field
{
    public static Vector3 size = new Vector3(100f, 20f, 30f);
}

[UpdateInGroup(typeof(LateSimulationSystemGroup))][UpdateBefore(typeof(Sys_TranslationAddVelocity))]
public class Sys_TranslationClampBounds : JobComponentSystem
{

    [BurstCompile][ExcludeComponent(typeof(Tag_Sticky))]
    struct Sys_TranslationClampBoundsJob : IJobForEach<Translation, C_Velocity>
    {
        [ReadOnly] public float dt;
        [ReadOnly] public float3 fieldBounds;
        [ReadOnly] public float gravity;

        public void Execute(ref Translation translation, ref C_Velocity velocity)
        {

            if (System.Math.Abs(translation.Value.x) > fieldBounds.x * .5f)
            {
                translation.Value.x = (fieldBounds.x * .5f) * Mathf.Sign(translation.Value.x);
                velocity.Value.x *= -.5f;
                velocity.Value.y *= .8f;
                velocity.Value.z *= .8f;
            }
            if (System.Math.Abs(translation.Value.z) > fieldBounds.z * .5f)
            {
                translation.Value.z = (fieldBounds.z * .5f) * Mathf.Sign(translation.Value.z);
                velocity.Value.z *= -.5f;
                velocity.Value.x *= .8f;
                velocity.Value.y *= .8f;
            }

            if (System.Math.Abs(translation.Value.y) > fieldBounds.y * .5f)
            {
                translation.Value.y = (fieldBounds.y * .5f) * Mathf.Sign(translation.Value.y);
                velocity.Value.y *= -.5f;
                velocity.Value.z *= .8f;
                velocity.Value.x *= .8f;
            }
            
        }
    }
    
    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        var job = new Sys_TranslationClampBoundsJob()
        {
            dt = Time.deltaTime,
            fieldBounds = Field.size,
            gravity = GameConstants.S.Gravity
        };

        return job.Schedule(this, inputDependencies);
    }
}