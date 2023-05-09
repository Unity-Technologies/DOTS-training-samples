using UnityEngine;
using Unity.Entities;
using UnityEngine.Serialization;

public class AntSpawnerAuthority : MonoBehaviour
{
    public int SpawnCount;
    public GameObject Prefab;

    class Baker : Baker<AntSpawnerAuthority>
    {
        public override void Bake(AntSpawnerAuthority authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new AntSpawner()
            {
                Count = authoring.SpawnCount, 
                Prefab = GetEntity(authoring.Prefab, TransformUsageFlags.None)
            });
        }
    }
}
