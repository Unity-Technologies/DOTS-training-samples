using Unity.Entities;
using Unity.Mathematics;

[InternalBufferCapacity(1)] // Can this be 0?
struct PheromoneMap : IBufferElementData
{
    public float amount;
}

struct PheromoneMapUtil
{
    public static float2 WorldToPheromoneMap(int playAreaSize, float2 posXZ)
    {
        int2 textureSize = new int2(PheromoneDisplaySystem.PheromoneTextureSizeX, PheromoneDisplaySystem.PheromoneTextureSizeX);
        float2 planeExtent = new float2(playAreaSize);
        float2 planeHalfExtent = planeExtent / 2;
        
        float2 posNormalized = (posXZ + planeHalfExtent) / planeExtent;
        return posNormalized * textureSize;
    }
    
    public static void AddAmount(ref DynamicBuffer<PheromoneMap> buffer, int x, int y, float amount)
    {
        if ((uint)x < PheromoneDisplaySystem.PheromoneTextureSizeX && (uint)y < PheromoneDisplaySystem.PheromoneTextureSizeY)
        {
            int bufIndex = x + y * PheromoneDisplaySystem.PheromoneTextureSizeX;
            ref var cell = ref buffer.ElementAt(bufIndex);
            cell.amount += amount;
        }   
    }
    
    public static void SetAmount(ref DynamicBuffer<PheromoneMap> buffer, int x, int y, float amount)
    {
        if ((uint)x < PheromoneDisplaySystem.PheromoneTextureSizeX && (uint)y < PheromoneDisplaySystem.PheromoneTextureSizeY)
        {
            int bufIndex = x + y * PheromoneDisplaySystem.PheromoneTextureSizeX;
            ref var cell = ref buffer.ElementAt(bufIndex);
            cell.amount = amount;
        }   
    }
    
    public static float GetAmount(ref DynamicBuffer<PheromoneMap> buffer, int x, int y)
    {
        if ((uint)x < PheromoneDisplaySystem.PheromoneTextureSizeX && (uint)y < PheromoneDisplaySystem.PheromoneTextureSizeY)
        {
            int bufIndex = x + y * PheromoneDisplaySystem.PheromoneTextureSizeX;
            ref var cell = ref buffer.ElementAt(bufIndex);
            return cell.amount;
        }

        return 0.0f;
    }
}