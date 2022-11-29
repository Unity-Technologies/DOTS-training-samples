using UnityEngine;
using Unity.Entities;

public class UnitSpawnerAuthoring : MonoBehaviour
{
    public GameObject spawnPrefab;
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
            //spawnObject = authoring.spawnPrefab,
        });



        /*
         *        AddComponent(new Turret
            {
                // By default, each authoring GameObject turns into an Entity.
                // Given a GameObject (or authoring component), GetEntity looks up the resulting Entity.
                CannonBallPrefab = GetEntity(authoring.CannonBallPrefab),
                CannonBallSpawn = GetEntity(authoring.CannonBallSpawn)
            });
         */
    }
}