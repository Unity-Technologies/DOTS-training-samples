using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

//[UpdateAfter(typeof(ObstacleSpawnerSystem))]
//[UpdateBefore(typeof(TransformSystemGroup))]

public partial class CarSpawnSystem : SystemBase
{
    Unity.Mathematics.Random myRandom;
    List<Entity> myAliveCar;
    float myTimeAccumulation;
    // Runtime    
    int myCurrentCarNumber;

    protected override void OnCreate()
    {
        Debug.Log("Did spawn system create!");
        RequireForUpdate<ExecuteCarSpawn>();
        RequireForUpdate<Config>();
        myRandom = new Unity.Mathematics.Random(10101);
        myAliveCar = new List<Entity>();
        myTimeAccumulation = 0;
        myCurrentCarNumber  = 0;
    }
    
    //public void OnUpdate(ref SystemState state)
    protected override void OnUpdate()
    {
        myTimeAccumulation += World.Time.DeltaTime;
        //Debug.Log("Did spawn system update!");
        var config = SystemAPI.GetSingleton<Config>();
        if (myCurrentCarNumber < config.DesiredCarNumber)
        {
            int numberOfCarToSpawn = Mathf.Min(config.DesiredCarNumber - myCurrentCarNumber, config.MaxCarSpawnPerFrame);
            for (int Idx = 0; Idx < numberOfCarToSpawn; ++Idx)
            {
                
                var car = EntityManager.Instantiate(config.CarPrefab);
                myAliveCar.Add(car);
                // Set the new player's transform (a position offset from the obstacle).
                EntityManager.SetComponentData(car, new LocalTransform
                {
                    Position = new float3
                    {
                        x = myRandom.NextFloat(10),
                        y = 1,
                        z = myRandom.NextFloat(10),
                    },
                    Scale = 1,  // If we didn't set Scale and Rotation, they would default to zero (which is bad!)
                    Rotation = quaternion.identity
                });

                //var carAuthoring = config.CarPrefab.GetComponent<CarAuthoring>();

                //carAuthoring.Speed = 1.0f;
            }
            myCurrentCarNumber += numberOfCarToSpawn;
        }
        if (myTimeAccumulation > 10.0f)
        { 
            // Car Killer ;p 
            for (int i = 0; i < Mathf.Min(config.MaxCarSpawnPerFrame, myAliveCar.Count); i++)
            {
                if (myRandom.NextFloat(1.0f) > 0.8f)          // 20 % of chance to get killed
                {
                    EntityManager.DestroyEntity(myAliveCar[0]);
                    myAliveCar.RemoveAt(0);
                    myCurrentCarNumber--;
                }
            }
        }
    }
}