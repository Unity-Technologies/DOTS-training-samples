using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

namespace AntPheromones_ECS
{
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public class RenderPheromoneSystem : JobComponentSystem
    {
        private (bool IsInitialised, Texture2D Value) _pheromoneTexture;
        private EntityQuery _pheromoneBufferQuery;
        private EntityQuery _renderingQuery;
        private Color[] _colours;
        private Material _pheromoneMaterial;

        protected override void OnCreate()
        {
            base.OnCreate();
            
            this._pheromoneBufferQuery = 
                GetEntityQuery(ComponentType.ReadOnly<PheromoneColourRValueBuffer>());
            this._renderingQuery = 
                GetEntityQuery(ComponentType.ReadOnly<PheromoneSharedRendering>());
        }

        protected override unsafe JobHandle OnUpdate(JobHandle inputDeps)
        {
            var renderer =
                EntityManager.GetSharedComponentData<PheromoneSharedRendering>(this._renderingQuery.GetSingletonEntity());
            
            if (!this._pheromoneTexture.IsInitialised)
            {
                Map map = GetSingleton<Map>();
              
                this._pheromoneTexture =
                    (IsInitialised: true,
                        Value: new Texture2D(map.Width, map.Width) {wrapMode = TextureWrapMode.Mirror});
                this._pheromoneMaterial = 
                    new Material(renderer.Material){mainTexture = this._pheromoneTexture.Value};
                this._colours = 
                    new Color[this._pheromoneTexture.Value.width * this._pheromoneTexture.Value.height];
            }

            renderer.Renderer.sharedMaterial = this._pheromoneMaterial;

            fixed (Color* colours = this._colours)
            {
                var pheromoneBuffer =
                    EntityManager.GetBuffer<PheromoneColourRValueBuffer>(this._pheromoneBufferQuery.GetSingletonEntity());
                
                new Job
                {
                    Pheromones = pheromoneBuffer,
                    Colours = colours
                }.Schedule(pheromoneBuffer.Length, 256, inputDeps).Complete();
            }

            
            this._pheromoneTexture.Value.SetPixels(_colours);
            this._pheromoneTexture.Value.filterMode = FilterMode.Point;
            this._pheromoneTexture.Value.Apply();

            return default;
        }

        [BurstCompile]
        private unsafe struct Job : IJobParallelFor
        {
            [ReadOnly] public DynamicBuffer<PheromoneColourRValueBuffer> Pheromones;
            [NativeDisableUnsafePtrRestriction] public Color* Colours;
            
            public void Execute(int index)
            {
                this.Colours[index].r = this.Pheromones[index];
            }
        }
    }
}