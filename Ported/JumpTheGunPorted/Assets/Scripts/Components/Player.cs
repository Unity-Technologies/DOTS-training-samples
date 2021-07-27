using Unity.Entities;

// When a component doesn't require any special handling during conversion,
// the attribute GenerateAuthoringComponent can be used to automatically
// generate an authoring component with the same fields and a conversion
// function that simply copies the fields from one to the other.
[GenerateAuthoringComponent]
public struct Player : IComponentData
{
    /// <summary>
    /// Raise player's origin this much above the top of a box.
    /// </summary>
    public const float Y_OFFSET = .3f;

    public const float BOUNCE_HEIGHT = 2;

    public const float BOUNCE_BASE_DURATION = .7f;
}