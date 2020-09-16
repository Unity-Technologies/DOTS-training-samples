using Unity.Entities;
using Unity.Mathematics;
using Debug = UnityEngine.Debug;

[UpdateBefore(typeof(YawToRotationSystem))]
public class SteeringSystem : SystemBase {

    static Random m_Rng = new Random(1337);

    float NextGaussian(float mean, float stdDev) {
        float u1 = 1.0f - m_Rng.NextFloat();
        float u2 = 1.0f - m_Rng.NextFloat();
        float randStdNormal = math.sqrt(-2.0f * math.log(u1))
                            * math.sin(2.0f * math.PI * u2);
        return mean + stdDev * randStdNormal;
    }

    public static void Gather(PheromoneMap map, DynamicBuffer<PheromoneStrength> pheromones, float3 worldPos, ref float[,] values) {
        var gridPos = PheromoneMap.WorldToGridPos(map, worldPos);

        if (gridPos.x > map.Resolution - 2 || gridPos.y > map.Resolution - 2) {
            Debug.LogWarning($"AddScent: gridPos={gridPos.x},{gridPos.y}, worldPos={worldPos.x},{worldPos.z}");
        }
        if (gridPos.x < 2 || gridPos.y < 2) {
            Debug.LogWarning($"AddScent: gridPos={gridPos.x},{gridPos.y}, worldPos={worldPos.x},{worldPos.z}");
        }

        for (int x = -2; x < 3; x++) {
            for (int y = -2; y < 3; y++) {
                if (x == 0 && y == 0) { continue; }

                var sampleGridPos = gridPos + new int2(x, y);
                var index = PheromoneMap.GridPosToIndex(map, sampleGridPos);

                if (index > pheromones.Length || index < 0) {
                    Debug.LogWarning($"AddScent: {index} > {pheromones.Length}, gridPos={gridPos.x},{gridPos.y}, worldPos={worldPos.x},{worldPos.z}");
                    continue;
                }

                values[x + 2, y + 2] = pheromones[index];
            }
        }
    }

    // ----------------------------------------------------------------------------------- //
    // XXX(jcowles): Seems like there is no Mathematics equivalent of Vector2.SignedAngle.
    //               This was ported from Mathf.
    // ----------------------------------------------------------------------------------- //
    public const float kEpsilonNormalSqrt = 1e-15f;
    public static float SqrMagnitude(float2 v) {
        return v.x * v.x + v.y * v.y;
    }
    public static float Angle(float2 from, float2 to) {
        float denominator = math.sqrt(SqrMagnitude(from) * SqrMagnitude(to));
        if (denominator < kEpsilonNormalSqrt) return 0F;
        float dot = math.clamp(math.dot(from, to) / denominator, -1F, 1F);
        return math.degrees(math.acos(dot));
    }
    public static float SignedAngle(float2 from, float2 to) {
        float unsigned_angle = Angle(from, to);
        float sign = math.sign(from.x * to.y - from.y * to.x);
        return unsigned_angle * sign;
    }
    // ----------------------------------------------------------------------------------- //

    float StrongestDirection(PheromoneMap map, DynamicBuffer<PheromoneStrength> pheromones, float3 forward) {
        float avgAngle = 0;
        for (int x = 0; x < 5; x++) {
            for (int y = 0; y < 5; y++) {
                var fw = math.normalize(new float2(forward.x, forward.z));
                var ang = SignedAngle(math.normalize(new float2(x - 1, y - 1)), fw);
                avgAngle += ang * pheromones[PheromoneMap.GridPosToIndex(map, new int2(x, y))];
            }
        }
        return avgAngle;
    }

    // Update is called once per frame
    protected override void OnUpdate()
    {
        float globalTime = (float)Time.ElapsedTime;

        // hack to test smoothing, only step up global time every second
        // Choose random angle targets
        // To be replaced by pheromone brain
        float updateTime = 1.0f;
        globalTime -= (globalTime % updateTime);
        float deltaTime = (float)Time.DeltaTime;

        float smootherFreq = 10.0f;
        float smootherDampingRatio = 1.0f; // < 1.0f underdamped, 1.0f == critically damped, > 1.0f overdamped

        Unity.Mathematics.Random rand = new Random((uint)(globalTime.GetHashCode()));

        // Had some weird results on the first random so I mutate the random a little first
        rand.NextInt();
        rand.NextInt();
        rand.NextInt();


        Entities.WithAll<AntTag>().ForEach((Entity entity, ref Yaw yaw, ref SteeringComponent steeringData) =>
        {

            if (globalTime - steeringData.LastSteerTime > updateTime)
            {
                steeringData.LastSteerTime = globalTime;
                steeringData.DesiredYaw = rand.NextFloat(-math.PI, math.PI);
            }

            // Interp towards desired

            float currentToDesired = steeringData.DesiredYaw - yaw.CurrentYaw;

            // Confine to -pi -- pi
            currentToDesired = ClampToPiMinusPi(currentToDesired);
            float deltaYaw = 0.0f;

            SpringDamp(ref deltaYaw, ref yaw.CurrentYawVel, currentToDesired, smootherDampingRatio, smootherFreq, deltaTime);
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
