using System;
using Unity.Collections;
using Unity.Entities;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public class CarSpawningSystem : ComponentSystem
{
    private float RandomInRange(ref System.Random rand, float min, float max)
    {
        return (float)(min + rand.NextDouble() * (max - min));
    }

    private float computePositionStep(float roadLen, int carsPerLane, int laneNum)
    {
        // Compute the easiest possible spacing of the cars assuming the maximum number of cars in a given lane
        return (roadLen - (HighwayConstants.LANE_SPACING * (HighwayConstants.NUM_LANES / 2 - laneNum))) / (carsPerLane + HighwayConstants.NUM_LANES - 1);
    }

    protected override void OnUpdate()
    {
        Entities.ForEach((Entity e, ref CarSpawnProperties spawner, ref HighwayProperties hiway) =>
        {
            var carSpawnCount = hiway.numCars;
            var carsPerLane = (int)Math.Floor((float)carSpawnCount / HighwayConstants.NUM_LANES);
            var random = new System.Random(100);

            var entities = new NativeArray<Entity>(carSpawnCount, Allocator.Temp);
            var carArchetype = EntityManager.CreateArchetype(
                                typeof(CarBasicState),
                                typeof(CarCrossVelocity),
                                typeof(CarLogicState),
                                typeof(CarVelocityStaticProperties),
                                typeof(CarMergeStaticProperties),
                                typeof(CarOvertakeStaticProperties)
                            );
            EntityManager.CreateEntity(carArchetype, entities);

            // Create a whole bunch of cars equally spaced within the road as a starting point.
            float curPosition = 0;
            int curLane = 1;
            float positionStep = computePositionStep(hiway.highwayLength, carsPerLane, curLane);
            for (int i = 0; i < carSpawnCount; ++i)
            {
                CarVelocityStaticProperties cvsp;
                CarOvertakeStaticProperties cosp;
                CarMergeStaticProperties cmsp;

                cvsp.DefaultSpeed = RandomInRange(ref random, spawner.defaultSpeedMin, spawner.defaultSpeedMax);
                cvsp.MaxSpeed = RandomInRange(ref random, spawner.overtakeSpeedMin, spawner.overtakeSpeedMax);
                cvsp.Acceleration = spawner.acceleration;
                cvsp.BrakeDecel = spawner.brakeDeceleration;
                cvsp.LaneCrossingSpeed = spawner.laneSwitchSpeed;

                cosp.OvertakeMaxTime = spawner.maxOvertakeTime;
                cosp.OvertakeEagerness = RandomInRange(ref random, spawner.overtakeEagernessMin, spawner.overtakeEagernessMax);

                cmsp.MergeDistance = RandomInRange(ref random, spawner.mergeLeftDistanceMin, spawner.mergeLeftDistanceMax);
                cmsp.MergeSpace = RandomInRange(ref random, spawner.mergeSpaceMin, spawner.mergeSpaceMax);

                EntityManager.SetComponentData(entities[i], cvsp);
                EntityManager.SetComponentData(entities[i], cosp);
                EntityManager.SetComponentData(entities[i], cmsp);

                CarBasicState cbs;
                CarLogicState cls;
                CarCrossVelocity ccv;

                cbs.Speed = 0;
                cbs.Lane = curLane - 1;
                cbs.Position = curPosition;

                cls.state = VehicleState.NORMAL;
                cls.targetLane = curLane - 1;
                cls.targetSpeed = cvsp.DefaultSpeed;

                ccv.CrossLaneVel = 0;

                EntityManager.SetComponentData(entities[i], ccv);
                EntityManager.SetComponentData(entities[i], cls);
                EntityManager.SetComponentData(entities[i], cbs);

                // If we've filled up this lane, move to the next one.
                if (((i + 1) % carsPerLane) == 0 && (curLane != 4))
                {
                    curPosition = 0;
                    ++curLane;
                    positionStep = computePositionStep(hiway.highwayLength, carsPerLane, curLane);
                }
                else
                {
                    curPosition += positionStep;
                }

                // Is this really necessary, or should the Drawing System just
                // DrawMeshInstanced using this prefab?
                EntityManager.Instantiate(spawner.carPrefab);
            }

            // Remove the spawner component so that we never update this system
            // a second time, and then clean up our entity temporary Entity list.
            EntityManager.RemoveComponent<CarSpawnProperties>(e);
            entities.Dispose();
        });
    }
}