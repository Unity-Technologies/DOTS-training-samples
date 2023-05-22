using Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

    public class SpawnerAuthoring : MonoBehaviour
    {
        public GameObject prefab;
        public int initialSpawnAmount;
    }

    public class SpawnerBaker : Baker<SpawnerAuthoring>
    {
        public override void Bake(SpawnerAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);

            AddComponent(entity, new SpawnerComponent
            {
                initialSpawnAmount = authoring.initialSpawnAmount,
                beePrefab = GetEntity(authoring.prefab, TransformUsageFlags.Dynamic)
            });
            
            AddComponent(entity, new LocalTransform
            {
                Position = authoring.transform.position,
                Rotation = authoring.transform.rotation,
                Scale = authoring.transform.localScale.magnitude
            });
        }
    }
