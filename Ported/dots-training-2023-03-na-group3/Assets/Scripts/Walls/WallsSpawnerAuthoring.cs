using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class WallsSpawnerAuthoring : MonoBehaviour
{
    public float radius;
    public float numSpheres;
    public float3 position;
    public GameObject environmentPrefab;
    
    public class EnvironmentSpawnerBaker : Baker<WallsSpawnerAuthoring>
    {
        public override void Bake(WallsSpawnerAuthoring authoring)
        {
            AddComponent(new WallsSpawner
            { 
                environmentPrefab = GetEntity(authoring.environmentPrefab),
                radius = authoring.radius,
                position = authoring.position,
                numSpheres = authoring.numSpheres
            });
        }
    }
}

