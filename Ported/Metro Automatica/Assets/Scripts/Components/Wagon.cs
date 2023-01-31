using Unity.Entities;

struct Wagon : IComponentData
{
    public Entity WagonPrefab;
    public int TrainID;
}
