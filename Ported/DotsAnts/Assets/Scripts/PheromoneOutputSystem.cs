using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Unity.Collections;


//[UpdateBefore(typeof(AntPositionSystem))]
public class PheromoneOutputSystem : SystemBase
{
	//static Texture2D texture;
    //static int mapSize;
    private static AntDefaults defaults;
    
    static public int PheromoneIndex(int x, int y, int mapSize)
    {
	    return x + y * mapSize;
    }
    
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
	    
	    NativeArray<float> pheromoneArray = defaults.GetCurrentPheromoneMapBuffer();

		Entities.WithAll<Ant>()
			.WithoutBurst()
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
					int index = PheromoneIndex(x, y, mapSizeTemp);
					float value = (trailAddSpeed * excitement * deltaTime) * (1f - pheromoneArray[index]);
					pheromoneArray[index] = value + pheromoneArray[index];
					if (pheromoneArray[index] > 1f)
						pheromoneArray[index] = 1f;
					
				}

			}).Run();
    }
}
