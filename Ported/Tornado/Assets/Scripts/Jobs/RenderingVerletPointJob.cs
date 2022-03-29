
using Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;

namespace Assets.Scripts.Jobs
{
    internal struct RenderingVerletPointJob : IJobEntityBatch
    {

        [ReadOnly]public NativeArray<VerletPoints> points;

        [ReadOnly]public ComponentTypeHandle<Bar> bars;

        public ComponentTypeHandle<Translation> translations;

        public void Execute(ArchetypeChunk batchInChunk, int batchIndex)
        {
            NativeArray<Bar> barsList = batchInChunk.GetNativeArray(bars);
            NativeArray<Translation> translationList = batchInChunk.GetNativeArray(translations);

            for (int i = 0; i < batchInChunk.Count; i++)
            {
                var trans = translationList[i];

                trans.Value = points[barsList[i].indexPoint].currentPosition;

                translationList[i] = trans;
            }            
        }
    }
}
