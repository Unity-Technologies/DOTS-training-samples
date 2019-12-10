using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour
{
	public Material basePheromoneMaterial;
	public Renderer pheromoneRenderer;
	public Material antMaterial;
	public Material obstacleMaterial;
	public Material resourceMaterial;
	public Material colonyMaterial;
	public Mesh antMesh;
	public Mesh obstacleMesh;
	public Mesh colonyMesh;
	public Mesh resourceMesh;
	public Color searchColor;
	public Color carryColor;
	public int antCount;
	public int mapSize = 128;
	public int bucketResolution;
	public Vector3 antSize;
	public float antSpeed;
	[Range(0f, 1f)]
	public float antAccel;
	public float trailAddSpeed;
	[Range(0f, 1f)]
	public float trailDecay;
	public float randomSteering;
	public float pheromoneSteerStrength;
	public float wallSteerStrength;
	public float goalSteerStrength;
	public float outwardStrength;
	public float inwardStrength;
	public int rotationResolution = 360;
	public int obstacleRingCount;
	[Range(0f, 1f)]
	public float obstaclesPerRing;
	public float obstacleRadius;
}
