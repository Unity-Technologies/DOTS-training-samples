using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

	// Is UpdateBEfore in AnPositionSystem.cs better ????
[UpdateAfter(typeof(AntPositionSystem))]
public class FoodSystem : SystemBase
{
	static private AntDefaults defaults;
    
    protected override void OnCreate()
    {
        base.OnCreate();
        defaults = GameObject.Find("Default values").GetComponent<AntDefaults>();
    }

    static float sqrMagnitude(float2 a)
    {
	    return a.x * a.x + a.y * a.y;
    }

    protected override void OnUpdate()
    {
	    float2 colonyPos = GetSingleton<ColonyLocation>().value;
	    float2 foodPos = GetSingleton<FoodLocation>().value;
		    
    	int mapSize = defaults.mapSize;
        Entities.WithAll<Ant>().ForEach((
	        ref CarryingFood carryingFood,
	        ref DirectionAngle angle,
	        in Position position) =>
        {
	        float2 targetPos = new float2(0,0);
	        if (carryingFood.value) {
		        targetPos = colonyPos;
	        }
	        else {
		        targetPos = foodPos;
	        }
	        
	        if ( sqrMagnitude(position.value - targetPos) < 4f * 4f) {
		        carryingFood.value = !carryingFood.value;
		        angle.value += Mathf.PI;
	        }
        }).ScheduleParallel();
    }
}