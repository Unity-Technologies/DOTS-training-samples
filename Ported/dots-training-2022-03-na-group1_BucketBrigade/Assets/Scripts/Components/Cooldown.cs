using Unity.Entities;

/// <summary>
///     Holds the cooldown time for a bot filling a bucket.
/// </summary>
struct Cooldown : IComponentData
{
    /// <summary>
    ///     The time the filling process is finished.
    /// </summary>
    public float Value;
}
