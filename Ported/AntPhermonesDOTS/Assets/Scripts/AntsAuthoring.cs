using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntsAuthoring : MonoBehaviour
{
    public Material antMaterial;
    public Mesh antMesh;
    public Color searchColor;
    public Color carryColor;
    public Vector3 antSize;
    public float antSpeed;
    [Range(0f,1f)]
    public float antAccel;
    public float randomSteering;
    public float pheromoneSteerStrength;
    public float wallSteerStrength;
    public float goalSteerStrength;
    public float outwardStrength;
    public float inwardStrength;
    public int rotationResolution = 360;
}
