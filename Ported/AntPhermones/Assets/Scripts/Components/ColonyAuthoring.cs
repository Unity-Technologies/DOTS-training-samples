using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public struct Colony: IComponentData
{
    public float antSize;
    public float antTargetSpeed;
    public float antAccel;
    [FormerlySerializedAs("nbAnts")]
    public int AntCount;
    public Vector3 AntSize;
    
    public float pheromoneDecayRate;
    public float obstacleSize;
    public float randomSteering;
    public float pheromoneSteerStrength;
    public float wallSteerStrength;

    public float mapSize;

    public Entity homePrefab;
    public Entity obstaclePrefab;
    public Entity antPrefab;
    
}

public class ColonyAuthoring : MonoBehaviour
{
    public Colony colony;
    public GameObject homePrefab;
    public GameObject obstaclePrefab;
    public GameObject antPrefab;
    class Baker : Baker<ColonyAuthoring>
    {
        public override void Bake(ColonyAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Renderable);
            var colony = authoring.colony;

            colony.homePrefab = GetEntity(authoring.homePrefab, TransformUsageFlags.Renderable);
            colony.obstaclePrefab = GetEntity(authoring.obstaclePrefab, TransformUsageFlags.Renderable);
            colony.antPrefab = GetEntity(authoring.antPrefab, TransformUsageFlags.Renderable);
            AddComponent<Colony>(entity, colony);
        }
    }
}
