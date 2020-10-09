using System;
using Unity.Mathematics;

namespace Assets.Scripts.BlobData
{
    public struct ColorMask
    {
        [Flags]
        public enum Masks
        {
            None,
            ColorA = (1 << 0),
            ColorB = (1 << 1),
            ColorC = (1 << 2),
            ColorD = (1 << 3), 
            ColorE = (1 << 4), 
            ColorF = (1 << 5), 
            ColorG = (1 << 6),
            ColorH = (1 << 7),
            ColorI = (1 << 8),
            ColorJ = (1 << 9),
            ColorK = (1 << 10),
            ColorL = (1 << 11)
        }
        
        public static (float4, Masks) ColorA = (new float4(0, 0, 1, 1), Masks.ColorA);         //Blue
        public static (float4, Masks) ColorB = (new float4(.5f, 0, 1, 1), Masks.ColorB);       //Purple
        public static (float4, Masks) ColorC = (new float4(1, 0, 0, 1), Masks.ColorC);         //Red
        public static (float4, Masks) ColorD = (new float4(1, 0, .7f, 1), Masks.ColorD);       //Pink
        public static (float4, Masks) ColorE = (new float4(0, .4f, 1, 1), Masks.ColorE);       //Light Blue
        public static (float4, Masks) ColorF = (new float4(0, .7f, 1, 1), Masks.ColorF);       //Lightest Blue
        public static (float4, Masks) ColorG = (new float4(0, .5f, 0, 1), Masks.ColorG);       //Green
        public static (float4, Masks) ColorH = (new float4(0, 1, 0, 1), Masks.ColorH);         //Light Green
        public static (float4, Masks) ColorI = (new float4(1, 1, 1, 1), Masks.ColorI);         //White
        public static (float4, Masks) ColorJ = (new float4(0, 0, 0, 1), Masks.ColorJ);         //Black
        public static (float4, Masks) ColorK = (new float4(1, 1, 0, 1), Masks.ColorK);         //Yellow
        public static (float4, Masks) ColorL = (new float4(1, .5f, 0, 1), Masks.ColorL);       //Orange

        public static float4 invalidColor = new float4(.5f, .5f, .5f, 1); //Gray
        
        private static readonly (float4, Masks)[] _allMasks = { ColorA, ColorB, ColorC, ColorD, ColorE, ColorF, ColorG, ColorH, ColorI, ColorJ, ColorK, ColorL };

        public static Masks GetMask(float4 color)
        {
            foreach(var mask in _allMasks)
            {
                if (mask.Item1.Equals(color))
                    return mask.Item2;
            }
            return Masks.None;
        }

        public static float4 GetColor(Masks mask)
        {
            foreach (var color in _allMasks)
            {
                if (color.Item2 == mask)
                    return color.Item1;
            }
            
            return invalidColor;
        }

        //Takes somewhere from 0 - 11
        public static Masks GetMaskFromInt(int value)
        {
            var mask = (Masks) (1 << value);
            return mask;
        }
    }
}
