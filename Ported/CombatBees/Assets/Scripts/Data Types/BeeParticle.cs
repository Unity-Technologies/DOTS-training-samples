using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeeParticle {
	public ParticleType type;
	public Vector3 position;
	public Vector3 velocity;
	public Vector3 size;
	public float life;
	public float lifeDuration;
	public Vector4 color;
	public bool stuck;
	public Matrix4x4 cachedMatrix;
}
