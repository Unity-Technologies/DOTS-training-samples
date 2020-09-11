using Unity.Entities;

[GenerateAuthoringComponent]
struct GameConfig : IComponentData
{
#pragma warning disable 0649
    public int Duration;
    public int RestartDelay;
#pragma warning restore
}
