using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

[NoAlias]
public struct Ant {
	public float2 position;
	public float facingAngle;
	public float speed;
	public bool holdingResource;
	//public float brightness;

	public Ant(float2 pos) {
		position = pos;
		facingAngle = (Random.value * math.PI * 2f);
		speed = 0f;
		holdingResource = false;
		//brightness = Random.Range(.75f,1.25f);
	}
}

/// <summary>
///		NW: Better random.
///		https://github.com/sublee/squirrel3-python/blob/master/squirrel3.py
/// </summary>
public static class Squirrel3
{
	const uint k_Noise1 = 0xb5297a4d;
	const uint k_Noise2 = 0x68e31da4;
	const uint k_Noise3 = 0x1b56c4e9;

	public static float NextFloat(uint last, uint seed, float min, float max)
	{
		return (float)(((double)NextRand(last, seed) / uint.MaxValue) * (max - min) + min);
	}
	
	public static uint NextRand(uint n, uint seed = 0)
	{
		n *= k_Noise1;
		n += seed;
		n ^= n >> 8;
		n += k_Noise2;
		n ^= n << 8;
		n *= k_Noise3;
		n ^= n >> 8;
		return n;
	}
}
