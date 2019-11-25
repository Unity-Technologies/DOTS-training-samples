using Unity.Entities;
using UnityEngine;

namespace AntPheromones_ECS
{
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public class RenderPheromoneSystem : ComponentSystem
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

        protected override void OnUpdate()
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
            
            var pheromoneBuffer = 
                EntityManager.GetBuffer<PheromoneColourRValueBuffer>(this._pheromoneBufferQuery.GetSingletonEntity());
            
            for (int i = 0; i < pheromoneBuffer.Length; i++)
            {
                this._colours[i].r = pheromoneBuffer[i];
            }
            
            this._pheromoneTexture.Value.SetPixels(_colours);
            this._pheromoneTexture.Value.filterMode = FilterMode.Point;
            this._pheromoneTexture.Value.Apply();
        }
    }
}