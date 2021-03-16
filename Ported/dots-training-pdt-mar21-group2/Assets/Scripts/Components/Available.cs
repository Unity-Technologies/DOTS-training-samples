using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
// Tag used to sort Rocks/Cans that are available to be grabbed or thrown at
public struct Available : IComponentData
{
}
