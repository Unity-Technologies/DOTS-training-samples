using UnityEngine;
using Unity.Entities;

public class UnitSpawnerAuthoring : MonoBehaviour
{
    public UnityEngine.GameObject spawnPrefab;
    public Transform spawnPoint;
    public MovementDirection startDirection;
    public int max;
    public float frequency;
    public float minSpeed = 1.0f;
    public float maxSpeed = 2.0f;
}

class UnitSpawnerBaker : Baker<UnitSpawnerAuthoring>
{
    public override void Bake(UnitSpawnerAuthoring authoring)
    {
        AddComponent(new UnitSpawnerComponent
        {
            max = authoring.max,
            frequency = authoring.frequency,
            counter = authoring.frequency - (authoring.frequency / 1000.0f),
            spawnObject = GetEntity(authoring.spawnPrefab),
            spawnPoint = GetEntity(authoring.spawnPoint),
            startDirection = authoring.startDirection,
            minSpeed = authoring.minSpeed,
            maxSpeed = authoring.maxSpeed,
        });
    }
}