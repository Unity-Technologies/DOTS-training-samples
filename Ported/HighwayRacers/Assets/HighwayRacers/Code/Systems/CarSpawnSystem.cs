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
    List<Entity> myFreeCar;
    float myTimeAccumulation;
    // Runtime    
    int myCurrentCarNumber;

    protected override void OnCreate()
    {
        RequireForUpdate<ExecuteCarSpawn>();
        RequireForUpdate<Config>();
        myRandom = new Unity.Mathematics.Random(10101);
        myAliveCar = new List<Entity>();
        myFreeCar = new List<Entity>();
        myTimeAccumulation = 0;
        myCurrentCarNumber  = 0;
    }
    
    //public void OnUpdate(ref SystemState state)
    protected override void OnUpdate()
    {
        myTimeAccumulation += World.Time.DeltaTime;
        // We only want to spawn cars in one frame. Disabling the system stops it from updating again after this one time.
        var config = SystemAPI.GetSingleton<Config>();

        var random = new Unity.Mathematics.Random(10101);

        if (myCurrentCarNumber < config.DesiredCarNumber)
        {
            int numberOfCarToSpawn = Mathf.Min(config.DesiredCarNumber - myCurrentCarNumber, config.MaxCarSpawnPerFrame);
            for (int Idx = 0; Idx < numberOfCarToSpawn; ++Idx)
            {
                Entity carEntity;
                if (myFreeCar.Count > 0)
                {
                    carEntity = myFreeCar[0];
                    myFreeCar.RemoveAt(0);
                }
                else
                {
                    carEntity = EntityManager.Instantiate(config.CarPrefab);
                }

                myAliveCar.Add(carEntity);
                float defaultSpeed = myRandom.NextFloat(config.DefaultSpeedMin, config.DefaultSpeedMax);
                float lane = myRandom.NextInt(0, config.NumLanes);

                // Set the new player's transform (a position offset from the obstacle).
                EntityManager.SetComponentData(carEntity, new CarData
                {
                    Distance = myRandom.NextFloat(10),
                    Lane = lane,
                    TEMP_NextLaneChangeCountdown = random.NextFloat(0, 3),
                    Speed = defaultSpeed,
                    DesiredLane = lane,
                    Acceleration = config.Acceleration,
//                    TEMP_NextLaneChangeCountdown = random.NextFloat(0, 3),
                });
                EntityManager.SetComponentData(carEntity, new CarColor());
                EntityManager.SetComponentData(carEntity, new CarParameters
                (
                    defaultSpeed,
                    myRandom.NextFloat(config.OvertakePercentMin, config.OvertakePercentMax),
                    myRandom.NextFloat(config.LeftMergeDistanceMin, config.LeftMergeDistanceMax),
                    myRandom.NextFloat(config.MergeSpaceMin, config.MergeSpaceMax),
                    myRandom.NextFloat(config.OvertakeEagernessMin, config.OvertakeEagernessMax),
                    1.0f,// car lenght ?
                    defaultSpeed,
                    config.Acceleration,
                    defaultSpeed,
                    defaultSpeed * myRandom.NextFloat(config.OvertakePercentMin, config.OvertakePercentMax)
                ));
            }
            myCurrentCarNumber += numberOfCarToSpawn;
        }

        if (myTimeAccumulation > 4.0f && config.AllowedToKillCar)
        { 
            // Car Killer ;p 
            for (int i = 0; i < Mathf.Min(config.MaxCarSpawnPerFrame, myAliveCar.Count); )
            {
                if (myRandom.NextFloat(1.0f) > 0.9f)          // 10 % of chance to get killed
                {
                    myFreeCar.Add(myAliveCar[i]);
                    myAliveCar.RemoveAt(i);
                    myCurrentCarNumber--; // We don't advance i if we just killed a car
                }
                else
                {
                    i++; // advance i
                }
            }           
        }
    }
}