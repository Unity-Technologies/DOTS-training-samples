using Unity.Entities;

enum Team
{
    Yellow,
    Blue
}

struct TeamComponent : IComponentData
{
    public Team Value;
}