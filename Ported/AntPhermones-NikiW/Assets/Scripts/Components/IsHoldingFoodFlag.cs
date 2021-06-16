using System;
using Unity.Entities;

/// <summary>
///     Ants hold food via <see cref="IsHoldingFoodFlag"/>, which they pick up from a <see cref="FoodSourceFlag"/> and drop off at a <see cref="ColonyFlag"/>.
/// </summary>
[GenerateAuthoringComponent]
public struct IsHoldingFoodFlag : IComponentData
{
}