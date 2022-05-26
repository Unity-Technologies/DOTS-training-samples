using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Burst;
partial class CarLaneChangeSystem : SystemBase
{
    public static readonly uint TilesPerLane = 100;
    static public int GetMyTile(float carOffset)
    {
        return (int)((carOffset * TilesPerLane) + 0.5f) % (int)TilesPerLane;
    }
    
    protected override void OnUpdate()
    {
        var deltaTime = Time.DeltaTime;
        var random = Random.CreateFromIndex(1234);
        var randomNum = random.NextInt(); //decide if we're all going to go up a lane or down a lane this time

        NativeArray<bool> myNativeArrays = new NativeArray<bool>((int)TilesPerLane*4, Allocator.Persistent);
        
        for (int i = 0; i < TilesPerLane*4; i++)
        {
            myNativeArrays[i] = false;
        }

        Entities
          .WithAll<Car>()
          .ForEach((ChangeLaneCarAspect laneChangingCar) =>
          {
              int myLane = laneChangingCar.CurrentLane;
              float myPosition = laneChangingCar.CurrentDistance;
              int myTiledLocation = GetMyTile(myPosition);
              myTiledLocation += ((myLane) * 100);
              myNativeArrays[myTiledLocation] = true;
          }).Run();
      
      Entities
            .WithAll<Car>()
            .ForEach((ChangeLaneCarAspect laneChangingCar) =>
            {
                if (laneChangingCar.CurrentCarState == 2) //Overtaking
                {
                    //increment our time spent overtaking
                     laneChangingCar.CurrentOvertakeTime += deltaTime;
                     
                     //Pick a new lane to overtake Into, if our Overtake lane is our current lane, we need to choose
                     //a direction
                     if (laneChangingCar.OvertakeLane == laneChangingCar.StartingLane)
                     {
                         int newOvertakeLane = -1;
 
                         if (laneChangingCar.CurrentLane == 1) //Lane 0, you can only go "up"
                         {
                             newOvertakeLane = 2;
                         }
                         else if (laneChangingCar.CurrentLane == 4) //Lane 3, you can only go "down"
                         {
                             newOvertakeLane = 3;
                         }
                         else //You have two lanes available, choose one!
                         {
                             bool moveUpLane = (randomNum % 2 == 0);
                             newOvertakeLane = moveUpLane
                                 ? laneChangingCar.CurrentLane + 1
                                 : laneChangingCar.CurrentLane - 1;
                         }

                         float myPosition = laneChangingCar.CurrentDistance;
                         int myTile = GetMyTile(myPosition);
                         myTile += ((newOvertakeLane - 1) * 100);
                         bool canOvertake = myNativeArrays[myTile];
                         if (canOvertake) laneChangingCar.OvertakeLane = newOvertakeLane;

                     }
                    else //We can infer that if our current lane is not the starting lane, we're now overtaking
                    {
                        //If we've been in overtake lane long enough
                        if (laneChangingCar.CurrentOvertakeTime > laneChangingCar.OvertakeTime)
                        {
                            //use the system to check if we can actually overtake
                            //CanMoveToNewLane(laneChangingCar.StartingLane, laneChangingCar.CurrentDistance, laneChangingCar.CurrentLane)
                            // {
                                //Reset our current overtake time to 0 for the next time we want to overtake
                                laneChangingCar.CurrentOvertakeTime = 0;
                                laneChangingCar.CurrentLane = laneChangingCar.StartingLane;
                            //}

                        }
                    }
                }
            }).ScheduleParallel();

      myNativeArrays.Dispose(Dependency);
    }
}
