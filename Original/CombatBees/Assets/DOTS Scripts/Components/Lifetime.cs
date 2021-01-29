using Unity.Entities;

// TODO: Split into 2 components
[GenerateAuthoringComponent]
public struct Lifetime : IComponentData
{
    public float NormalizedTimeRemaining;
    public float NormalizedDecaySpeed;

    public static Lifetime FromTimeRemaining(float timeRemaining)
    {
        return new Lifetime
        {
            NormalizedTimeRemaining = 1,
            NormalizedDecaySpeed = 1 / timeRemaining,
        };
    }
}
