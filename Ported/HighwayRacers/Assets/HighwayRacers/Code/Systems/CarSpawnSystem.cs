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
<<<<<<< HEAD
        Debug.Log("Did spawn system create!");
        RequireForUpdate<ExecuteCarSpawn>();
        RequireForUpdate<Config>();
        myRandom = new Unity.Mathematics.Random(10101);
        myAliveCar = new List<Entity>();
        myTimeAccumulation = 0;
        myCurrentCarNumber  = 0;
=======
        state.RequireForUpdate<ExecuteCarSpawn>();
        state.RequireForUpdate<Config>();
>>>>>>> 0b61f031b2f7138e590d0c3aed0b94778d50c595
    }
    
    //public void OnUpdate(ref SystemState state)
    protected override void OnUpdate()
    {
<<<<<<< HEAD
        myTimeAccumulation += World.Time.DeltaTime;
        //Debug.Log("Did spawn system update!");
=======
        // We only want to spawn cars in one frame. Disabling the system stops it from updating again after this one time.
        state.Enabled = false;

>>>>>>> 0b61f031b2f7138e590d0c3aed0b94778d50c595
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

<<<<<<< HEAD
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
=======
            float defaultSpeed = random.NextFloat(config.DefaultSpeedMin, config.DefaultSpeedMax);

            state.EntityManager.SetComponentData(car, new Car
            {
                Distance = random.NextFloat(10),
                Lane = random.NextInt(0, config.NumLanes),

                Acceleration = config.Acceleration,
                DesiredSpeed = defaultSpeed,

                Speed = defaultSpeed,
                defaultSpeed = defaultSpeed,
                overtakePercent = random.NextFloat(config.OvertakePercentMin, config.OvertakePercentMax),
                leftMergeDistance = random.NextFloat(config.LeftMergeDistanceMin, config.LeftMergeDistanceMax),
                mergeSpace = random.NextFloat(config.MergeSpaceMin, config.MergeSpaceMax),
                overtakeEagerness = random.NextFloat(config.OvertakeEagernessMin, config.OvertakeEagernessMax),

                Color = float4.zero,
            });
>>>>>>> 0b61f031b2f7138e590d0c3aed0b94778d50c595
        }
    }
}