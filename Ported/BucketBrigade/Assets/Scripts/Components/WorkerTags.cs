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

public struct CarriesBucketTag : IComponentData, IEnableableComponent
{
}

public struct HasReachedDestinationTag : IComponentData, IEnableableComponent
{
}