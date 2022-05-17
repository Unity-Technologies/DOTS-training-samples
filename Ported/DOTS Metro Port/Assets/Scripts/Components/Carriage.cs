using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public struct Carriage : IComponentData
{
	public Entity Train;
	public int CarriageIndex;
}
