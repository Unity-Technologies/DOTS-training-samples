#define USE_ENTITY_CAR

using UnityEngine;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Collections.LowLevel.Unsafe;
using System;

namespace HighwayRacers
{
    public class Car : MonoBehaviour
    {
        [Tooltip("Distance from center of car to the front.")]
        public float distanceToFront = 1;
        [Tooltip("Distance from center of car to the back.")]
        public float distanceToBack = 1;

        public Color defaultColor = Color.gray;
        public Color maxSpeedColor = Color.green;
        public Color minSpeedColor = Color.red;

        public float defaultSpeed;
        public float overtakePercent;
        public float leftMergeDistance;
        public float mergeSpace;
        public float overtakeEagerness;

        private bool hidden = false;

        public Entity entity;

        public float maxSpeed { get { return defaultSpeed * overtakePercent; } }

        [Header("Children")]
        public MeshRenderer topRenderer;
        public MeshRenderer baseRenderer;
        public Transform cameraPos;

        public float velocityPosition;

        public float velocityLane;

        /// <summary>
        /// Distance in current lane.  Can change when switching lanes.
        /// </summary>
        public float distance;

        /// <summary>
        /// Ranges from [0, 4]
        /// </summary>
        public float lane;

        public Color color
        {
            get
            {
                return topRenderer.material.color;
            }
            set
            {
                topRenderer.material.color = value;
                baseRenderer.material.color = value;
            }
        }

        public void Show()
        {
            if (!hidden)
                return;
            topRenderer.enabled = true;
            baseRenderer.enabled = true;
            hidden = false;
        }

        public void Hide()
        {
            if (hidden)
                return;
            topRenderer.enabled = false;
            baseRenderer.enabled = false;
            hidden = true;

        }

        public void SetRandomPropeties()
        {
            defaultSpeed = UnityEngine.Random.Range(Game.instance.defaultSpeedMin, Game.instance.defaultSpeedMax);
            overtakePercent = UnityEngine.Random.Range(Game.instance.overtakePercentMin, Game.instance.overtakePercentMax);
            leftMergeDistance = UnityEngine.Random.Range(Game.instance.leftMergeDistanceMin, Game.instance.leftMergeDistanceMax);
            mergeSpace = UnityEngine.Random.Range(Game.instance.mergeSpaceMin, Game.instance.mergeSpaceMax);
            overtakeEagerness = UnityEngine.Random.Range(Game.instance.overtakeEagernessMin, Game.instance.overtakeEagernessMax);
        }

        private void Update()
        {
#if USE_ENTITY_CAR
            transform.localPosition = new Vector3(50.0f, 0.0f, 0.0f);
#else
            var em = World.Active.EntityManager;
            var pos = em.GetComponentData<Position>(entity);
            var vel = em.GetComponentData<Velocity>(entity);

            var time = pos.MergingTime / 0.2f;

            float x0, z0, rotation0;
            Highway.instance.highway1.GetPosition(pos.Pos, pos.Lane, out x0, out z0, out rotation0);

            float x1, z1, rotation1;
            Highway.instance.highway1.GetPosition(pos.Pos / Highway.instance.highway1.length(pos.Lane) * Highway.instance.highway1.length(pos.FromLane), pos.FromLane, out x1, out z1, out rotation1);

            transform.localPosition = new Vector3(Mathf.Lerp(x0, x1, time), transform.position.y, Mathf.Lerp(z0, z1, time));
            transform.localRotation = Quaternion.Euler(0, Mathf.Lerp(rotation0, rotation1, time) * Mathf.Rad2Deg, 0);

            if (vel.Value > vel.Default)
            {
                color = Color.Lerp(defaultColor, maxSpeedColor, (vel.Value - vel.Default) / (maxSpeed - vel.Default));
            }
            else if (vel.Value < vel.Default)
            {
                color = Color.Lerp(minSpeedColor, defaultColor, vel.Value / vel.Default);
            }
            else
            {
                color = defaultColor;
            }

#endif
        }
    }

    public struct Lane0Tag : IComponentData { }
    public struct Lane1Tag : IComponentData { }
    public struct Lane2Tag : IComponentData { }
    public struct Lane3Tag : IComponentData { }

    public struct Position : IComponentData
    {
        public float Pos;
        public float MergingTime;
        public float OvertakingTime;
        public int FromLane;
        public int Lane;
    }

    public struct State : IComponentData
    { 
        public float DefaultSpeed;
        public float OvertakePercent;
        public float OvertakeEagerness;
        public float LeftMergeDistance;
        public float MergeSpace;

        public float MaxSpeed { get { return DefaultSpeed * OvertakePercent; } }
    }

    public struct Velocity : IComponentData
    {
        public float Value;
        public float Default;
    }

    public struct CarInFront
    {
        public float Dist;
        public float Velocity;
    }
    
    public struct PositionSort : IComparable<PositionSort>
    {
        public float Position;
        public float Velocity;
        public int Index;

        public int CompareTo(PositionSort rhs)
        {
            // descending sort
            return Convert.ToInt32(rhs.Position > Position) - Convert.ToInt32(rhs.Position < Position);
        }
    }

    public struct Merge
    {
        public Entity entity;
        public int fromLane;
        public int toLane;
    }

    public static class NativeSortExtension
    {
        unsafe public static JobHandle SortJob<T>(this NativeArray<T> array, int length, JobHandle inputDeps = default) where T : unmanaged, IComparable<T>
        {
            return Unity.Collections.NativeSortExtension.SortJob((T*)NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks(array), length, inputDeps);
        }
    }

    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateBefore(typeof(TransformSystemGroup))]
    public class CarUpdateSystem : JobComponentSystem
    {
        public const float MaxMergingTime = 0.2f;

        NativeArray<PositionSort> positionSort;
        NativeArray<CarInFront> carInFront;
        NativeQueue<Merge> queue;

        static float Wrap(float a, float wrap)
        {
            var mod = a % wrap;
            return mod < 0.0f ? wrap + mod : mod;
        }

        protected override void OnCreate()
        {
            var maxCars = 800;

            queue = new NativeQueue<Merge>(Allocator.Persistent);
            positionSort = new NativeArray<PositionSort>(maxCars, Allocator.Persistent, NativeArrayOptions.ClearMemory);
            carInFront = new NativeArray<CarInFront>(maxCars, Allocator.Persistent, NativeArrayOptions.ClearMemory);
        }

        protected override void OnDestroy()
        {
            queue.Dispose();
            positionSort.Dispose();
            carInFront.Dispose();
        }

        [BurstCompile]
        struct PositionDistJob : IJobForEachWithEntity<Position, Velocity>
        {
            [WriteOnly]
            public NativeArray<PositionSort> positionSort;

            public void Execute(Entity entity, int index, [ReadOnly] ref Position position, [ReadOnly] ref Velocity velocity)
            {
                positionSort[index] = new PositionSort
                {
                    Index = index,
                    Position = position.Pos,
                    Velocity = velocity.Value,
                };
            }
        }

        [BurstCompile]
        struct CalcDistancesJob : IJobParallelFor
        {
            [ReadOnly]
            public NativeArray<PositionSort> positionSort;

            [ReadOnly]
            public float laneLength;

            [ReadOnly]
            public int count;

            [WriteOnly]
            [NativeDisableParallelForRestriction]
            public NativeArray<CarInFront> carInFront;

            public void Execute(int index)
            {
                float d0;
                float velocity;

                if (index == 0)
                {
                    d0 = positionSort[count - 1].Position + laneLength;
                    velocity = positionSort[count - 1].Velocity;
                }
                else
                {
                    d0 = positionSort[index - 1].Position;
                    velocity = positionSort[index - 1].Velocity;
                }

                var d1 = positionSort[index].Position;
                var dist = d0 - d1;
                var remap = positionSort[index].Index;
                carInFront[remap] = new CarInFront { Dist = dist, Velocity = velocity };
            }
        }

        [BurstCompile]
        struct CalcDistancesSingleCarJob : IJob
        {
            [WriteOnly]
            public NativeArray<CarInFront> carInFront;

            public void Execute()
            {
                carInFront[0] = new CarInFront
                {
                    Dist = Highway1.MinDistBetweenCars * 10.0f,
                    Velocity = 40.0f,
                };
            }
        }

        [BurstCompile]
        struct PositionUpdateJob : IJobForEachWithEntity<Position, Velocity, State, LocalToWorld>
        {
            [ReadOnly]
            public NativeArray<CarInFront> carInFront;

            [ReadOnly]
            public float laneLength;

            [ReadOnly]
            public float dt;

            [WriteOnly]
            public NativeQueue<Merge>.ParallelWriter queue;

            [ReadOnly]
            public Highway1 highway1;

            public void Execute(Entity entity, int index, ref Position position, ref Velocity velocity, [ReadOnly] ref State state, ref LocalToWorld localToWorld)
            {
                var maxDist = Math.Max(0.0f, carInFront[index].Dist - Highway1.MinDistBetweenCars * 3.0f);
                var offset = Math.Min(maxDist, velocity.Value * dt);
                position.Pos = Wrap(position.Pos + offset, laneLength);
                position.MergingTime = Math.Max(0.0f, position.MergingTime - dt);
                position.OvertakingTime = Math.Max(0.0f, position.OvertakingTime - dt);
                velocity.Value = Mathf.Lerp(velocity.Value, carInFront[index].Velocity, dt);

                if (position.OvertakingTime == 0.0f
                &&  position.MergingTime == 0.0f)
                {
                    position.FromLane = position.Lane;
                    velocity.Value = state.DefaultSpeed;

                    if (state.OvertakeEagerness > carInFront[index].Velocity / state.DefaultSpeed)
                    {
                        if (position.Lane < Highway1.NumLanes - 1
                        &&  carInFront[index].Dist < state.LeftMergeDistance)
                        {
                            position.OvertakingTime = 3.0f;
                            velocity.Value = state.MaxSpeed;

                            queue.Enqueue(new Merge
                            {
                                entity = entity,
                                fromLane = position.Lane,
                                toLane = position.Lane + 1,
                            });
                        }
                        else if (position.Lane > 0)
                        {
                            queue.Enqueue(new Merge
                            {
                                entity = entity,
                                fromLane = position.Lane,
                                toLane = position.Lane - 1,
                            });
                        }
                    }
                }

#if USE_ENTITY_CAR
                float x0, z0, rotation0;
                highway1.GetPosition(
                      position.Pos
                    , position.Lane
                    , out x0
                    , out z0
                    , out rotation0
                    );

                float x1, z1, rotation1;
                highway1.GetPosition(
                      position.Pos / highway1.length(position.Lane) * highway1.length(position.FromLane)
                    , position.FromLane
                    , out x1
                    , out z1
                    , out rotation1
                    );

                var time = position.MergingTime / MaxMergingTime;

                localToWorld = new LocalToWorld
                {
                    Value = float4x4.TRS(
                          new Vector3(Mathf.Lerp(x0, x1, time), 0.0f, Mathf.Lerp(z0, z1, time))
                        , Quaternion.Euler(0, Mathf.Lerp(rotation0, rotation1, time) * Mathf.Rad2Deg, 0)
                        , new float3(1.0f)
                        )
                };
#endif
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDependencies)
        {
            var type = new ComponentType[] {
                typeof(Lane0Tag),
                typeof(Lane1Tag),
                typeof(Lane2Tag),
                typeof(Lane3Tag),
            };

            var laneLength = new float[] {
                Highway.instance.highway1.length(0.0f),
                Highway.instance.highway1.length(1.0f),
                Highway.instance.highway1.length(2.0f),
                Highway.instance.highway1.length(3.0f),
            };

            Merge merge;
            while (queue.TryDequeue(out merge))
            {
                EntityManager.RemoveComponent(merge.entity, type[merge.fromLane]);
                EntityManager.AddComponent(merge.entity, type[merge.toLane]);
                Position pos = EntityManager.GetComponentData<Position>(merge.entity);
                pos.Pos = pos.Pos / laneLength[merge.fromLane] * laneLength[merge.toLane];
                pos.FromLane = pos.Lane;
                pos.Lane = merge.toLane;
                pos.MergingTime = MaxMergingTime;
                EntityManager.SetComponentData(merge.entity, pos);
            }

            var deps = inputDependencies;
            var queueWriter = queue.AsParallelWriter();

            for (int lane = 0; lane < 4; ++lane)
            {
                var pvQ = GetEntityQuery(
                      ComponentType.ReadOnly<Position>()
                    , ComponentType.ReadOnly<Velocity>()
                    , type[lane]
                    );
                var pvsQ  = GetEntityQuery(
                      ComponentType.ReadWrite<Position>()
                    , ComponentType.ReadWrite<Velocity>()
                    , ComponentType.ReadOnly<State>()
                    , type[lane]
                    , ComponentType.ReadWrite<LocalToWorld>()
                    );
                var count = pvsQ.CalculateEntityCount();

                if (count > 1)
                {
                    deps = new PositionDistJob()
                    {
                        positionSort = positionSort,

                    }.Schedule(pvQ, deps);

                    deps = positionSort.SortJob(count, deps);

                    deps = new CalcDistancesJob()
                    {
                        positionSort = positionSort,
                        laneLength = laneLength[lane],
                        count = count,
                        carInFront = carInFront,

                    }.Schedule(count, 64, deps);
                }
                else
                {
                    deps = new CalcDistancesSingleCarJob()
                    {
                        carInFront = carInFront,

                    }.Schedule(deps);
                }

                deps = new PositionUpdateJob()
                {
                    carInFront = carInFront,
                    laneLength = laneLength[lane],
                    dt = Time.deltaTime,
                    queue = queueWriter,
                    highway1 = Highway.instance.highway1,

                }.Schedule(pvsQ, deps);
            }

#if !USE_ENTITY_CAR
            // GO will be accessing entity components to extract position for rendering...
            deps.Complete();
#endif
            return deps;
        }
    }
}
