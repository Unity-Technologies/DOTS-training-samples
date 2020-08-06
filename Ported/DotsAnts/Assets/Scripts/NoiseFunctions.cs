using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class Noise
{
    // Fowler-Noll-Vo hash function                          
    static public uint FnvHash(byte[] bytes)
    {                                                                
        uint h = 2166136261;
        for (int i = 0; i < bytes.Length; i++)
        {
            h = (h * 16777619) ^ bytes[i];
        }
        return h;           
    }

    static public uint HashCombine(uint a, uint b)
    {
        return a ^ (b << 1);
    }

    static public float HashNoise(int x, int y, int octave, int scramble)
    {
        uint hash = FnvHash(BitConverter.GetBytes(x));
        hash = HashCombine(hash, FnvHash(BitConverter.GetBytes(y)));
        hash = HashCombine(hash, FnvHash(BitConverter.GetBytes(octave)));
        int seed = (int)HashCombine(hash, FnvHash(BitConverter.GetBytes(scramble)));

        int div = seed / 44488;        // max: M / Q = A = 48,271
        int rem = seed % 44488;        // max: Q - 1     = 44,487
        
        int s = rem * 48271;   // max: 44,487 * 48,271 = 2,147,431,977 = 0x7fff3629
        int t = div * 3399;    // max: 48,271 *  3,399 =   164,073,129
        seed = s - t;
        
        if (seed < 0) { seed += 0x7fffffff; }
       
        return (float)seed / (float)2147483647;
    }

    static private float SineBlend(float n0, float n1, float alpha)
    {
        return n0 + (n1 - n0) * (float)(Math.Sin(Math.PI * (alpha - 0.5)) * 0.5 + 0.5);
    }

    static public float PerlinNoise(int x, int y, float scale, int numOctaves, float persistence, float exponent, int scramble)
    {
        float2 position = new float2(x / scale, y / scale);

        float sumNoise = 0.0f;
        float amplitude = 1.0f;
        float sumAmplitudes = 0.0f;
        for (int octave = 0; octave < numOctaves; octave++)
        {
            int2 intPos = (int2)position;
            float2 deltaPos = position - (float2)intPos;

            float n00 = HashNoise(intPos.x, intPos.y, octave, scramble);
            float n10 = HashNoise(intPos.x + 1, intPos.y, octave, scramble);
            float n01 = HashNoise(intPos.x, intPos.y + 1, octave, scramble);
            float n11 = HashNoise(intPos.x + 1, intPos.y + 1, octave, scramble);

            sumNoise += SineBlend(SineBlend(n00, n10, deltaPos.x), SineBlend(n01, n11, deltaPos.x), deltaPos.y) * amplitude;

            sumAmplitudes += amplitude;
            amplitude *= persistence;
            position *= 2;
        }

        return (float)Math.Pow(sumNoise / sumAmplitudes, exponent);
    }
}
