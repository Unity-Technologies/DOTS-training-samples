using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace AutoFarmers
{
    [GenerateAuthoringComponent]
    public struct CellPosition : IComponentData
    {
        public int2 Value;

        public static float3 ToTranslation(int2 Value)
        {
            return new float3(Value.x, 0.0f, Value.y) + new float3(0.5f, 0.0f, 0.5f);
        }

        public Translation ToTranslation()
        {
            return new Translation { Value = ToTranslation(Value) };
        }

        public static int2 FromTranslation(float3 t)
        {
            var bias = t - new float3(0.5f, 0.0f, 0.5f);
            return new int2((int)bias.x, (int)bias.z);
        }

        public void FromTranslation(Translation t)
        {
            Value = FromTranslation(t.Value);
        }
    }
}