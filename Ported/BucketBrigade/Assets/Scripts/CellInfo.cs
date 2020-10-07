using System;
using Unity.Entities;

[GenerateAuthoringComponent]
struct CellInfo : IComponentData
{
	public int X;
	public int Z;
}