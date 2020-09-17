using Unity.Entities;

[GenerateAuthoringComponent]
public struct AntTag : IComponentData
{
    public const float Size = 0.5f;
    public bool HasFood;
    public float GoalSeekAmount; // How much is this ant currently seeking its goal based on its smell senses (runtime variable)
}