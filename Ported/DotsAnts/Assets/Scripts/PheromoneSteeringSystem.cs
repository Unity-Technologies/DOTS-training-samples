using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Unity.Collections;


[UpdateBefore(typeof(AntPositionSystem))]
public class PheromoneSteeringSystem : SystemBase
{
	static Texture2D texture;
    static int mapSize;
    private static AntDefaults defaults;
    
    protected override void OnCreate()
    {
        base.OnCreate();
        defaults = GameObject.Find("Default values").GetComponent<AntDefaults>();
        texture = defaults.pheromoneMap;
        mapSize = defaults.mapSize;
    }
    
    static public int PheromoneIndex(int x, int y, int mapSize)
    {
	    return x + y * mapSize;
    }

    static float PheromoneSteering(float2 position, float facingAngle, float distance, int mapSize, NativeArray<float> pheromoneArray)
	{
		float output = 0;

		for (int i=-1;i<=1;i+=2) {
			float angle = facingAngle + i * Mathf.PI*.25f;
			float testX = position.x + Mathf.Cos(angle) * distance;
			float testY = position.y + Mathf.Sin(angle) * distance;

			if (testX <0 || testY<0 || testX>=mapSize || testY>=mapSize) {

			} 
			else
			{
				int index = PheromoneIndex((int)testX, (int)testY, mapSize);
				float value = pheromoneArray[index];
				output += value*i;
			}
		}
		return Mathf.Sign(output);
	}

    protected override void OnUpdate()
    {
	    int mapSizeTemp = mapSize;
	    
	    NativeArray<float> pheromoneArray = defaults.GetCurrentPheromoneMapBuffer();

		Entities.WithAll<Ant>()
			.ForEach((
			ref SteeringAngle steeringAngle, 
			in Position pos, in DirectionAngle directionAngle) => {

			steeringAngle.value.x = PheromoneSteering(pos.value, directionAngle.value, 3f, mapSizeTemp, pheromoneArray);

		}).ScheduleParallel();
    }
}
