using Unity.Entities;

struct Plant : IComponentData
{
    public Entity ClaimedBy;
}