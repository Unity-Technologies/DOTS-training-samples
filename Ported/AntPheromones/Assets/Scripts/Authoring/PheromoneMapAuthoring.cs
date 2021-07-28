using Unity.Entities;
using Unity.Mathematics;

public class PheromoneMapAuthoring : UnityEngine.MonoBehaviour, IConvertGameObjectToEntity
{
    public float trailDecay = 0.015f;
    public float trailSpeed = 0.3f;

    public int mapSize = 128;
    public float2 offset = new float2(14,14);
    public float worldSize = 100f;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new PheromoneMapSetting
        {
            Size = mapSize,
            Offset = offset,
            WorldSize = worldSize
        });
        dstManager.AddComponentData(entity, new PheromoneTrailSetting
        {
            Decay = trailDecay,
            Speed = trailSpeed
        });

        int mapSize2 = mapSize * mapSize;
        var buffer = dstManager.AddBuffer<Pheromone>(entity).Reinterpret<float4>();
        buffer.ResizeUninitialized(mapSize2);
        for (int i = 0; i < mapSize2; i++)
            buffer[i] = 0;
    }
}
