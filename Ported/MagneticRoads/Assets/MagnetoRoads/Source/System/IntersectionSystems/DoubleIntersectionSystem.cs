using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
// For traffic that can only go straight through the intersection
// car1: laneOut0 <-- laneIn1
// car0: laneIn0   --> laneOut1
[UpdateAfter(typeof(SimpleIntersectionSystem))]
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
                    else
                    {
                        if (cars[i] != Entity.Null)
                        {
                            var car0Pos = GetComponent<CarPosition>(cars[i]);
                            if (car0Pos.Value < 1)
                            {
                                var carSpeed = GetComponent<CarSpeed>(cars[i]);
                                float newPosition = car0Pos.Value + carSpeed.NormalizedValue * CarSpeed.MAX_SPEED * deltaTime;
                                SetComponent(cars[i], new CarPosition {Value = newPosition});
                                var splineData = GetComponent<Spline>(splines[i]);
                                var eval = BezierUtility.EvaluateBezier(splineData.startPos, splineData.anchor1, splineData.anchor2,
                                    splineData.endPos, newPosition);
                                SetComponent(cars[i], new Translation {Value = eval});
                            }
                            else
                            {
                                // TODO: MBRIAU: Still make that car accelerate but cap the normalized speed to 0.7f while in an intersection (Look at Car.cs)
                                // TODO: MBRIAU: We also need to make the first car of each input lane slowdown since the intersection is busy
                                DynamicBuffer<CarBufferElement> laneOutCars = GetBuffer<CarBufferElement>(laneOuts[i]);
                                laneOutCars.Add(cars[i]);
                                SetComponent(cars[i], new CarPosition {Value = 0});
                                
                                if (i == 0)
                                    doubleIntersection.car0 = Entity.Null;
                                else
                                    doubleIntersection.car1 = Entity.Null;
                            }
                        }
                    }
                }

                laneIns.Dispose();
                laneOuts.Dispose();
                cars.Dispose();

            }).Schedule();
    }
}