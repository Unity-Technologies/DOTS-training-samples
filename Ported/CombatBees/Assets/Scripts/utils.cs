using Unity.Mathematics;
using UnityEditor.UIElements;

namespace Utils
{
    public static class utilityExtensions
    {
        public static float3 Floored(this float3 f)
        {
            return new float3(f.x, 0.5f, f.z);
        }
    }
}