using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public struct AntData : IComponentData
{
	public float facingAngle;
	public float speed;
	public bool holdingResource;
	public float brightness;
	public AntData(ref Random random)
	{
		facingAngle = random.NextFloat() * math.PI * 2f;
		speed = 0f;
		holdingResource = false;
		brightness = random.NextFloat(.75f, 1.25f);
	}
}

public struct Excitement : IComponentData
{
	public float Value;
}

struct InitializeAnts : IComponentData
{
	public Entity AntPrefab;
	public Entity ObstaclePrefab;
	public Entity ColonyPrefab;
	public Entity ResourcePrefab;
	public int ObstacleRingCount;
	public uint RandomSeed;
	public float ObstaclesPerRing;
	public int AntCount;
}

public struct ObstacleData : IComponentData
{
}
public struct Resource : IComponentData
{

}
public struct Colony : IComponentData
{

}
[InternalBufferCapacity(8)]
public struct NearbyObstacle : IBufferElementData
{
	public float2 Position;
}


public struct AntParams : IComponentData 
{
	public float AntSpeed;
	public float RandomSteering;
	public int MapSize;
	public float PheromoneSteerStrength;
	public float WallSteerStrength;
	public float AntAccel;
	public float4 SearchColor;
	public float4 CarryColor;
	public float GoalSteerStrength;
	public float ObstacleRadius;
	public float OutwardStrength;
	public float InwardStrength;
	public float TrailAddSpeed;
}

public struct PheromoneTexture : ISharedComponentData, System.IEquatable<PheromoneTexture>
{
	public Texture2D Value;

    public override bool Equals(object obj)
    {
        return obj is PheromoneTexture texture && Equals(texture);
    }

    public bool Equals(PheromoneTexture other)
    {
        return EqualityComparer<Texture2D>.Default.Equals(Value, other.Value);
    }

    public override int GetHashCode()
    {
        return -1937169414 + EqualityComparer<Texture2D>.Default.GetHashCode(Value);
    }

    public static bool operator ==(PheromoneTexture left, PheromoneTexture right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(PheromoneTexture left, PheromoneTexture right)
    {
        return !(left == right);
    }
}

