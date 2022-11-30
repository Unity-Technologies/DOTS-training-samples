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
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float SignedAngle(float3 from, float3 to, float3 axis)
        {
            float num1 = Angle(from, to);
            float num2 = from.y * to.z - from.z * to.y;
            float num3 = from.z * to.x - from.x * to.z;
            float num4 = from.x * to.y - from.y * to.x;
            float num5 = math.sign(axis.x *  num2 + axis.y * num3 + axis.z * num4);
            return num1 * num5;
        }
    }
}