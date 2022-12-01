using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;
using Unity.Burst;

[BurstCompile]
partial struct TargetSeekingSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {

    }

    public void OnDestroy(ref SystemState state)
    {

    }
    [BurstCompile]
    private bool Intersect(float2 p1, float2 p2, float2 center, float radius)
    {
        //  get the distance between X and Z on the segment
        float2 dp = p2 - p1;

        float a = math.dot(dp, dp);
        float b = 2 * math.dot(dp, p1 - center);
        float c = math.dot(center, center) - 2 * math.dot(center, p1) + math.dot(p1, p1) - radius * radius;
        float bb4ac = b * b - 4 * a * c;
        if (math.abs(a) < float.Epsilon || bb4ac < 0)
        {
            //  line does not intersect
            return false;
        }

        float t1 = (-b + math.sqrt(bb4ac)) / (2 * a);
        float t2 = (-b - math.sqrt(bb4ac)) / (2 * a);

        return (t1 > 0.0f && t1 < 1.0f) || (t2 > 0.0f && t2 < 1.0f);
    }
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        /*
        if (Linecast(ant.position,targetPos)==false) {
				Color color = Color.green;
				float targetAngle = Mathf.Atan2(targetPos.y-ant.position.y,targetPos.x-ant.position.x);
				if (targetAngle - ant.facingAngle > Mathf.PI) {
					ant.facingAngle += Mathf.PI * 2f;
					color = Color.red;
				} else if (targetAngle - ant.facingAngle < -Mathf.PI) {
					ant.facingAngle -= Mathf.PI * 2f;
					color = Color.red;
				} else {
					if (Mathf.Abs(targetAngle-ant.facingAngle)<Mathf.PI*.5f)
					ant.facingAngle += (targetAngle-ant.facingAngle)*goalSteerStrength;
				}

				//Debug.DrawLine(ant.position/mapSize,targetPos/mapSize,color);
			}
        */
        float3 foodPos = new();
        float3 colonyPos = new();
        bool lineOfSight;
        foreach (var food in SystemAPI.Query<TransformAspect>().WithAll<Food>())
        {
            foodPos = food.WorldPosition;
        }
        foreach (var colony in SystemAPI.Query<TransformAspect>().WithAll<Colony>())
        {
            colonyPos = colony.WorldPosition;
        }
        foreach (var ant in SystemAPI.Query<TransformAspect, DirectionAspect, HasResource>().WithAll<Ant>())
        {
            lineOfSight = true;

            float3 targetPos = ant.Item3.Value ? colonyPos : foodPos;

            foreach (var wall in SystemAPI.Query<TransformAspect>().WithAll<Obstacle>())
            {

                if (Intersect(ant.Item1.WorldPosition.xz, targetPos.xz, wall.LocalPosition.xz, 1.0f))
                {
                    lineOfSight = false;
                    // Draw line to the obstacle
                    // Debug.DrawLine(ant.Item1.WorldPosition, new float3(wall.LocalPosition.x, 0, wall.LocalPosition.z), lineOfSight ? Color.green : Color.red);
                }
               
            }
            if(lineOfSight)
            {
                //Debug.Log(math.atan2(foodPos.x - ant.Item1.WorldPosition.x, foodPos.z - ant.Item1.WorldPosition.z));
                ant.Item2.TargetDirection = math.atan2(targetPos.x - ant.Item1.WorldPosition.x, targetPos.z - ant.Item1.WorldPosition.z);
                ant.Item2.TargetDirection = ant.Item2.TargetDirection - ant.Item2.CurrentDirection;
                if (ant.Item2.TargetDirection > math.PI)
                    ant.Item2.TargetDirection -= (float)(math.PI * 2.0f);
                if (ant.Item2.TargetDirection < - math.PI)
                    ant.Item2.TargetDirection += (float)(math.PI * 2.0f);
            } else {
                ant.Item2.TargetDirection = 0;
            }
            Debug.DrawLine(targetPos, new float3(ant.Item1.LocalPosition.x, 0, ant.Item1.LocalPosition.z), lineOfSight ? Color.green : Color.red);
        }

        //foreach (var ant in SystemAPI.Query<TargetDirectionAspect, TransformAspect>().WithAll<Ant>())
        //{
        //    //Checks if there is anything between the ant and the goal
        //    if (!Physics.Linecast(ant.Item2.WorldPosition, new float3(7.4f, 0f, 10f)))
        //    {
        //        float angle = Vector3.Angle(new float3(7.4f, 0f, 10f) - ant.Item2.WorldPosition, Vector3.right);
        //        Debug.Log(angle);
        //        ant.Item1.Direction = angle;
        //    }
        //    
        //    ant.Item1.Direction = 0.3f;
        //}

/*
if (!Physics.Linecast(ant.Item1.WorldPosition, foodPos))
        {
            Debug.Log("Ant Target Found");
            float newAngle = math.atan2(foodPos.y - ant.Item1.WorldPosition.y, foodPos.x - ant.Item1.WorldPosition.x);
            if (newAngle - ant.Item2.CurrentDirection < math.PI)
            {
                ant.Item2.TargetDirection += math.PI * 2f;
            }
            else if (newAngle - ant.Item2.CurrentDirection < -math.PI)
            {
                ant.Item2.TargetDirection -= math.PI * 2f;
            }
            else
            {
                if (math.abs(newAngle - ant.Item2.CurrentDirection) < math.PI * 0.5f)
                {
                    ant.Item2.TargetDirection += (newAngle - ant.Item2.CurrentDirection) * 0.5f;
                }

            }
        }
*/
}
}
