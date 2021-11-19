using Unity.Entities;

public struct Heat : IBufferElementData
{
    #region Constants
    /// <summary>
    /// Non Burning Heat value
    /// </summary>
    const float NonBurningHeatValue = default;

    /// <summary>
    /// Initial fire Heat value
    /// </summary>
    const float InitialFireSpotHeatValue = 1.0f;
    #endregion Constants

    /// <summary>
    /// Heat value in [NonBurningHeatValue, InitialFireSpotHeatValue]
    /// </summary>
    public float Value;

    #region Constant Structures
    /// <summary>
    /// Initial fire value (Lit on Start)
    /// </summary>
    public static Heat InitialFireSpotHeat => new Heat() { Value = InitialFireSpotHeatValue };

    /// <summary>
    /// Fire value of cells not burning
    /// </summary>
    public static Heat NonBurningHeat => new Heat() { Value = NonBurningHeatValue };
    #endregion Constant Structures
}