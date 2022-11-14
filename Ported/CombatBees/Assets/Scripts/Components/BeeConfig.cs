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

        public Team Team1;
        public Team Team2;
    }

    [System.Serializable]
    public struct Team 
    {
        public float3 MinBounds;
        public float3 MaxBounds;
        public float4 Color;
        public float TeamAttraction;
    }
}
