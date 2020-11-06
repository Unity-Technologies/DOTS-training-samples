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
        
        ComponentDataFromEntity<CarPosition> positionAccessor = GetComponentDataFromEntity<CarPosition>(false);
        ComponentDataFromEntity<CarSpeed> speedAccessor = GetComponentDataFromEntity<CarSpeed>(false);

        Entities
            .WithNone<IntersectionNeedsInit>()
            .WithNativeDisableContainerSafetyRestriction(positionAccessor)
            .WithNativeDisableContainerSafetyRestriction(speedAccessor)
            .ForEach((Entity entity, ref DoubleIntersection doubleIntersection) =>
            {
                {
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

            }).Schedule();
    }
}