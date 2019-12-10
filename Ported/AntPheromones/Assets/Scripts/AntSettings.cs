using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct AntSettings : IComponentData
{
	//public Material basePheromoneMaterial;
	//public Renderer pheromoneRenderer;
	//public Material antMaterial;
	//public Material obstacleMaterial;
	//public Material resourceMaterial;
	//public Material colonyMaterial;
	//public Mesh antMesh;
	//public Mesh obstacleMesh;
	//public Mesh colonyMesh;
	//public Mesh resourceMesh;
	public Color searchColor;
	public Color carryColor;
	public int antCount;
	[DefaultValue(128)]
	public int mapSize;
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
	[DefaultValue(360)]
	public int rotationResolution;
	public int obstacleRingCount;
	[Range(0f, 1f)]
	public float obstaclesPerRing;
	public float obstacleRadius;
}
