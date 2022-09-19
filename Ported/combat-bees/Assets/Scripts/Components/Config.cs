using Unity.Entities;

#region step1
struct Config : IComponentData
{
    public Entity TankPrefab;
    public int TankCount;
    public float SafeZoneRadius;
}
#endregion