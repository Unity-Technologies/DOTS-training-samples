using Unity.Entities;

public struct Ant: IComponentData
{
    public float wallSteering;
    public float pheroSteering;
    public bool hasResource;
}
