using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Combatbees.Testing.Mahmoud
{
   public struct ResourceComponent : IComponentData
   {
      public Entity resourcePrefab;

      public int gridX;
      public int gridY;

   }
}