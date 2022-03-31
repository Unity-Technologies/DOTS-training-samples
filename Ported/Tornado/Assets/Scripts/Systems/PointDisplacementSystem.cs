using Assets.Scripts.Jobs;
using Components;
using System;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using UnityEngine;

namespace Systems
{
    [UpdateBefore(typeof(BarRenderingSystem))]
    public partial class PointDisplacementSystem : SystemBase
    {
        public NativeArray<VerletPoints> points;
        public NativeArray<Link> links;
        public NativeArray<Components.PhysicMaterial> physicmaterials;
        public NativeArray<int> islandPointAllocators;
        public NativeArray<int> linkStartIndices;
        public bool isInitialized;


        // public static int AllocatedPointCount;



        public const int SoftBar = 0;
        public const int HardBar = 1;


        EntityQueryDesc barQueryDesc = new EntityQueryDesc
        {
            All = new ComponentType[] { typeof(Translation), typeof(Bar) },
        };

        EntityQuery barQuery;
        protected override void OnCreate()
        {
            base.OnCreate();
            barQuery = GetEntityQuery(barQueryDesc);

            physicmaterials = new NativeArray<Components.PhysicMaterial>(2, Allocator.Persistent);
            physicmaterials[0] = new Components.PhysicMaterial() { weight = 1 };
            physicmaterials[1] = new Components.PhysicMaterial() { weight = 1.6f };
        }


        public void Reset()
        {
            var tornadoParams = GetSingleton<TornadoParameters>();

            tornadoParams.tornadoFader = 0.0f;

            SetSingleton<TornadoParameters>(tornadoParams);

            if (points.IsCreated)
            {
                points.Dispose();
            }

            if (links.IsCreated)
            {
                links.Dispose();
            }

            isInitialized = false;
        }

        protected override void OnUpdate()
        {
            if (!isInitialized) return;

            float invDamping = 1f - 0.012f;
            var tornadoParams = GetSingleton<TornadoParameters>();
            var tornadoSettings = GetSingleton<TornadoSettings>();
            var physicParameters = GetSingleton<PhysicsSettings>();

            tornadoParams.tornadoFader = Mathf.Clamp01(tornadoParams.tornadoFader + Time.DeltaTime / 10f);

            SetSingleton<TornadoParameters>(tornadoParams);

           //burst compatible & parallalized 
            var jobDisplacement = new PointDisplacementJob()
            {
                points = points,
                invDamping = invDamping,
                torandoParameters = tornadoParams,
                torandoSettings = tornadoSettings,
                time = (float)Time.ElapsedTime,
                random = new Unity.Mathematics.Random(1234),
                physicMaterials = physicmaterials,
                physicSettings = physicParameters
            };


            var pointDisplacementSystem = World.GetExistingSystem<PointDisplacementSystem>();
            var pointAllocators = pointDisplacementSystem.islandPointAllocators;
            int islandCount = pointAllocators.Length;

            //parallelized  - fix point iteration
            JobHandle jobHandlePoint = jobDisplacement.Schedule(points.Length, 64, Dependency);
            NativeArray<JobHandle> constraintJobs = new NativeArray<JobHandle>(islandCount,Allocator.Temp);

            //burst compatible
            for (int islandIndex = 0; islandIndex < islandCount; ++islandIndex)
            {
                var constraintJob = new ContraintJob()
                {
                    points = points,
                    links = links,
                    islandIndex = islandIndex,
                    pointAllocators = pointAllocators,
                    islandStartLinkIndex = linkStartIndices[islandIndex],
                    islandEndLinkIndex = islandIndex < linkStartIndices.Length - 1 ? linkStartIndices[islandIndex + 1] : links.Length,
                    iterations = physicParameters.constraintIterations,
                    physicSettings = physicParameters
                };

                JobHandle jobHandleConstraint = constraintJob.Schedule(jobHandlePoint);
                constraintJobs[islandIndex] = jobHandleConstraint;


                // startLinkIndex += pointAllocators[]
            }

            Dependency = JobHandle.CombineDependencies(constraintJobs);
            constraintJobs.Dispose();

            // var constraintJob = new ContraintJob()
            // {
            //     points = points,
            //     links = links,
            //     count = pointAllocators,
            //     iterations = physicParameters.constraintIterations,
            //     physicSettings = physicParameters
            // };
         



            //signle threaded job
            // JobHandle jobHandleConstraint = constraintJob.Schedule(jobHandlePoint);


            JobHandle.ScheduleBatchedJobs();
            // jobHandleConstraint.Complete();

            // AllocatedPointCount = pointCount[0];
            // pointCount.Dispose();

          
            // Dependency = jobHandleConstraint;

        }

        public void Initialize(NativeArray<VerletPoints> points, NativeArray<Link> links, NativeArray<int> islandPointAllocators, NativeArray<int> linkStartIndices)
        {
            this.links = links;
            this.points = points;
            isInitialized = true;         
            
            //AllocatedPointCount = allocatedPoints;
            this.islandPointAllocators = new NativeArray<int>(islandPointAllocators, Allocator.Persistent);
            this.linkStartIndices = new NativeArray<int>(linkStartIndices, Allocator.Persistent);

        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            points.Dispose();
            links.Dispose();
            physicmaterials.Dispose();
            islandPointAllocators.Dispose();
            linkStartIndices.Dispose();
        }
    }
}