using Unity.Entities;
using Unity.Mathematics;

namespace Aspects
{
    readonly partial struct HiveAspect : IAspect
    {
        private readonly RefRO<Hive> Hive;

        public float4 color => Hive.ValueRO.color;
    }
}