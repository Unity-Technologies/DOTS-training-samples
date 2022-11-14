using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Authoring
{
    public class BeeAuthoring : MonoBehaviour
    {
        public float3 Velocity;
        public int Team;
        public float4 Color;
    }

    public class BeeBaker : Baker<BeeAuthoring>
    {
        public override void Bake(BeeAuthoring authoring)
        {
            AddComponent<Bee>();
        }
    }
}