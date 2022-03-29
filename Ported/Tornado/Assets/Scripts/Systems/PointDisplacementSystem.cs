using Assets.Scripts.Jobs;
using Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using UnityEngine;

namespace Systems
{
    public partial class PointDisplacementSystem : SystemBase
    {
        public NativeArray<VerletPoints> points;
        public NativeArray<Link> links;              

        public bool isInitialized;


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
        protected override void OnUpdate()
        {
            if (!isInitialized) return;

            float invDamping = 1f - 0.012f;

            var jobDisplacement = new PointDisplacementJob()
            {
                points = points,
                invDamping = invDamping
            };

            var constraintJob = new ContraintJob()
            {
                points = points,
                links = links
            };
         
            //parallelized 
            JobHandle jobHandlePoint = jobDisplacement.Schedule(points.Length, 64);
            JobHandle jobHandleConstraint = constraintJob.Schedule(jobHandlePoint);          

            JobHandle.ScheduleBatchedJobs();

            jobHandlePoint.Complete();
            jobHandleConstraint.Complete();   

            var renderingJob = new RenderingVerletPointJob()
            {
                points = points,
                bars = GetComponentTypeHandle<Bar>(),
                translations = GetComponentTypeHandle<Translation>(),
            };

            
            JobHandle jobHandleRendering = renderingJob.ScheduleParallel(barQuery, Dependency);
            JobHandle.ScheduleBatchedJobs();
            jobHandleRendering.Complete();          
           

        }

       

        public void Initialize(NativeArray<VerletPoints> points, NativeArray<Link> links)
        {
            this.links = links;
            this.points = points;
            isInitialized = true;
          
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if(points != null) points.Dispose();
            if(links != null) links.Dispose();
        }
    }
}