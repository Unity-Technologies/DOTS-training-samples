using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;
using UnityEngine;

[UpdateBefore(typeof(AntPositionSystem))]
public class WallSteeringSystem : SystemBase
{
	private AntDefaults defaults;
    
    protected override void OnCreate()
    {
        base.OnCreate();
        defaults = GameObject.Find("Default values").GetComponent<AntDefaults>();
    }

    static int WallSteering(float2 position, float facingAngle, float distance, int mapSize, NativeArray<float> bakedCollisionMap) {
		int output = 0;

		for (int i = -1; i <= 1; i+=2) {
			float angle = facingAngle + i * Mathf.PI*.25f;
			int testX = (int)(position.x + Mathf.Cos(angle) * distance);
			int testY = (int)(position.y + Mathf.Sin(angle) * distance);

            if (testX >= 0 && testY >= 0 && testX < mapSize && testY < mapSize)
            {
                if (bakedCollisionMap[testY * mapSize + testX] > 0.5f)
                {
                    output -= i;
                }
            }
        }
		return output;
	}

    protected override void OnUpdate()
    {
        //WallSteering(ant,1.5f);
        float distance = 1.5f;
        int mapSize = defaults.mapSize;
        NativeArray<float> bakedCollisionMap = defaults.colisionMap.GetRawTextureData<float>();

        Entities.WithAll<Ant>().ForEach((
	        ref SteeringAngle steeringAngle,
	        in Position position, in DirectionAngle angle) =>
        {
	        steeringAngle.value.y = WallSteering(position.value, angle.value, distance, mapSize, bakedCollisionMap);
        }).ScheduleParallel();
    }
}
