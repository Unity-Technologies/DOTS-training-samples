using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
public struct ResourceComponent : IComponentData
{
   public Entity resourcePrefab;

   public int gridX;
   public int gridY;

}
