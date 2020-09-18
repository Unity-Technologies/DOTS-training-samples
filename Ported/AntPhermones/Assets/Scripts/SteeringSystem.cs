using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Debug = UnityEngine.Debug;
using Unity.Jobs;

using static Unity.Collections.Allocator;
using Unity.Collections;

[UpdateAfter(typeof(BoardInitSystem))]
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

   // static readonly Random m_Rng = new Random(1337);

    static float NextGaussian(float mean, float stdDev, ref Random rand) {
        float u1 = 1.0f - rand.NextFloat();
        float u2 = 1.0f - rand.NextFloat();
        float randStdNormal = math.sqrt(-2.0f * math.log(u1))
                            * math.sin(2.0f * math.PI * u2);
        return mean + stdDev * randStdNormal;
    }

    static float StrongestDirection(PheromoneMap map,
                                    NativeArray<PheromoneStrength> pheromones,
                                    float3 neighborhoodCenterWorldPos,
                                    float3 forward) {
        // A radius of 2 implies a 5x5 neighborhood.
        int radius = 2;
        int2 pixelCenter = PheromoneMap.WorldToGridPos(map, neighborhoodCenterWorldPos);
        float avgDeltaAngle = 0;
        float totalStrength = 0;

        for (int y = -radius; y < radius + 1; y++) {
            for (int x = -radius; x < radius + 1; x++) {
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

    /*static float AlternativePheromoneFollow
        (PheromoneMap map,
            float currentYaw,
            DynamicBuffer<PheromoneStrength> pheromones,
            float3 neighborhoodCenterWorldPos,
            float3 forward,
            ref Random rand
        )
    {
        int radius = 2;
        int2 pixelCenter = PheromoneMap.WorldToGridPos(map, neighborhoodCenterWorldPos);

        float3 pheromoneDir = float3.zero;

        for (int x = -radius; x < radius + 1; x++)
        {
            for (int y = -radius; y < radius + 1; y++)
            {
                if (x == 0 && y == 0) { continue; }

                var centerOffset = new int2(x, y);

                int2 gridPos = pixelCenter + centerOffset;
                int index = PheromoneMap.GridPosToIndex(map, gridPos);
                float strength = math.max(0, pheromones[index]);

                float3 cellWorldPos = PheromoneMap.GridToWorldPos(map, gridPos);

                float3 toCell = cellWorldPos - neighborhoodCenterWorldPos;
                if(math.dot(toCell, forward) < -0.01f)
                {
                    continue;
                }
                toCell = math.normalize(toCell);

                pheromoneDir += strength * toCell;
            }
        }

        if(math.lengthsq(pheromoneDir) < 0.01f)
        {
            return rand.NextFloat(-math.PI, math.PI);
        }

        float angle = math.atan2(pheromoneDir.x, pheromoneDir.z);
        
        return angle - currentYaw;
    }

    private int m_FrameCount;

    private static int CombineHashCode(int code1, int code2)
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 31 + code1;
            hash = hash * 31 + code2;
            return hash;
        }
    }
    */

    // Update is called once per frame
    protected override void OnUpdate()
    {
        float globalTime = (float)Time.ElapsedTime;
        float deltaTime = (float)Time.DeltaTime;

        var settingsEntity = GetSingletonEntity<SteeringSettings>();
        var settingsData = EntityManager.GetComponentData<SteeringSettings>(settingsEntity);

        float maxAngleDeviation = math.radians(settingsData.RandomDeviationDegrees);

        var mapEntity = GetSingletonEntity<PheromoneMap>();
        var map = EntityManager.GetComponentData<PheromoneMap>(mapEntity);
        DynamicBuffer<PheromoneStrength> pheromones = EntityManager.GetBuffer<PheromoneStrength>(mapEntity);
        NativeArray<PheromoneStrength> pheromoneArray = pheromones.AsNativeArray();

        var arcQuery = GetEntityQuery(typeof(Arc));
        var arcArray = arcQuery.ToComponentDataArrayAsync<Arc>(TempJob, out var arcJobHandle);

        // XXX(jcowles): Seems like this isn't necessary.
        Dependency = JobHandle.CombineDependencies(Dependency, arcJobHandle);

        var foodEntity = GetSingletonEntity<FoodTag>();
        float3 foodPos = EntityManager.GetComponentData<Translation>(foodEntity).Value;

        var homeEntity = GetSingletonEntity<HomeTag>();
        float3 homePos = EntityManager.GetComponentData<Translation>(homeEntity).Value;
        
        Dependency = Entities
            .WithName("SteeringSystem")
            .WithDisposeOnCompletion(arcArray)
            .WithAll<AntTag>()
            .ForEach((Entity entity, ref Yaw yaw, ref SteeringComponent steeringData, ref AntTag antData, in LocalToWorld ltw) =>
        {
            var random = new Random((uint)(Hashing.Combine((uint)(ltw.Position.x * 1000), (uint)(ltw.Position.z * 1000))) + 1);

            // Start with gaussian noise
            if (maxAngleDeviation > 0.0f)
            {
                steeringData.DesiredYaw = NextGaussian(yaw.CurrentYaw, maxAngleDeviation, ref random);
            }

            // Pheromone steering
            float strongestPheromoneAngleDiff = StrongestDirection(map, pheromoneArray, ltw.Position, ltw.Forward);
            //float strongestPheromoneAngleDiff = AlternativePheromoneFollow(map, yaw.CurrentYaw, pheromones, ltw.Position, ltw.Forward, ref random);
            steeringData.DesiredYaw += strongestPheromoneAngleDiff * settingsData.PheromoneSteeringStrength;
            
            // goal steering
            float3 goal = antData.HasFood ? homePos : foodPos;
            float goalYaw = 0.0f;
            float3 toGoal = goal - ltw.Position;
            float toGoalDistSq = math.lengthsq(toGoal);
            antData.GoalSeekAmount = 0.0f;

            if (toGoalDistSq < settingsData.GoalSteeringDistanceStart * settingsData.GoalSteeringDistanceStart)
            {
                if (SteerTowardsGoal(arcArray, yaw, ltw, settingsData, goal, out goalYaw))
                {
                    float diff = goalYaw - yaw.CurrentYaw;
                    diff = ClampToPiMinusPi(diff);

                    // Calculate strength
                    float toGoalDist = math.sqrt(toGoalDistSq);
                    float strengthMult = 0.0f;
                    if(settingsData.GoalSteeringDistanceEnd != settingsData.GoalSteeringDistanceStart)
                    {
                        strengthMult = (toGoalDist - settingsData.GoalSteeringDistanceStart) / (settingsData.GoalSteeringDistanceEnd - settingsData.GoalSteeringDistanceStart);
                        strengthMult = math.clamp(strengthMult, 0.0f, 1.0f);
                    }

                    antData.GoalSeekAmount = strengthMult;

                    steeringData.DesiredYaw += (diff) * settingsData.GoalSteeringStrength * strengthMult;
                }
            }

            //steeringData.DesiredYaw = strongestPheromoneAngle;

            // Interp towards desired
            steeringData.DesiredYaw = ClampToPiMinusPi(steeringData.DesiredYaw);

            float currentToDesired = steeringData.DesiredYaw - yaw.CurrentYaw;

            // Confine to -pi -- pi
            currentToDesired = ClampToPiMinusPi(currentToDesired);
            float deltaYaw = 0.0f;

            SpringDamp(ref deltaYaw, ref yaw.CurrentYawVel, currentToDesired, settingsData.SteeringDampingRatio, settingsData.SteeringSmoothingFrequency, deltaTime);
            yaw.CurrentYaw += deltaYaw;
            yaw.CurrentYaw = ClampToPiMinusPi(yaw.CurrentYaw);

        }).ScheduleParallel(Dependency);
    }

    static bool SteerTowardsGoal(in NativeArray<Arc> arcs, in Yaw yaw, in LocalToWorld ltw, in SteeringSettings settings, in float3 goalPos, out float yawToGoalWorld)
    {
        yawToGoalWorld = 0.0f;

        float2 currentPos2, goalPos2;
        currentPos2.x = ltw.Position.x;
        currentPos2.y = ltw.Position.z;
        goalPos2.x = goalPos.x;
        goalPos2.y = goalPos.z;
        float2 circlePos = float2.zero;
        for (int iArc = 0; iArc < arcs.Length; iArc++)
        {
            float2 intersection1, intersection2;
            int numHits = MathHelper.FindLineCircleIntersections(circlePos, arcs[iArc].Radius, currentPos2, goalPos2, out intersection1, out intersection2, out float t1, out float t2);

            if (numHits == 0)
            {
                continue;
            }

            if (t1 >= 0.0f && t1 <= 1.0f)
            {
                // Check hit 1
                float intersectionAngle = math.atan2(intersection1.x, intersection1.y); // I know this looks backwards but its not in our coordinates

                if (CollisionSystem.IsBetween(math.degrees(intersectionAngle), arcs[iArc].StartAngle, arcs[iArc].EndAngle))
                {
                    return false;
                }
            }

            if(t2 >= 0.0f && t2 <= 1.0f)
            {
                // Check hit 2
                float intersectionAngle = math.atan2(intersection2.x, intersection2.y); // I know this looks backwards but its not in our coordinates

                if (CollisionSystem.IsBetween(math.degrees(intersectionAngle), arcs[iArc].StartAngle, arcs[iArc].EndAngle))
                {
                    return false;
                }
            }
        }

        // Calc yaw to target
        float3 antToGoal = goalPos - ltw.Position;
        if (math.length(antToGoal) > 0.0001f)
        {
            yawToGoalWorld = math.atan2(antToGoal.x, antToGoal.z);
        }

        Debug.Assert(!math.isnan(yawToGoalWorld));

        return true;
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

    static float ClampToZeroMinus2Pi(float angleRadians)
    {
        float ret = angleRadians;
        while (ret > 2.0f*math.PI)
        {
            ret -= 2.0f * math.PI;
        }

        while (ret < 0.0f)
        {
            ret += 2.0f * math.PI;
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

    protected override void OnCreate()
    {
        // We must wait for the pheromone map to be initialized
        RequireForUpdate(GetEntityQuery(typeof(PheromoneMap), typeof(PheromoneStrength)));
        RequireForUpdate(GetEntityQuery(typeof(Arc)));
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }
}
