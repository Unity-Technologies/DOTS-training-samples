using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct CellComponentLink : IComponentData
{
    public Entity arrow;
    public Entity arrowOutline;
}
