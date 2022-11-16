using Unity.Entities;
using Unity.Mathematics;

namespace Components
{
    public struct BeeConfig : IComponentData
    {
        public Entity BeePrefab;
        public int BeesToSpawn;
        public float MinBeeSize;
        public float MaxBeeSize;
        public float Stretch;

        public Team Team1;
        public Team Team2;
    }

    [System.Serializable]
    public struct Team
    {
        public int TeamNumber;
        public float3 MinBounds;
        public float3 MaxBounds;
        public float4 Color;
        public float TeamAttraction;
        public float TeamAggression;
        public float AttackDistance;
        public float ChaseForce;
        public float AttackForce;
        public float HitDistance;
    }
}
