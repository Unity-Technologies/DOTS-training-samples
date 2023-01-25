using Unity.Entities;
using UnityEngine;

public struct PassFullTag : IComponentData
{
}

public struct PassEmptyTag : IComponentData
{
}

public struct FetcherTag : IComponentData
{
}

public struct ShouldPassTag : IComponentData, IEnableableComponent
{
}

public struct HasReachedDestinationTag : IComponentData, IEnableableComponent
{
}