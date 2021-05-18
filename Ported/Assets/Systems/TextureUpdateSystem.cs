using Unity.Collections;
using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

public class TextureUpdateSystem : SystemBase
{
    private EntityQuery MissingCustomTextureQuery;
    private EntityQuery PresentCustomTextureQuery;

    protected override void OnCreate()
    {
        MissingCustomTextureQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new ComponentType[] {typeof(PheromoneMap), typeof(RenderMesh)},
            None = new ComponentType[] {typeof(Pheromone)}
        });

        PresentCustomTextureQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new ComponentType[] {typeof(PheromoneMap), typeof(Pheromone), typeof(RenderMesh)},
        });
    }

    protected override void OnUpdate()
    {
        if (!MissingCustomTextureQuery.IsEmpty)
        {
            var entities = MissingCustomTextureQuery.ToEntityArray(Allocator.Temp);
            var colorMapsSizes = MissingCustomTextureQuery.ToComponentDataArray<PheromoneMap>(Allocator.Temp);

            for (int i = 0; i < entities.Length; i += 1)
            {
                var size = colorMapsSizes[i].gridSize;

                var buffer = EntityManager.AddBuffer<Pheromone>(entities[i]);
                buffer.Length = size * size;
                
                for (int j = 0; j < buffer.Length; j++)
                {
                    buffer[j] = new Pheromone
                    {
                        Value = 0.0f
                    };
                }

                var material = EntityManager.GetSharedComponentData<RenderMesh>(entities[i]).material;
                material.mainTexture = new Texture2D(size, size, TextureFormat.RFloat, false);
            }

            colorMapsSizes.Dispose();
            entities.Dispose();
        }

        if (!PresentCustomTextureQuery.IsEmpty)
        {
            var entities = PresentCustomTextureQuery.ToEntityArray(Allocator.Temp);

            for (int i = 0; i < entities.Length; i += 1)
            {
                var colorMap = EntityManager.GetBuffer<Pheromone>(entities[i]);
                
                var texture = (Texture2D) EntityManager.GetSharedComponentData<RenderMesh>(entities[i]).material.mainTexture;
                var raw = texture.GetRawTextureData<float>();
                raw.CopyFrom(colorMap.AsNativeArray().Reinterpret<float>());
                texture.Apply();
            }

            entities.Dispose();
        }
    }
}