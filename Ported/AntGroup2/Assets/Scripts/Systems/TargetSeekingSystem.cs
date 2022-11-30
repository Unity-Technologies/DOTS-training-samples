using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;

partial struct TargetSeekingSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {

    }

    public void OnDestroy(ref SystemState state)
    {

    }

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
        float3 stepDirection;
        bool lineOfSight;
        foreach (var food in SystemAPI.Query<TransformAspect>().WithAll<Food>())
        {
            foodPos = food.WorldPosition;
        }
        foreach (var ant in SystemAPI.Query<TransformAspect, DirectionAspect>().WithAll<Ant>())
        {
            lineOfSight = true;
            foreach (var wall in SystemAPI.Query<TransformAspect>().WithAll<Obstacle>())
            {
                for(float stepSize = 0.5f; stepSize < 100f; stepSize += 0.5f)
                {
                    
                    stepDirection = new float3(foodPos.x - ant.Item1.WorldPosition.x, 0, foodPos.z - ant.Item1.WorldPosition.z);
                    float3 stepPosition = ant.Item1.WorldPosition + math.normalize(stepDirection) * stepSize;

                    if (stepPosition.x - wall.WorldPosition.x <= 2.0f || stepPosition.z - wall.WorldPosition.z <= 2.0f)
                    {
                        var dx = stepPosition.x - wall.WorldPosition.x;
                        var dy = stepPosition.z - wall.WorldPosition.z;
                        float sqrDist = dx * dx + dy * dy;

                        if (sqrDist < 2f * 2f)
                            lineOfSight = false;
                    }
                }
                
            }
            if(lineOfSight)
            {
                //Debug.Log(math.atan2(foodPos.x - ant.Item1.WorldPosition.x, foodPos.z - ant.Item1.WorldPosition.z));
                ant.Item2.TargetDirection = math.atan2(foodPos.x - ant.Item1.WorldPosition.x, foodPos.z - ant.Item1.WorldPosition.z);
                ant.Item2.TargetDirection = ant.Item2.TargetDirection - ant.Item2.CurrentDirection;
            }
            Debug.DrawLine(foodPos, new float3(ant.Item1.LocalPosition.x, 0, ant.Item1.LocalPosition.z), lineOfSight ? Color.green : Color.red);
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
