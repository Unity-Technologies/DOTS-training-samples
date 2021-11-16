using Unity.Mathematics;

namespace Dots
{
    public class TornadoUtils
    {
        public static float TornadoSway(float y, float time)
        {
            return math.sin(y / 5f + time / 4f) * 3f;
        }
    }
}