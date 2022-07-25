using Unity.Entities;

#region step1
struct Config : IComponentData
{
    public Entity CarPrefab;
    public int CarCount;
}
#endregion