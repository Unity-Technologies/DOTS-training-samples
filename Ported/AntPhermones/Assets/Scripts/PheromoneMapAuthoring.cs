using Unity.Entities;
using Unity.Mathematics;

[InternalBufferCapacity(8)]
public struct PheromoneStrength : IBufferElementData
{
    public static implicit operator float(PheromoneStrength e) { return e.Value; }
    public static implicit operator PheromoneStrength(float e) { return new PheromoneStrength { Value = e }; }

    public float Value;
}

[GenerateAuthoringComponent]
public struct PheromoneMap : IComponentData
{
    public int Resolution;
    public float WorldSpaceSize;
    public float AntPheremoneStrength;
    public float PheremoneDecay;

    public static int2 WorldToGridPos(PheromoneMap map, float3 worldPos) {
        float offset = map.WorldSpaceSize / 2f;
        float x = (worldPos.x + offset) / map.WorldSpaceSize;
        float z = (worldPos.z + offset)/ map.WorldSpaceSize;
        return new int2((int)math.round(x * map.Resolution), (int)math.round(z * map.Resolution));
    }

    public static float3 GridToWorldPos(PheromoneMap map, int2 gridPos) {
        float offset = map.WorldSpaceSize / 2f;
        float x = ((gridPos.x) / (float)(map.Resolution));
        float z = ((gridPos.y) / (float)(map.Resolution));
        x -= offset;
        z -= offset;
        return new float3(x * map.WorldSpaceSize, 0, z * map.WorldSpaceSize);
    }

    public static int GridPosToIndex(PheromoneMap map, int2 gridPos) {
        var index = gridPos.x + map.Resolution * gridPos.y;
        return index;
    }
}