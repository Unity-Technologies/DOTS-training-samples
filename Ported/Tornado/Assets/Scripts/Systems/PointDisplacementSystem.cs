using Assets.Scripts.Jobs;
using Components;
using Unity.Collections;
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

        public bool isInitialized;

        public static int AllocatedPointCount;

        EntityQueryDesc barQueryDesc = new EntityQueryDesc
        {
            All = new ComponentType[] { typeof(Translation), typeof(Bar) },

        };

        EntityQuery barQuery;
        protected override void OnCreate()
        {
            base.OnCreate();
            barQuery = GetEntityQuery(barQueryDesc);
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

            var jobDisplacement = new PointDisplacementJob()
            {
                points = points,
                invDamping = invDamping,
                torandoParameters = tornadoParams,
                torandoSettings = tornadoSettings,
                time = (float)Time.ElapsedTime,
                random = new Unity.Mathematics.Random(1234),
            };


            var constraintJob = new ContraintJob()
            {
                points = points,
                links = links,
                iterations = physicParameters.constraintIterations,
                physicSettings = physicParameters
            };
         
            //parallelized 
            JobHandle jobHandlePoint = jobDisplacement.Schedule(AllocatedPointCount, 64, Dependency);

            //signle threaded job
            JobHandle jobHandleConstraint = constraintJob.Schedule(jobHandlePoint);          

            JobHandle.ScheduleBatchedJobs();          

            Dependency = jobHandleConstraint;

        }

        public void Initialize(NativeArray<VerletPoints> points, NativeArray<Link> links, int allocatedPoints)
        {
            this.links = links;
            this.points = points;
            isInitialized = true;

            AllocatedPointCount = allocatedPoints;
            Debug.Log(AllocatedPointCount + "   " + allocatedPoints);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if(points != null) points.Dispose();
            if(links != null) links.Dispose();
        }
    }
}