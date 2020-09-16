using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Debug = UnityEngine.Debug;

[UpdateBefore(typeof(YawToRotationSystem))]
public class SteeringSystem : SystemBase {

    // ----------------------------------------------------------------------------------- //
    // XXX(jcowles): Seems like there is no Mathematics equivalent of Vector2.SignedAngle;
    //               this was ported from Mathf.
    // ----------------------------------------------------------------------------------- //
    private const float kEpsilonNormalSqrt = 1e-15f;
    private static float _SqrMagnitude(float2 v) {
        return v.x * v.x + v.y * v.y;
    }
    private static float _Angle(float2 from, float2 to) {
        float denominator = math.sqrt(_SqrMagnitude(from) * _SqrMagnitude(to));
        if (denominator < kEpsilonNormalSqrt) return 0F;
        float dot = math.clamp(math.dot(from, to) / denominator, -1F, 1F);
        return math.degrees(math.acos(dot));
    }
    private static float _SignedAngle(float2 from, float2 to) {
        float unsigned_angle = _Angle(from, to);
        float sign = math.sign(from.x * to.y - from.y * to.x);
        return unsigned_angle * sign;
    }
    // ----------------------------------------------------------------------------------- //

    static Random m_Rng = new Random(1337);

    static float NextGaussian(float mean, float stdDev) {
        float u1 = 1.0f - m_Rng.NextFloat();
        float u2 = 1.0f - m_Rng.NextFloat();
        float randStdNormal = math.sqrt(-2.0f * math.log(u1))
                            * math.sin(2.0f * math.PI * u2);
        return mean + stdDev * randStdNormal;
    }

    static float StrongestDirection(PheromoneMap map,
                                    DynamicBuffer<PheromoneStrength> pheromones,
                                    float3 neighborhoodCenterWorldPos,
                                    float3 forward) {
        // A radius of 2 implies a 5x5 neighborhood.
        int radius = 2;
        int2 pixelCenter = PheromoneMap.WorldToGridPos(map, neighborhoodCenterWorldPos);
        float avgDeltaAngle = 0;
        float totalStrength = 0;

        for (int x = -radius; x < radius + 1; x++) {
            for (int y = -radius; y < radius + 1; y++) {
                if (x == 0 && y == 0) { continue; }

                var centerOffset = new int2(x, y);

                int index = PheromoneMap.GridPosToIndex(map, pixelCenter + centerOffset);
                float strength = math.max(0, pheromones[index]);

                if (strength < kEpsilonNormalSqrt) { continue; }

                float2 fwNorm = math.normalize(new float2(forward.x, forward.z));
                float2 pxNorm = math.normalize((float2)centerOffset);
                float fwDeltaAngle = _SignedAngle(pxNorm, fwNorm);
                if (math.abs(fwDeltaAngle) > 90) { continue; }

                avgDeltaAngle += fwDeltaAngle * strength;
                totalStrength += strength;
            }
        }

        if (totalStrength < kEpsilonNormalSqrt) {
            // No signal, could return a random value here, but it's probably best handled externally.
            return 0;
        }

        // Normalize by the total signal strength.
        return math.radians(avgDeltaAngle / totalStrength);
    }

    // Update is called once per frame
    protected override void OnUpdate()
    {
        float globalTime = (float)Time.ElapsedTime;

        float deltaTime = (float)Time.DeltaTime;

        var settingsEntity = GetSingletonEntity<SteeringSettings>();
        var settingsData = EntityManager.GetComponentData<SteeringSettings>(settingsEntity);

        float maxAngleDeviation = math.radians(settingsData.PheromoneDeviationDegrees);

        var mapEntity = GetSingletonEntity<PheromoneMap>();
        var map = EntityManager.GetComponentData<PheromoneMap>(mapEntity);
        var pheromones = EntityManager.GetBuffer<PheromoneStrength>(mapEntity);

        Entities.WithAll<AntTag>().ForEach((Entity entity, ref Yaw yaw, ref SteeringComponent steeringData, in LocalToWorld ltw) =>
        {
            // Calculate a desired yaw based on current position
            float strongestPheromoneAngle = StrongestDirection(map, pheromones, ltw.Position, ltw.Forward);
            float gaussianSample = NextGaussian(strongestPheromoneAngle, maxAngleDeviation);

            steeringData.DesiredYaw = gaussianSample;

            // Interp towards desired

            float currentToDesired = steeringData.DesiredYaw - yaw.CurrentYaw;

            // Confine to -pi -- pi
            currentToDesired = ClampToPiMinusPi(currentToDesired);
            float deltaYaw = 0.0f;

            SpringDamp(ref deltaYaw, ref yaw.CurrentYawVel, currentToDesired, settingsData.SteeringDampingRatio, settingsData.SteeringSmoothingFrequency, deltaTime);
            yaw.CurrentYaw += deltaYaw;
            yaw.CurrentYaw = ClampToPiMinusPi(yaw.CurrentYaw);
        }).Run();
    }

    static float ClampToPiMinusPi(float angleRadians)
    {
        float ret = angleRadians;
        while(ret > math.PI)
        {
            ret -= 2.0f*math.PI;
        }

        while(ret < -math.PI)
        {
            ret += 2.0f*math.PI;
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
