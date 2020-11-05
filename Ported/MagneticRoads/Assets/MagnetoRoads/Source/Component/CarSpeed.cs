using Unity.Entities;

[GenerateAuthoringComponent]    
public struct CarSpeed : IComponentData
{
    public const float ACCELERATION = 2f;
    public const float MAX_SPEED = 1.0f;
    public const float CAR_SPACING = .13f;
    public float NormalizedValue;
}
