using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct Beam : IComponentData
{
   public float size;
   public int pointAIndex;
   public int pointBIndex;
}
