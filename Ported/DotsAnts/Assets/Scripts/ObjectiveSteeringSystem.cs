using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;
using UnityEngine;
using System;

[UpdateBefore(typeof(AntPositionSystem))]
public class ObjectiveSteeringSystem : SystemBase
{
    private AntDefaults defaults;

    protected override void OnCreate()
    {
        base.OnCreate();
        defaults = GameObject.Find("Default values").GetComponent<AntDefaults>();
    }

    static bool ArePointsVisible(float2 p0, float2 p1, int mapSize, ref NativeArray<float> bakedCollisionMap)
    {  
        int numSteps = (int)math.length(p1 - p0);
        if (numSteps == 0) { return true; }

        for (int i = 0; i < numSteps; i++)
        {
            float2 pi = p0 + (p1 - p0) * ((float)i / (float)(numSteps - 1));
            int x = (int)pi.x, y = (int)pi.y;
            if (bakedCollisionMap[y * mapSize + x] > 0.5f)
            {
                return false;
            }
        }
        return true;
    }

    protected override void OnUpdate()
    {
        int mapSize = defaults.mapSize;
        float2 colonyPos = GetSingleton<ColonyLocation>().value;
        float2 foodPos = GetSingleton<FoodLocation>().value;
        NativeArray<float> bakedCollisionMap = defaults.colisionMap.GetRawTextureData<float>();

        Entities.WithAll<Ant>().ForEach((
            ref SteeringAngle steeringAngle,
            in Position antPos, in DirectionAngle directionAngle, in CarryingFood hasFood) =>
        {
            float2 objectivePos = hasFood.value ? colonyPos : foodPos;
            if(ArePointsVisible(antPos.value, objectivePos, mapSize, ref bakedCollisionMap))
            {
                // If we can see the objective from the ant's position, steer straight towards it.
                float2 relObjectivePos = antPos.value - objectivePos;
                steeringAngle.value.z =  (float)(Math.Atan2(relObjectivePos.y, relObjectivePos.x) + Math.PI - directionAngle.value);
            }
            else
            {
                // Otherwise, do nothing.
                steeringAngle.value.z = 0.0f;
            }

        }).ScheduleParallel();
    }
}
