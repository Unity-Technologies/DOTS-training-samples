using Unity.Entities;

public struct CitySpawner : IComponentData
{
    public Entity BarPrefab;
    public int NumberOfTowers;
    public int MinTowerHeight, MaxTowerHeight;
    public float CityWidth, CityLength;
}
