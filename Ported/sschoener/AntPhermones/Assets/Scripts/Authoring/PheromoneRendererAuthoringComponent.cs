using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using UnityEngine;

[RequiresEntityConversion]
public class PheromoneRendererAuthoringComponent : MonoBehaviour, IConvertGameObjectToEntity
{
    public MeshRenderer Renderer;
    public Material Material;
    public GameManager GameManager;
    
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddSharedComponentData(entity, new PheromoneRenderData
        {
            Material = Material,
            Renderer = Renderer
        });
        int size = GameManager.mapSize;
        var buffer = dstManager.AddBuffer<PheromoneBuffer>(entity);
        buffer.ResizeUninitialized(size * size);
        unsafe
        {
            UnsafeUtility.MemClear(buffer.GetUnsafePtr(), buffer.Length * sizeof(PheromoneBuffer));
        }
    }
}