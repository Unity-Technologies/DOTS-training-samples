using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

unsafe struct Neighbors
{
    public fixed ushort Intersections[4];
    public fixed ushort Splines[4];
    public byte Count;

    public void Add(ushort intersection, ushort spline)
    {
        Debug.Assert(Count < 4);
        Intersections[Count] = intersection;
        Splines[Count] = spline;
        Count++;
    }

    public int IndexOfSpline(ushort spline)
    {
        for (int i = 0; i < Count; i++)
        {
            if (Splines[i] == spline)
                return i;
        }
        return -1;
    }
}

struct Intersection
{
    public float3 Position;
    public float3 Normal;
    public Neighbors Neighbors;
}

struct IntersectionsBlob
{
    public BlobArray<Intersection> Intersections;

    public static BlobAssetReference<IntersectionsBlob> Instance;
}