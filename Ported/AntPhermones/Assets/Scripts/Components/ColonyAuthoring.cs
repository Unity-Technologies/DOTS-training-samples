using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;


[Serializable]
public struct Colony: IComponentData
{
    public float antSize;
    public float antTargetSpeed;
    public float antAccel;
    public float pheromoneDecayRate;
    public float obstacleSize;
    public float randomSteering;
    public float pheromoneSteerStrength;
    public float wallSteerStrength;
    public Entity obstaclePrefab;
}

public class ColonyAuthoring : MonoBehaviour
{
    public Colony colony;
    public GameObject obstaclePrefab;
    class Baker : Baker<ColonyAuthoring>
    {
        public override void Bake(ColonyAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Renderable);
            var colony = authoring.colony;

            colony.obstaclePrefab = GetEntity(authoring.obstaclePrefab, TransformUsageFlags.Renderable); 
            AddComponent<Colony>(entity, colony);
        }
    }
}
