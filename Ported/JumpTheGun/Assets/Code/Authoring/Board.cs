
using Unity.Entities;

[GenerateAuthoringComponent]
public struct Board : IComponentData
{
    public int SizeX;
    public int SizeY;

    public float MinHeight;
    public float MaxHeight;

    public float ReloadTime;
    public float Radius;

    public int NumberOfTanks;
    public float HitStrength;
    
    public Entity PlaformPrefab;
    public Entity TankPrefab;
}
