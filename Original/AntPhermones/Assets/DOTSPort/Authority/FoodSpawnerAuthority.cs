using UnityEngine;
using Unity.Entities;


public class FoodSpawnerAuthority : MonoBehaviour
{
    public int SpawnCount;
    public GameObject Prefab;

    class Baker : Baker<FoodSpawnerAuthority>
    {
        public override void Bake(FoodSpawnerAuthority authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new FoodSpawner()
            {
                Count = authoring.SpawnCount,
                Prefab = GetEntity(authoring.Prefab, TransformUsageFlags.None)
            });
        }
    }
}
