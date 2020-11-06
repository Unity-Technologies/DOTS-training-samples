using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
// For traffic that can only go straight through the intersection
// car1: laneOut0 <-- laneIn1
// car0: laneIn0   --> laneOut1
[UpdateAfter(typeof(SimpleIntersectionActiveCarSystem))]
public class DoubleIntersectionSystem : SystemBase
{
    //private EndFixedStepSimulationEntityCommandBufferSystem ecbSystem;

    protected override void OnCreate()
    {
        base.OnCreate();
        //ecbSystem = World.GetExistingSystem<EndFixedStepSimulationEntityCommandBufferSystem>();
    }
    
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;

        Entities
            .WithNone<IntersectionNeedsInit>()
            .ForEach((Entity entity, ref DoubleIntersection doubleIntersection) =>
            {
                NativeArray<Entity> laneIns = new NativeArray<Entity>(2, Allocator.Temp);
                NativeArray<Entity> laneOuts = new NativeArray<Entity>(2, Allocator.Temp);
                NativeArray<Entity> cars = new NativeArray<Entity>(2, Allocator.Temp);
                NativeArray<Entity> splines = new NativeArray<Entity>(2, Allocator.Temp);

                laneIns[0] = doubleIntersection.laneIn0;
                laneIns[1] = doubleIntersection.laneIn1;

                laneOuts[0] = doubleIntersection.laneOut1;
                laneOuts[1] = doubleIntersection.laneOut0;

                cars[0] = doubleIntersection.car0;
                cars[1] = doubleIntersection.car1;

                splines[0] = doubleIntersection.spline0;
                splines[1] = doubleIntersection.spline1;

                for (int i = 0; i < laneIns.Length; i++)
                {
                    Lane lane = GetComponent<Lane>(laneIns[i]);
                    DynamicBuffer<CarBufferElement> laneInCars = GetBuffer<CarBufferElement>(laneIns[i]);
                    Entity reachedEndOfLaneCar = Entity.Null;
                    if (!laneInCars.IsEmpty)
                    {
                        Entity laneCar = laneInCars[0];  //always get first car in lane at intersection
                        CarPosition carPosition = GetComponent<CarPosition>(laneCar);
                        CarSpeed carSpeed = GetComponent<CarSpeed>(laneCar);

                        carSpeed.NormalizedValue += deltaTime * CarSpeed.ACCELERATION;
                        if (carSpeed.NormalizedValue > 1.0f)
                        {
                            carSpeed.NormalizedValue = 1.0f;
                        }

                        float newPosition =
                            carPosition.Value + carSpeed.NormalizedValue * CarSpeed.MAX_SPEED * deltaTime;

                        if (newPosition > lane.Length)
                        {
                            reachedEndOfLaneCar = laneCar;
                            newPosition = lane.Length;
                        }

                        SetComponent(laneCar, new CarPosition {Value = newPosition});
                        SetComponent(laneCar, carSpeed);
                    }

                    if (cars[i] == Entity.Null)
                    {
                        if (reachedEndOfLaneCar != Entity.Null)
                        {
                            laneInCars.RemoveAt(0);  //car has passed through intersection. Remove from buffer
                            if (i == 0)
                                doubleIntersection.car0 = reachedEndOfLaneCar;
                            else
                                doubleIntersection.car1 = reachedEndOfLaneCar;

                            SetComponent(reachedEndOfLaneCar, new CarPosition {Value = 0});
                        }
                    }
                }

                laneIns.Dispose();
                laneOuts.Dispose();
                cars.Dispose();
                splines.Dispose();

            }).Schedule();
    }
    
    
    /*protected override void OnUpdate2()
    {
        float deltaTime = Time.DeltaTime;
        
        //var ecb = ecbSystem.CreateCommandBuffer();
        //var ecbWriter = ecb.AsParallelWriter();
        
        ComponentDataFromEntity<CarPosition> positionAccessor = GetComponentDataFromEntity<CarPosition>(false);
        ComponentDataFromEntity<CarSpeed> speedAccessor = GetComponentDataFromEntity<CarSpeed>(false);

        Entities
            .WithoutBurst()
            .WithNone<IntersectionNeedsInit>()
            .WithNativeDisableContainerSafetyRestriction(positionAccessor)
            .WithNativeDisableContainerSafetyRestriction(speedAccessor)
            .ForEach((Entity entity, ref DoubleIntersection doubleIntersection) =>
            {
                {
                    //DynamicBuffer<CarBufferElement> laneInCarsWrite = ecbWriter.SetBuffer<CarBufferElement>(entityInQueryIndex, doubleIntersection.laneIn0);
                    Lane lane = GetComponent<Lane>(doubleIntersection.laneIn0);
                    DynamicBuffer<CarBufferElement>
                        laneInCars = GetBuffer<CarBufferElement>(doubleIntersection.laneIn0);
                    Entity reachedEndOfLaneCar = Entity.Null;
                    if (!laneInCars.IsEmpty)
                    {
                        Entity laneCar = laneInCars[0]; //always get first car in lane at intersection
                        CarPosition carPosition = positionAccessor[laneCar];
                        CarSpeed carSpeed = speedAccessor[laneCar];

                        carSpeed.NormalizedValue += deltaTime * CarSpeed.ACCELERATION;
                        if (carSpeed.NormalizedValue > 1.0f)
                        {
                            carSpeed.NormalizedValue = 1.0f;
                        }

                        float newPosition =
                            carPosition.Value + carSpeed.NormalizedValue * CarSpeed.MAX_SPEED * deltaTime;

                        if (newPosition > lane.Length)
                        {
                            reachedEndOfLaneCar = laneCar;
                            newPosition = lane.Length;
                        }

                        positionAccessor[laneCar] = new CarPosition {Value = newPosition};
                        speedAccessor[laneCar] = carSpeed;
                        Debug.Log("Lane car new position 0: " + newPosition + " id: " + laneCar);
                    }

                    if (doubleIntersection.car0 == Entity.Null)
                    {
                        if (reachedEndOfLaneCar != Entity.Null)
                        {
                            laneInCars.RemoveAt(0);
                            doubleIntersection.car0 = reachedEndOfLaneCar;
                            positionAccessor[reachedEndOfLaneCar] = new CarPosition {Value = 0};
                        }
                    }
                }

                {
                    Lane lane = GetComponent<Lane>(doubleIntersection.laneIn1);
                    //DynamicBuffer<CarBufferElement> laneInCarsWrite = ecbWriter.SetBuffer<CarBufferElement>(entityInQueryIndex, doubleIntersection.laneIn1);
                    DynamicBuffer<CarBufferElement>
                        laneInCars = GetBuffer<CarBufferElement>(doubleIntersection.laneIn1);
                    Entity reachedEndOfLaneCar = Entity.Null;
                    if (!laneInCars.IsEmpty)
                    {
                        Entity laneCar = laneInCars[0]; //always get first car in lane at intersection
                        CarPosition carPosition = positionAccessor[laneCar];
                        CarSpeed carSpeed = speedAccessor[laneCar];

                        carSpeed.NormalizedValue += deltaTime * CarSpeed.ACCELERATION;
                        if (carSpeed.NormalizedValue > 1.0f)
                        {
                            carSpeed.NormalizedValue = 1.0f;
                        }

                        float newPosition =
                            carPosition.Value + carSpeed.NormalizedValue * CarSpeed.MAX_SPEED * deltaTime;

                        if (newPosition > lane.Length)
                        {
                            reachedEndOfLaneCar = laneCar;
                            newPosition = lane.Length;
                        }

                        positionAccessor[laneCar] = new CarPosition {Value = newPosition};
                        speedAccessor[laneCar] = carSpeed;
                        Debug.Log("Lane car new position 1: " + newPosition + " id: " + laneCar);
                    }

                    if (doubleIntersection.car1 == Entity.Null)
                    {
                        if (reachedEndOfLaneCar != Entity.Null)
                        {
                            laneInCars.RemoveAt(0);
                            doubleIntersection.car1 = reachedEndOfLaneCar;
                            positionAccessor[reachedEndOfLaneCar] = new CarPosition {Value = 0};
                        }
                    }
                }

            }).Run();
        
        //ecbSystem.AddJobHandleForProducer(Dependency);
    }*/
}