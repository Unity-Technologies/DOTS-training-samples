using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateBefore(typeof(AntPositionSystem))]
public class WallSteeringSystem : SystemBase
{
	static private AntDefaults defaults;
    
    protected override void OnCreate()
    {
        base.OnCreate();
        defaults = GameObject.Find("Default values").GetComponent<AntDefaults>();
    }

/*
    Obstacle[] emptyBucket = new Obstacle[0];
	Obstacle[] GetObstacleBucket(Vector2 pos) {
		return GetObstacleBucket(pos.x,pos.y);
	}
	Obstacle[] GetObstacleBucket(float posX, float posY) {
		int x = (int)(posX / mapSize * bucketResolution);
		int y = (int)(posY / mapSize * bucketResolution);
		if (x<0 || y<0 || x>=bucketResolution || y>=bucketResolution) {
			return emptyBucket;
		} else {
			return obstacleBuckets[x,y];
		}
	}
*/

	static int WallSteering(float2 position, float facingAngle, float distance, int mapSize) {
		int output = 0;

		for (int i = -1; i <= 1; i+=2) {
			float angle = facingAngle + i * Mathf.PI*.25f;
			float testX = position.x + Mathf.Cos(angle) * distance;
			float testY = position.y + Mathf.Sin(angle) * distance;

			if (testX < 0 || testY < 0 || testX >= mapSize || testY >= mapSize) {

			} else {
				int value = 0; //GetObstacleBucket(testX,testY).Length;
				if (value > 0) {
					output -= i;
				}
			}
		}
		return output;
	}

    protected override void OnUpdate()
    {
    	//WallSteering(ant,1.5f);
        int mapSize = defaults.mapSize;
        Entities.WithAll<Ant>().ForEach((
	        ref SteeringAngle steeringAngle,
	        in Position position, in DirectionAngle angle) =>
        {
	        steeringAngle.value.y = WallSteering(position.value, angle.value, 1.5f, mapSize);
        }).ScheduleParallel();
    }
}