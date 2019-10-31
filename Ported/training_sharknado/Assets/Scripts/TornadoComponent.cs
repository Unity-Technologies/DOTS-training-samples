using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct TornadoSpawner : IComponentData
{
	public Entity particle;
	public int numOfParticles;
	public float force;
	public float maxForceDist;
	public float height;
	public float upForce;
	public float inwardForce;
	public float damping;
	public float spinRate;
	public float expForce;
	public float breakResist;
	public float particleSpinRate;
	public float particleUpSpeed;
	public float friction;
	public float gravity;
}
