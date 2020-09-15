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

        // hack to test smoothing, only step up global time every second
        float updateTime = 1.0f;
        globalTime -= (globalTime % updateTime);
        float deltaTime = (float)Time.DeltaTime;

        // Variables for choosing random angle targets
        // To be replaced by pheromone brain
        float steerDeviation = math.radians(180.0f);

        float smootherFreq = 10.0f;
        float smootherDampingRatio = 1.0f; // < 1.0f underdamped, 1.0f == critically damped, > 1.0f overdamped

        Unity.Mathematics.Random rand = new Random((uint)(globalTime.GetHashCode()));
        rand.NextFloat();
        rand.NextFloat();
        rand.NextFloat();

        Entities.WithAll<AntTag>().ForEach((Entity entity, ref Yaw yaw, ref SteeringComponent steeringData) =>
        {

            if (globalTime - steeringData.LastSteerTime > updateTime)
            {
                steeringData.LastSteerTime = globalTime;
                steeringData.DesiredYaw = rand.NextFloat(0.0f, 2.0f * math.PI);
            }

            // Interp towards desired
            // Todo deal with -pi to pi

            float currentToDesired = steeringData.DesiredYaw - yaw.CurrentYaw;
            // Confine to -pi -- pi
            currentToDesired = ClampToPiMinusPi(currentToDesired);
            float deltaYaw = 0.0f;

            SpringDamp(ref deltaYaw, ref yaw.CurrentYawVel, currentToDesired, smootherDampingRatio, smootherFreq, deltaTime);
            yaw.CurrentYaw += deltaYaw;
        }).Run();
    }

    static float ClampToPiMinusPi(float angleRadians)
    {
        float ret = angleRadians;
        while(ret > math.PI)
        {
            ret -= math.PI;
        }

        while(ret < -math.PI)
        {
            ret += math.PI;
        }

        return ret;
    }

    // Credit to reddit: https://www.reddit.com/r/gamedev/comments/31vwyg/game_math_precise_control_over_numeric_springing/
    static void SpringDamp
    (
      ref float value, 
      ref float valueVel,
      float targetValue,
      float dampingRatio, 
      float angularFrequency, 
      float deltaTime
    )
    {
        float f = 1.0f + 2.0f * deltaTime * dampingRatio * angularFrequency;
        float oo = angularFrequency * angularFrequency;
        float hoo = deltaTime * oo;
        float hhoo = deltaTime * hoo;
        float detInv = 1.0f / (f + hhoo);
        float detX = f * value + deltaTime * valueVel + hhoo * targetValue;
        float detV = valueVel + hoo * (targetValue - value);
        value = detX * detInv;
        valueVel = detV * detInv;
    }
}
