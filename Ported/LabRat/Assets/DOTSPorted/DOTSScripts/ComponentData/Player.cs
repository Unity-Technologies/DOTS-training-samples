
using Unity.Entities;

public struct Player : IComponentData
{
    public Entity targetEntity;
    public int index;
    public int score;
    public int nextArrowIndex;
}
