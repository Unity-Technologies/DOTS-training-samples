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
        public Team Team1;
        public Team Team2;
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
                MaxBeeSize = authoring.MaxBeeSize,
                Team1 = authoring.Team1,
                Team2 = authoring.Team2
            });
        }
    }
        
}