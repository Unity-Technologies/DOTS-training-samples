using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;


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

    static float PheromoneSteering(float2 position, float facingAngle, float distance, int mapSize)
	{
		float output = 0;

		for (int i=-1;i<=1;i+=2) {
			float angle = facingAngle + i * Mathf.PI*.25f;
			float testX = position.x + Mathf.Cos(angle) * distance;
			float testY = position.y + Mathf.Sin(angle) * distance;

			if (testX <0 || testY<0 || testX>=mapSize || testY>=mapSize) {

			} 
			else {
				float value = defaults.GetPheromoneAt((int) testX, (int) testY);
				output += value*i;
			}
		}
		return Mathf.Sign(output);
	}

    protected override void OnUpdate()
    {
	    int mapSizeTemp = mapSize;

		Entities.WithAll<Ant>()
			.WithoutBurst()
			.ForEach((
			ref SteeringAngle steeringAngle, 
			in Position pos, in DirectionAngle directionAngle) => {

			steeringAngle.value.x = PheromoneSteering(pos.value, directionAngle.value, 3f, mapSizeTemp);

		}).Run(); // TODO: Change to ScheduleParallel
    }
}
