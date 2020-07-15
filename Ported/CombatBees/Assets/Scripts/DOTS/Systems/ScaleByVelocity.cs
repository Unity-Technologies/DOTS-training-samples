using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class ScaleByVelocity : SystemBase
{
    protected override void OnUpdate()
    {
        // float size = bees[i].size;
        // Vector3 scale = new Vector3(size,size,size);
        // if (bees[i].dead == false) {
        // 	float stretch = Mathf.Max(1f,bees[i].velocity.magnitude * speedStretch);
        // 	scale.z *= stretch;
        // 	scale.x /= (stretch-1f)/5f+1f;
        // 	scale.y /= (stretch-1f)/5f+1f;
        // }

        float speedStretch = BeeManager.Instance.speedStretch;

        Entities.ForEach((ref NonUniformScale scale, in Size size, in Velocity velocity) =>
        {
            float stretch = math.max(1.0f, math.length(velocity.Value) * speedStretch);
            scale.Value = new float3(size.Value);
            scale.Value.z *= stretch;
            scale.Value.x /= (stretch - 1f) / 5f + 1f;
            scale.Value.y /= (stretch - 1f) / 5f + 1f;
        }).ScheduleParallel();
    }
}
