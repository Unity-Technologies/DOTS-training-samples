
using Unity.Mathematics;

namespace Assets.Scripts
{
    internal class utils
    {

        //inline ??
        public static int ToIndex(int x, int y, int z, int size)
        {
            return x * size * size + y * size + z;
        }

        public static float Magnitude(float3 vector)
        {
            return math.sqrt(vector.x * vector.x + vector.y * vector.y + vector.z * vector.z);
        }

        public static float3 Normalize(float3 vector)
        {
            return vector / Magnitude(vector);
        }
    }
}
