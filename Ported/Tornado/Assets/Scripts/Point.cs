using System;
using Unity.Entities;
using UnityEngine;

struct Points
{
    public int count;
    public int capacity;

    public Vector3[] pos;
    public Vector3[] old;
    public bool[] anchor;
    public int[] neighbors;

    public Points(int capacity)
    {
        count = 0;
        this.capacity = capacity;
        pos = new Vector3[capacity];
        old = new Vector3[capacity];
        anchor = new bool[capacity];
        neighbors = new int[capacity];
    }

    public void Add(in Point point)
    {
        var index = count++;
        old[index] = pos[index] = point.position;
        anchor[index] = point.anchor;
        neighbors[index] = point.neighborCount;
    }
}

class Point// : IComponentData
{
    public readonly Vector3 position;
    public readonly bool anchor;
    public int neighborCount;

    public Point(Point other, int neighborCount)
    {
        position = other.position;
        anchor = other.anchor;
        this.neighborCount = neighborCount;
    }

    public Point(float x, float y, float z)
        : this(x, y, z, false)
    {
    }

    public Point(float x, float y, float z, bool anchor)
    {
        position = new Vector3(x, y, z);
        this.anchor = anchor;
        neighborCount = 0;
    }
}
