using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct Postition : IComponentData
{
	public int2 Position;	//discrete location for index in the board grid
}
