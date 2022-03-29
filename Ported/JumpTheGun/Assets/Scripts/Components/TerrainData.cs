using Unity.Entities;

[GenerateAuthoringComponent]
public struct TerrainData : IComponentData
{
    // Start is called before the first frame update
    public float MinTerrainHeight;
    public float MaxTerrainHeight;

    public int TerrainWidth;
    public int TerrainLength;
}
