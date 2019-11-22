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
        
        protected override void OnCreate()
        {
            base.OnCreate();
            
            this._pheromoneBufferQuery = 
                GetEntityQuery(ComponentType.ReadOnly<PheromoneColourRValueBuffer>());
            this._renderingQuery = 
                GetEntityQuery(ComponentType.ReadOnly<PheromoneRenderingSharedComponent>());
        }

        protected override void OnUpdate()
        {
            if (!this._pheromoneTexture.IsInitialised)
            {
                var renderer =
                    EntityManager.GetSharedComponentData<PheromoneRenderingSharedComponent>(
                        this._renderingQuery.GetSingletonEntity());
                MapComponent map = GetSingleton<MapComponent>();
              
                this._pheromoneTexture =
                    (IsInitialised: true,
                    Value: new Texture2D(map.Width, map.Width) {wrapMode = TextureWrapMode.Mirror});
                this._colours = new Color[this._pheromoneTexture.Value.width * this._pheromoneTexture.Value.height];
                
                renderer.Renderer.sharedMaterial = new Material(renderer.Material){mainTexture = this._pheromoneTexture.Value};
            }
            
            var pheromoneBuffer = 
                EntityManager.GetBuffer<PheromoneColourRValueBuffer>(this._pheromoneBufferQuery.GetSingletonEntity());
            
            for (int i = 0; i < pheromoneBuffer.Length; i++)
            {
                this._colours[i].r = pheromoneBuffer[i];
            }
            
            this._pheromoneTexture.Value.SetPixels(_colours);
            this._pheromoneTexture.Value.Apply();
        }
    }
}