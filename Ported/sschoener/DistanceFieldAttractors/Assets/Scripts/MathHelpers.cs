using Unity.Mathematics;
using static Unity.Mathematics.math;

namespace Systems
{
    public static class MathHelpers
    {
        public static quaternion LookRotationWithUp(float3 forward)
        {
            float3 t = normalize(new float3(forward.z, 0, -forward.x));
            return quaternion(float3x3(t, cross(forward, t), forward));
        }
    }
}
