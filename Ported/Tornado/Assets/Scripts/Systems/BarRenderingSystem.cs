using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Systems
{
    public partial class BarRenderingSystem : SystemBase
    {
        
        protected override void OnUpdate()
        {
            
            var time = (float)Time.ElapsedTime;

            var pointDisplacement = World.GetExistingSystem<PointDisplacementSystem>();
            if (!pointDisplacement.isInitialized) return;


            var points = pointDisplacement.points;

            Entities
                .ForEach((ref Translation translation, in Components.Bar bar) =>
                {

                    translation.Value = points[bar.indexPoint].currentPosition;
                    

                }).Run();

            


        }
    }
}