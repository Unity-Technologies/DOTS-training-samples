using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Debug = UnityEngine.Debug;
using Unity.Jobs;

using static Unity.Collections.Allocator;
using Unity.Collections;

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

    static float AlternativePheromoneFollow
        (PheromoneMap map,
            DynamicBuffer<PheromoneStrength> pheromones,
            float3 neighborhoodCenterWorldPos,
            float3 forward
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
            return m_Rng.NextFloat(-math.PI, math.PI);
        }

        float angle = math.atan2(pheromoneDir.z, pheromoneDir.x);
        
        return angle;
    }


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
        var pheromones = EntityManager.GetBuffer<PheromoneStrength>(mapEntity);

        var arcArray = GetEntityQuery(typeof(Arc)).ToComponentDataArrayAsync<Arc>(TempJob, out var arcJobHandle);
        Dependency = JobHandle.CombineDependencies(Dependency, arcJobHandle);

        var foodEntity = GetSingletonEntity<FoodSpawnAuthoring>();
        float3 foodPos = EntityManager.GetComponentData<Translation>(foodEntity).Value;

        var homeEntity = GetSingletonEntity<HomeTag>();
        float3 homePos = EntityManager.GetComponentData<Translation>(homeEntity).Value;

        Entities.WithAll<AntTag>().WithDisposeOnCompletion(arcArray).ForEach((Entity entity, ref Yaw yaw, ref SteeringComponent steeringData, in LocalToWorld ltw, in AntTag antData) =>
        {
            // Pheromone steering
            float strongestPheromoneAngleDiff = StrongestDirection(map, pheromones, ltw.Position, ltw.Forward);
            steeringData.DesiredYaw = yaw.CurrentYaw + (strongestPheromoneAngleDiff * settingsData.PheromoneSteeringStrength);
            
            // goal steering
            float3 goal = antData.HasFood ? homePos : foodPos;
            float goalYaw = 0.0f;
            if(SteerTowardsGoal(arcArray, yaw, ltw, settingsData, goal, out goalYaw))
            {
                float diff = goalYaw - steeringData.DesiredYaw;
                diff = ClampToPiMinusPi(diff);
                steeringData.DesiredYaw += (diff) * settingsData.GoalSteeringStrength;
            }

            // Add some gaussian noise
            if(maxAngleDeviation > 0.0f)
            {
                steeringData.DesiredYaw = NextGaussian(steeringData.DesiredYaw, maxAngleDeviation);
                steeringData.DesiredYaw = ClampToPiMinusPi(steeringData.DesiredYaw);
            }

            //steeringData.DesiredYaw = strongestPheromoneAngle;

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
            int numHits = MathHelper.FindLineCircleIntersections(circlePos, arcs[iArc].Radius, currentPos2, goalPos2, out intersection1, out intersection2);

            if(numHits == 0)
            {
                continue;
            }
            else if(numHits == 2)
            {
                // Process hit 2

                // TODO: deal with arcs
                
            }

            // Todo: deal with arcs
            return false;
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
