using System;
using Unity.Mathematics;

namespace Assets.Scripts.BlobData
{
    public struct ColorMask
    {
        [Flags]
        public enum Masks
        {
            None = 0,
            ColorA = 1,
            ColorB = 2,
            ColorC = 3,
            ColorD = 4, 
            ColorE = 5, 
            ColorF = 6, 
        }
        
        public static (float4, Masks) ColorA = (new float4(0, 0, 0, 1), Masks.ColorA);
        public static (float4, Masks) ColorB = (new float4(1, 1, 1, 1), Masks.ColorB);
        public static (float4, Masks) ColorC = (new float4(0, 0, 1, 1), Masks.ColorC); 
        public static (float4, Masks) ColorD = (new float4(0, 1, 0, 1), Masks.ColorD);
        public static (float4, Masks) ColorE = (new float4(1, 0, 0, 1), Masks.ColorE);
        public static (float4, Masks) ColorF = (new float4(1, 0, 1, 1), Masks.ColorF);

        private static readonly (float4, Masks)[] _allMasks = { ColorA, ColorB, ColorC, ColorD, ColorE, ColorF };

        public static Masks GetMask(float4 color)
        {
            foreach((float4, Masks) mask in _allMasks)
            {
                if (mask.Item1.Equals(color))
                    return mask.Item2;
            }
            return Masks.None;
        }
    }
}
