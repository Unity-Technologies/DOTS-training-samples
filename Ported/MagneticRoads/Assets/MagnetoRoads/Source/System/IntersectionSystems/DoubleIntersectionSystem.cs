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
}