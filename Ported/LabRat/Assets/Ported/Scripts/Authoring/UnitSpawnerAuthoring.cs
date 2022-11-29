using UnityEngine;
using Unity.Entities;

public class UnitSpawnerAuthoring : MonoBehaviour
{
    public UnityEngine.GameObject spawnPrefab;
    public Transform spawnPoint;
    public int max;
    public float frequency;
}

class UnitSpawnerBaker : Baker<UnitSpawnerAuthoring>
{
    public override void Bake(UnitSpawnerAuthoring authoring)
    {
        AddComponent(new UnitSpawnerComponent
        {
            max = authoring.max,
            frequency = authoring.frequency,
            counter = 0.0f,
            spawnObject = GetEntity(authoring.spawnPrefab),
            spawnPoint = GetEntity(authoring.spawnPoint)
        });
    }
}