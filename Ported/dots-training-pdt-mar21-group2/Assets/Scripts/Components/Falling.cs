using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
// Tag used mark objects that need Gravity applied to them
public struct Falling : IComponentData
{
}
