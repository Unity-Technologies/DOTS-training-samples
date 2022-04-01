using Assets.Scripts.Jobs;
using Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Systems
{
  
    public partial class BarRenderingSystem : SystemBase
    {
        EntityQuery renderingQuery;
        
        protected override void OnCreate()
        {
            base.OnCreate();
            //dynamicbuffer
            var entityQueryDesc = new EntityQueryDesc()
            {
                All = new ComponentType[] { typeof(Rotation), typeof(Translation), typeof(Bar) },
                None = new ComponentType[] { typeof(Particle) }
            };


            renderingQuery = GetEntityQuery(entityQueryDesc);
            
        }
        protected override void OnUpdate()
        {
            var pointDisplacement = World.GetExistingSystem<VerletSimulationSystem>();
            if (!pointDisplacement.isInitialized) return;

            var points = pointDisplacement.points;
            var links = pointDisplacement.links;
            
            var renderingJob = new BarRenderingJob()
            {
                handleBar = GetComponentTypeHandle<Bar>(),
                handleRotation = GetComponentTypeHandle<Rotation>(),
                handleTranslation = GetComponentTypeHandle<Translation>(),
                points = pointDisplacement.points, 
                links = pointDisplacement.links
            };

            Dependency = renderingJob.ScheduleParallel(renderingQuery, Dependency);
            
           
        }
    }
}