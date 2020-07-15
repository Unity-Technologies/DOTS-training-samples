using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class BeeManagerDOTS : MonoBehaviour, IConvertGameObjectToEntity
{
    public Entity Prefab;
    public int Count;
    public Color[] teamColors;
    public Vector3[] baseLocations;
    public int BeeCount;
    public float minBeeSize;
    public float maxBeeSize;
    public float speedStretch;
    public float rotationStiffness;
    [Space(10)]
    [Range(0f, 1f)]
    public float aggression;
    public float flightJitter;
    public float teamAttraction;
    public float teamRepulsion;
    [Range(0f, 1f)]
    public float damping;
    public float chaseForce;
    public float carryForce;
    public float grabDistance;
    public float attackDistance;
    public float attackForce;
    public float hitDistance;
    public float maxSpawnSpeed;
    [Space(10)]
    public int startBeeCount;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
    }

}
