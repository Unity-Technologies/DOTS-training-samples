
using Unity.Entities;

public struct BoardSpawnerData : IComponentData
{
    public int SizeX;
    public int SizeY;

    public float MinHeight;
    public float MaxHeight;

    public float ReloadTime;
    public float Radius;

    public int NumberOfTanks;
    public float HitStrength;
    
    public float BulletArcHeightFactor;
    
    public Entity PlaformPrefab;
    public Entity TankPrefab;
    public Entity TurretPrefab;
    public Entity BulletPrefab;
}
