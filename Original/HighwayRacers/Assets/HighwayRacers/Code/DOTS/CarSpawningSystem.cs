using System;
using Unity.Collections;
using Unity.Entities;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public class CarSpawningSystem : ComponentSystem
{
    // Once cars are spawned we add this component to the spawner entity so that we don't get to spawn again.
    [Serializable]
    public struct DeactiveSpawner : IComponentData
    { }

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
        Entities.WithNone<DeactiveSpawner>().ForEach((Entity e, ref CarSpawnProperties spawner, ref HighwayProperties hiway) =>
        {
            var carSpawnCount = hiway.numCars;
            var carsPerLane = (int)Math.Floor((float)carSpawnCount / HighwayConstants.NUM_LANES);
            var random = new System.Random(100);

            var entities = new NativeArray<Entity>(carSpawnCount, Allocator.Temp);
            var carActiveComponentTypes = new ComponentTypes(
                                typeof(CarBasicState),
                                typeof(CarLogicState)
                            );
            var carStaticComponentTypes = new ComponentTypes(
                                typeof(CarReadOnlyProperties),
                                typeof(CarOvertakeStaticProperties)
                            );
            // Instantiate with the prefab and add all the components.
            for (int i = 0; i < carSpawnCount; ++i)
            {
                // Is this really necessary, or should the Drawing System just
                // DrawMeshInstanced using this prefab?
                entities[i] = EntityManager.Instantiate(spawner.carPrefab);
                EntityManager.AddComponents(entities[i],carActiveComponentTypes);
                EntityManager.AddComponents(entities[i],carStaticComponentTypes);
            }

            // Create a whole bunch of cars equally spaced within the road as a starting point.
            float curPosition = 0;
            int curLane = 1;
            float positionStep = computePositionStep(hiway.highwayLength, carsPerLane, curLane);
            for (int i = 0; i < carSpawnCount; ++i)
            {
                CarReadOnlyProperties crop;
                CarOvertakeStaticProperties cosp;

                crop.DefaultSpeed = RandomInRange(ref random, spawner.defaultSpeedMin, spawner.defaultSpeedMax);
                crop.MaxSpeed = RandomInRange(ref random, spawner.overtakeSpeedMin, spawner.overtakeSpeedMax);
                crop.LaneCrossingSpeed = spawner.laneSwitchSpeed;
                crop.MergeDistance = RandomInRange(ref random, spawner.mergeLeftDistanceMin, spawner.mergeLeftDistanceMax);
                crop.MergeSpace = RandomInRange(ref random, spawner.mergeSpaceMin, spawner.mergeSpaceMax);

                cosp.OvertakeMaxTime = spawner.maxOvertakeTime;
                cosp.OvertakeEagerness = RandomInRange(ref random, spawner.overtakeEagernessMin, spawner.overtakeEagernessMax);

                EntityManager.SetComponentData(entities[i], crop);
                EntityManager.SetComponentData(entities[i], cosp);

                CarBasicState cbs;
                CarLogicState cls;

                cbs.Speed = crop.DefaultSpeed;
                cbs.Lane = curLane - 1;
                cbs.Position = curPosition;

                cls.State = VehicleState.NORMAL;
                cls.TargetLane = cbs.Lane;
                cls.TargetSpeed = crop.DefaultSpeed;

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

            }

            EntityManager.AddComponent<DeactiveSpawner>(e);
            // Clean up our entity temporary Entity list.
            entities.Dispose();
        });
    }
}