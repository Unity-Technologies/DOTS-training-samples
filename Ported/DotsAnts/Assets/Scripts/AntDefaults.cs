using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntDefaults : MonoBehaviour
{
    public int antCount = 1000;
    public int mapSize = 128;
    public int bucketResolution = 64;
    public Vector3 antSize;
    public float antSpeed = 0.2f;
    
    [Range(0f,1f)]
    public float antAccel = 0.07f;
    public float trailAddSpeed = 0.3f;
    public float trailDecay = 0.9985f;
    public float randomSteering = 0.14f;
    public float pheromoneSteerStrength = 0.015f;
    public float wallSteerStrength = 0.12f;
    public float goalSteerStrength = 0.04f;
    public float outwardStrength = 0.003f;
    public float inwardStrength = 0.003f;
    public int rotationResolution = 360;
    public int obstacleRingCount = 3;
    public float obstaclesPerRing = 0.8f;
    public float obstacleRadius = 2.0f;

    public Texture2D pheromoneMap;
}
