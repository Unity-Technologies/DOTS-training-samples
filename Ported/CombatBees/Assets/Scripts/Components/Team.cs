using Unity.Entities;

enum TeamName
{
    Yellow,
    Blue
}

struct Team : IComponentData
{
    public TeamName Value;
}