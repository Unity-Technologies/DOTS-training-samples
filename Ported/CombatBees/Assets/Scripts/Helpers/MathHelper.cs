using Unity.Mathematics;
using UnityEngine;
using Color = System.Drawing.Color;
using Random = Unity.Mathematics.Random;

namespace Helpers
{
    public static class MathHelper
    {
        public static float3 NextInsideSphere(this ref Random rand)
        {
            var phi = rand.NextFloat(2 * math.PI);
            var theta = math.acos(rand.NextFloat(-1f, 1f));
            var r = math.pow(rand.NextFloat(), 1f / 3f);
            var x = math.sin(theta) * math.cos(phi);
            var y = math.sin(theta) * math.sin(phi);
            var z = math.cos(theta);
            return r * new float3(x, y, z);
        }
 
        public static float3 NextOnSphereSurface(this ref Random rand)
        {
            var phi = rand.NextFloat(2 * math.PI);
            var theta = math.acos(rand.NextFloat(-1f, 1f));
            var x = math.sin(theta) * math.cos(phi);
            var y = math.sin(theta) * math.sin(phi);
            var z = math.cos(theta);
            return new float3(x, y, z);
        }

        public static float Range(this ref Random rand, float min, float max)
        {
            return min + rand.NextFloat() * (max - min);
        }

        public static Vector4 ColorHSV(this ref Random rand, float hueMin,
            float hueMax,
            float saturationMin,
            float saturationMax,
            float valueMin,
            float valueMax)
        {
            return new Vector4(Range(ref rand, hueMin, hueMax), 
                Range(ref rand, saturationMin, saturationMax),
                Range(ref rand, valueMin, valueMax),
                1f);
        }
        
    }
}