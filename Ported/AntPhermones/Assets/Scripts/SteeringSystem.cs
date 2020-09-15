using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateBefore(typeof(YawToRotationSystem))]
public class SteeringSystem : SystemBase
{
    // Update is called once per frame
    protected override void OnUpdate()
    {
        float globalTime = (float)Time.ElapsedTime;
        float frequency = 10.0f;
        float steerDeviation = math.radians(30.0f);

        Unity.Mathematics.Random rand = new Random(1234);

        Entities.WithAll<AntTag>().ForEach((Entity entity, ref Yaw yaw) =>
        {
            float center = rand.NextFloat(2.0f * math.PI);
            float phase = rand.NextFloat(2.0f * math.PI);
            yaw.Value = center + (steerDeviation * math.sin((globalTime * frequency) + phase));
        }).Run();
    }
}
