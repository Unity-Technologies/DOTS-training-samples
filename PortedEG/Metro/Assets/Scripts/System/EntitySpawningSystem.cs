using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine.Rendering;
using Unity.Burst;

// TODO: Better name and struct
[BurstCompile]
public struct CommuterUnmanagedInfo
{
    public int StartPlatform;
    public int FinalPlatform;

    public float3 InitPosition;
}

public struct PlatformInitInfo
{
    public float3 Position;
    public quaternion Rotation;
}

public struct TaskIndexRange
{
    public int StartIndex;
    public int EndIndex;
}

public partial class EntitySpawningSystem : SystemBase
{
    private bool IsSpawningDone = false;

    EntityQuery m_BaseColorQuery;

    protected override void OnCreate()
    {
        m_BaseColorQuery = GetEntityQuery(ComponentType.ReadOnly<URPMaterialPropertyBaseColor>());
    }

    [BurstCompile]
    public void Spawn()
    {
        if (IsSpawningDone)
            return;

        Metro metro = Metro.INSTANCE;

        var config = SystemAPI.GetSingleton<Config>();

        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.TempJob);

        // Walkways
        Entity PlatformWalkwaysEntity;
        NativeArray<WalkwayInfo> platformWalkwayInfos = new NativeArray<WalkwayInfo>(metro.allPlatforms.Length, Allocator.TempJob);
        {
            int objCount = metro.allPlatforms.Length;

            for (int i = 0; i < objCount; ++i)
            {
                var frontWalkway = metro.allPlatforms[i].walkway_FRONT_CROSS;
                var backWalkway  = metro.allPlatforms[i].walkway_BACK_CROSS;

                platformWalkwayInfos[i] = new WalkwayInfo
                {
                    fPlatform_Connects_FROM = frontWalkway.connects_FROM.globalPlatformIndex,
                    fPlatform_Connects_TO   = frontWalkway.connects_TO.globalPlatformIndex,

                    bPlatform_Connects_FROM = backWalkway.connects_FROM.globalPlatformIndex,
                    bPlatform_Connects_TO   = backWalkway.connects_TO.globalPlatformIndex,

                    fPos = frontWalkway.transform.position,
                    bPos = backWalkway.transform.position,

                    fnav_START = frontWalkway.nav_START.position,
                    fnav_END   = frontWalkway.nav_END.position,

                    bnav_START = backWalkway.nav_START.position,
                    bnav_END   = backWalkway.nav_END.position
                };
            }
             
            PlatformWalkwaysEntity = EntityManager.CreateEntity();
            EntityManager.AddComponent<PlatformWalkways>(PlatformWalkwaysEntity);
            EntityManager.SetName(PlatformWalkwaysEntity, "PlatformWalkways");
            EntityManager.SetComponentData(PlatformWalkwaysEntity, new PlatformWalkways { Walkways = platformWalkwayInfos });
        }

        // Platform
        {
            int objCount = metro.allPlatforms.Length;

            NativeArray<PlatformInitInfo> platformsInfo = new NativeArray<PlatformInitInfo>(objCount, Allocator.TempJob);
            for (int i = 0; i < objCount; ++i)
            {
                platformsInfo[i] = new PlatformInitInfo
                {
                    Position = metro.allPlatforms[i].transform.position,
                    Rotation = metro.allPlatforms[i].transform.rotation
                };
            }

            var platformSpawnJob = new PlatfromSpawnJob
            {
                entity = config.PlatformPrefab,
                Ecb = ecb.AsParallelWriter(),
                Count = objCount,
                PlatformsInfo = platformsInfo
            };

            var spawnHandle = platformSpawnJob.Schedule(objCount, 128);
            spawnHandle.Complete();
        }

        // Commuter

        NativeArray<CommuterUnmanagedInfo> commuterUnmanagedInfos = new NativeArray<CommuterUnmanagedInfo>(metro.commuters.Count, Allocator.TempJob);

        NativeArray<TaskIndexRange> taskIndexRanges = new NativeArray<TaskIndexRange>(metro.commuters.Count, Allocator.TempJob);

        // Can't have nested native container, so..hack
        int totalTaskAccount = 0;
        for (int i = 0; i < metro.commuters.Count; i++)
        {
            totalTaskAccount += metro.commuters[i].route_TaskList.Count;
        }
        NativeArray<CommuterTaskUnmanaged> commuterStateTasks = new NativeArray<CommuterTaskUnmanaged>(totalTaskAccount, Allocator.TempJob);

        {
            int taskStartIndex = 0;
            int taskEndIndex = 0;

            for (int i = 0; i < metro.commuters.Count; i++)
            {
                commuterUnmanagedInfos[i] = new CommuterUnmanagedInfo
                {
                    StartPlatform = metro.commuters[i].currentPlatform.globalPlatformIndex,
                    FinalPlatform = metro.commuters[i].FinalDestination.globalPlatformIndex,

                    InitPosition = metro.commuters[i].currentPlatform.transform.position
                };

                taskStartIndex = taskEndIndex;
                taskEndIndex = taskStartIndex + metro.commuters[i].route_TaskList.Count;

                taskIndexRanges[i] = new TaskIndexRange { StartIndex = taskStartIndex, EndIndex = taskEndIndex };

                CommuterTask[] taskList = metro.commuters[i].route_TaskList.ToArray();
                for (int j = 0; j < metro.commuters[i].route_TaskList.Count; j++)
                {
                    int desNum = (taskList[j].destinations is null) ? 0 : taskList[j].destinations.Length;
                    float4 d0 = desNum > 0 ? new float4(taskList[j].destinations[0].x, taskList[j].destinations[0].y, taskList[j].destinations[0].z, 0) : new float4(0);
                    float4 d1 = desNum > 1 ? new float4(taskList[j].destinations[1].x, taskList[j].destinations[1].y, taskList[j].destinations[1].z, 0) : new float4(0);
                    float4 d2 = desNum > 2 ? new float4(taskList[j].destinations[2].x, taskList[j].destinations[2].y, taskList[j].destinations[2].z, 0) : new float4(0);
                    float4 d3 = desNum > 3 ? new float4(taskList[j].destinations[3].x, taskList[j].destinations[3].y, taskList[j].destinations[3].z, 0) : new float4(0);

                    commuterStateTasks[taskStartIndex + j] = new CommuterTaskUnmanaged
                    {
                        State           = (int)taskList[j].state,
                        StartPlatform   = (taskList[j].startPlatform is null) ? -1 : taskList[j].startPlatform.globalPlatformIndex,
                        EndPlatform     = (taskList[j].endPlatform is null) ? -1 : taskList[j].endPlatform.globalPlatformIndex,
                        WalkwayInfo     = (taskList[j].walkway is null) ? new WalkwayConnectInfo() :
                                            new WalkwayConnectInfo
                                            {
                                                platform_Connects_FROM = taskList[j].walkway.connects_FROM.globalPlatformIndex,
                                                platform_Connects_TO = taskList[j].walkway.connects_TO.globalPlatformIndex,
                                                nav_START = taskList[j].walkway.nav_START.position,
                                                nav_END = taskList[j].walkway.nav_END.position
                                            },
                        DestNum = desNum,
                        Destinations = new float4x4(d0, d1, d2, d3),
                        DestinationCount = desNum,
                        DestinationIndex = 0
                    };
                }
            }

            int objCount = metro.commuters.Count;

            var commuterSpawnJob = new CommuterSpawnJob
            {
                entity = config.CommuterPrefab,
                Ecb = ecb.AsParallelWriter(),
                Count = objCount,
                mask = m_BaseColorQuery.GetEntityQueryMask(),
                commuterInfos = commuterUnmanagedInfos,
                taskList = commuterStateTasks,
                taskIndexRanges = taskIndexRanges,
                WalkwayEntity = PlatformWalkwaysEntity
            };

            var spawnHandle = commuterSpawnJob.Schedule(objCount, 128);
            spawnHandle.Complete();
        }

        ecb.Playback(EntityManager);
        ecb.Dispose();

        IsSpawningDone = true;
    }

    protected override void OnUpdate()
    {
        if (Metro.INSTANCE._ENABLE_DOTS)
        {
            Spawn();

            Metro.INSTANCE.IS_COMMUTER_SPAWNED = IsSpawningDone;
        }
    }


    /// <summary>
    /// Jobs 
    /// </summary>
    //////////////////////////////////////////////////////////////////////////////////////////////////////////
    
    // Example Burst job that creates many entities
    [BurstCompile]
    public struct PlatfromSpawnJob : IJobParallelFor
    {
        public Entity entity;
        public int Count;
        public EntityCommandBuffer.ParallelWriter Ecb;

        public NativeArray<PlatformInitInfo> PlatformsInfo;

        public void Execute(int index)
        {
            // Clone the Prototype entity to create a new entity.
            var e = Ecb.Instantiate(index, entity);

            Ecb.SetComponent(index, e, new LocalTransform { Position = PlatformsInfo[index].Position, Scale = 1f, Rotation = PlatformsInfo[index].Rotation });
        }
    }


    [BurstCompile]
    public struct CommuterSpawnJob : IJobParallelFor
    {
        public Entity entity;
        public int Count;
        public EntityCommandBuffer.ParallelWriter Ecb;
        public EntityQueryMask mask;

        public Entity WalkwayEntity;

        [ReadOnly]
        public NativeArray<CommuterUnmanagedInfo> commuterInfos;

        [ReadOnly]
        public NativeArray<CommuterTaskUnmanaged> taskList;

        [ReadOnly]
        public NativeArray<TaskIndexRange> taskIndexRanges;

        [BurstCompile]
        public void Execute(int index)
        {
            // Clone the Prototype entity to create a new entity.
            var e = Ecb.Instantiate(index, entity);

            // Prototype has all correct components up front, can use SetComponent to
            // set values unique to the newly created entity, such as the transform.
            //int matIndex = singleMat ? 0 : index;
            //Ecb.SetComponent(index, e, MaterialMeshInfo.FromRenderMeshArrayIndices(matIndex, 0));
            //Ecb.SetComponent(index, e, new LocalToWorld { Value = ComputeTransform(index) });


            // This system will only run once, so the random seed can be hard-coded.
            // Using an arbitrary constant seed makes the behavior deterministic.
            var random = Unity.Mathematics.Random.CreateFromIndex((uint)index);
            var hue = random.NextFloat();

            // Helper to create any amount of colors as distinct from each other as possible.
            // The logic behind this approach is detailed at the following address:
            // https://martin.ankerl.com/2009/12/09/how-to-create-random-colors-programmatically/
            URPMaterialPropertyBaseColor RandomColor()
            {
                // Note: if you are not familiar with this concept, this is a "local function".
                // You can search for that term on the internet for more information.

                // 0.618034005f == 2 / (math.sqrt(5) + 1) == inverse of the golden ratio
                hue = (hue + 0.618034005f) % 1;
                var color = UnityEngine.Color.HSVToRGB(hue, 1.0f, 1.0f);
                return new URPMaterialPropertyBaseColor { Value = (UnityEngine.Vector4)color };
            }

            CommuterSpeed RandomSpeed()
            {
                //float3 speed = new float3(random.NextFloat(-1f, 1f), random.NextFloat(-1f, 1f), random.NextFloat(-1f, 1f));
                float3 speed = new float3(0);
                const float ACCELERATION_STRENGTH = 0.01f;
                float acc = ACCELERATION_STRENGTH * random.NextFloat(0.8f, 2f);
                return new CommuterSpeed { Value = speed, acceleration = acc };
            }

            Ecb.SetComponentForLinkedEntityGroup(index, e, mask, RandomColor());

            Ecb.SetComponent(index, e, RandomSpeed());

            Ecb.SetComponent(index, e, new LocalTransform { Position = commuterInfos[index].InitPosition, Scale = 1f, Rotation = quaternion.identity });

            Ecb.SetComponent(index, e, new CommuterPlatformIDInfo {
                CurrentPlatformID = commuterInfos[index].StartPlatform,
                StartPlatformID = commuterInfos[index].StartPlatform,
                FinalPlatformID = commuterInfos[index].FinalPlatform
            });

            NativeQueue<CommuterTaskUnmanaged> taskQueue = new NativeQueue<CommuterTaskUnmanaged>(Allocator.TempJob);

            int taskStartIndex = taskIndexRanges[index].StartIndex;
            int taskEndIndex = taskIndexRanges[index].EndIndex;
            int taksCount = taskEndIndex - taskStartIndex;

            for (int i = taskStartIndex; i < taskEndIndex; ++i)
            {
                taskQueue.Enqueue(taskList[i]);
            }
            Ecb.SetComponent(index, e, new CommuterStateInfo { WalkwaysEntity = WalkwayEntity, CurrentTaskIndex = 0, NeedNextTask = 1, TaskList = taskQueue });
        }
    }
}