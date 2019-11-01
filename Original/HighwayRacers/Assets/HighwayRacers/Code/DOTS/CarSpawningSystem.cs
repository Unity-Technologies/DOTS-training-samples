using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

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
        float maxLength = Utilities.LaneLength(laneNum, roadLen);
        float lerpFac = (float)(laneNum-1) / (float)(HighwayConstants.NUM_LANES-1); 
        return (roadLen + (maxLength - roadLen) * lerpFac) / (carsPerLane + HighwayConstants.NUM_LANES - 1);
    }

    protected override void OnUpdate()
    {
        Entities.WithNone<DeactiveSpawner>().ForEach((Entity e, ref CarSpawnProperties spawner, ref HighwayProperties hiway) =>
        {
            var carSpawnCount = hiway.numCars;
            var carsPerLane = (int)Math.Floor((float)carSpawnCount / HighwayConstants.NUM_LANES);
            carsPerLane = Math.Max(carsPerLane, 2);
            var random = new System.Random(100);

            var entities = new NativeArray<Entity>(carSpawnCount, Allocator.Temp);
            // No more prefab (using the drawing system) means we need to add LocalToWorld
            // ourselves.
            var carArchetype = EntityManager.CreateArchetype(
                                typeof(CarBasicState),
                                typeof(CarLogicState),
                                typeof(CarColor),
                                typeof(LocalToWorld),
                                typeof(CarReadOnlyProperties)
                                );
            EntityManager.CreateEntity(carArchetype, entities);

            // Create a whole bunch of cars equally spaced within the road as a starting point.
            float curPosition = 0;
            int curLane = 1;
            float positionStep = computePositionStep(hiway.highwayLength, carsPerLane, curLane);
            for (int i = 0; i < carSpawnCount; ++i)
            {
                CarReadOnlyProperties crop;

                crop.DefaultSpeed = RandomInRange(ref random, spawner.defaultSpeedMin, spawner.defaultSpeedMax);
                crop.MaxSpeed = RandomInRange(ref random, spawner.overtakeSpeedMin, spawner.overtakeSpeedMax);
                crop.MergeDistance = RandomInRange(ref random, spawner.mergeLeftDistanceMin, spawner.mergeLeftDistanceMax);
                crop.MergeSpace = RandomInRange(ref random, spawner.mergeSpaceMin, spawner.mergeSpaceMax);
                crop.OvertakeEagerness = RandomInRange(ref random, spawner.overtakeEagernessMin, spawner.overtakeEagernessMax);

                EntityManager.SetComponentData(entities[i], crop);

                CarBasicState cbs;
                CarLogicState cls;
                CarColor col;

                cbs.Speed = crop.DefaultSpeed;
                cbs.Lane = curLane - 1;
                cbs.Position = curPosition;

                col.Color = new float4(spawner.defaultColor.r,
                                       spawner.defaultColor.g,
                                       spawner.defaultColor.b,
                                       spawner.defaultColor.a);

                cls.State = VehicleState.NORMAL;
                cls.TargetLane = cbs.Lane;
                cls.OvertakingCarIndex = -1;
                cls.OvertakeRemainingTime = 0.0f;

                EntityManager.SetComponentData(entities[i], cls);
                EntityManager.SetComponentData(entities[i], cbs);
                EntityManager.SetComponentData(entities[i], col);

                LocalToWorld ltw;
                ltw.Value = float4x4.identity;

                EntityManager.SetComponentData<LocalToWorld>(entities[i], ltw);

                // If we've filled up this lane, move to the next one.
                if (((i + 1) % carsPerLane) == 0 && (curLane < 4))
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