using System;
using Unity.Entities;

/// <summary>
///     Ants die if away from the <see cref="ColonyFlag"/>/<see cref="FoodSourceFlag"/> for a while.
/// </summary>
public struct LifeTicks : IComponentData
{
    /// <summary>
    ///     Value of 0 indicates dead.
    ///     NWalker: Would this be better as lifeDrainStartFrame and lifeHp, so data is only WRITTEN exceptionally rarely?
    /// </summary>
    public ushort value;
}