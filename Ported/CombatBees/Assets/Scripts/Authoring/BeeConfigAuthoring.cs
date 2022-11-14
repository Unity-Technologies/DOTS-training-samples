using Components;
using Unity.Entities;
using UnityEngine;

namespace Authoring
{
    public class BeeConfigAuthoring : MonoBehaviour
    {
        public GameObject BeePrefab;
        public int BeesToSpawn;
        public float MinBeeSize;
        public float MaxBeeSize;
    }

    public class BeeConfigBaker : Baker<BeeConfigAuthoring>
    {
        public override void Bake(BeeConfigAuthoring authoring)
        {
            AddComponent(new BeeConfig
            {
                BeePrefab = GetEntity(authoring.BeePrefab),
                BeesToSpawn = authoring.BeesToSpawn,
                MinBeeSize = authoring.MinBeeSize,
                MaxBeeSize = authoring.MaxBeeSize
            });
        }
    }
        
}