using Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace Assets.Scripts.Jobs
{
    [BurstCompile]
    public struct BarRenderingJob : IJobChunk
    {        
        [ReadOnly] public NativeArray<VerletPoint> points;
        [ReadOnly] public NativeArray<Link> links;

        public ComponentTypeHandle<Rotation> handleRotation;
        public ComponentTypeHandle<Translation> handleTranslation;
        public ComponentTypeHandle<Bar> handleBar;

       
        public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
        {          
            var rotations = chunk.GetNativeArray<Rotation>(handleRotation);
            var translations = chunk.GetNativeArray<Translation>(handleTranslation);
            var bars = chunk.GetNativeArray<Bar>(handleBar);

            for (int i = 0; i < bars.Length; i++)
            {
                var bar = bars[i];
                var rotation = rotations[i];
                var translation = translations[i];

                var link = links[bar.indexLink];
                var start = points[link.point1Index].currentPosition;
                var end = points[link.point2Index].currentPosition;
                var midPoint = (start + end) * 0.5f;            


              
                    rotation.Value = quaternion.LookRotation(link.direction, new float3(0.0f, 1.0f, 0.0f));
                    rotations[i] = rotation;
             
                   
                translation.Value = midPoint;
                translations[i] = translation;

               
            }
        }
    }
}
