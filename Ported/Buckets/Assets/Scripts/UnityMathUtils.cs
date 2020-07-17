
using Unity.Mathematics;

namespace Utils
{
    public static class UnityMathUtils
    {
        /// <summary>
        /// Lerps between two float2s
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="alpha"></param>
        /// <returns></returns>
        public static float2 Lerp(float2 from, float2 to, float alpha)
        {
            float newX = math.lerp(from.x, to.x, alpha);
            float newY = math.lerp(from.y, to.y, alpha);
            return new float2(newX, newY);
        }

        /// <summary>
        /// Lerps between two float3s
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="alpha"></param>
        /// <returns></returns>
        public static float3 Lerp(float3 from, float3 to, float alpha)
        {
            float newX = math.lerp(from.x, to.x, alpha);
            float newY = math.lerp(from.y, to.y, alpha);
            float newZ = math.lerp(from.z, to.z, alpha);
            return new float3(newX, newY, newZ);
        }

        /// <summary>
        /// Lerps between two float4s
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="alpha"></param>
        /// <returns></returns>
        public static float4 Lerp(float4 from, float4 to, float alpha)
        {
            float newX = math.lerp(from.x, to.x, alpha);
            float newY = math.lerp(from.y, to.y, alpha);
            float newZ = math.lerp(from.z, to.z, alpha);
            float newW = math.lerp(from.w, to.w, alpha);
            return new float4(newX, newY, newZ, newW);
        }

        /// <summary>
        /// Compares two floating point values and returns true if they are similar.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="epsilon"></param>
        public static bool Approximately(float a, float b, float epsilon)
        {
            return (double)math.abs(b - a) < (double)math.max(1E-06f * math.max(math.abs(a), math.abs(b)), epsilon * 8f);
        }
    }
}
