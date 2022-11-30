using System.Runtime.CompilerServices;
using Unity.Mathematics;

namespace Systems
{
    public static class Utility
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Angle(float3 from, float3 to)
        {
            float num = math.sqrt(math.lengthsq(from) * math.lengthsq(to));
            return num < 1.0000000036274937E-15 ? 0.0f : math.acos( math.clamp(math.dot(from, to) / num, -1f, 1f));
        }
    }
}