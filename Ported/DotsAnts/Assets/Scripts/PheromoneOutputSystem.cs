using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;


//[UpdateBefore(typeof(AntPositionSystem))]
public class PheromoneOutputSystem : SystemBase
{
	//static Texture2D texture;
    //static int mapSize;
    private static AntDefaults defaults;
    
    protected override void OnCreate()
    {
        base.OnCreate();
        defaults = GameObject.Find("Default values").GetComponent<AntDefaults>();
        //texture = defaults.pheromoneMap;
        //mapSize = defaults.mapSize;
    }

    protected override void OnUpdate()
    {
	    int mapSizeTemp = defaults.mapSize;
	    float antSpeed = defaults.antSpeed;
	    float trailAddSpeed = defaults.trailAddSpeed;

	    float deltaTime = Time.fixedDeltaTime;

		Entities.WithAll<Ant>()
			.WithoutBurst()//TODO fix that
			.ForEach((
			in CarryingFood carryingFood,
			in Speed speed,
			in Position pos) => {
				
				float excitement = .3f;
				if (carryingFood.value) {
					excitement = 1f;
				}
				excitement *= speed.value / antSpeed;
				
				//DropPheromones(ant.position,excitement);
				int x = Mathf.FloorToInt(pos.value.x);
				int y = Mathf.FloorToInt(pos.value.y);
				if (x < 0 || y < 0 || x >= mapSizeTemp || y >= mapSizeTemp) {
				}
				else
				{
					float value = (trailAddSpeed * excitement * deltaTime) * (1f - defaults.GetPheromoneAt(x,y));
					defaults.IncPheromoneAtWithClamp(x, y, value, 1f);
				}

			}).ScheduleParallel();
    }
}
