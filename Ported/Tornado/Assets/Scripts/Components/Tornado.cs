using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct Tornado : IComponentData
{
	public float tornadoX;
	public float tornadoZ;
	public float fader;
	public float maxForceDist;
	public float height;
	public float force;
	public float upForce;
	public float inwardForce;
	public float groundToCoverSize;

}
