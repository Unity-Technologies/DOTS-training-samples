using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct Board : IComponentData
{
	public int BoardWidth;
	public int BoardHeight;
}
