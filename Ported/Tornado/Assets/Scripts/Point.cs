using System;
using UnityEngine;

class Point
{
    public Vector3 position;
    public Vector3 oldPosition;

    public bool anchor;
    public int neighborCount;

    public float x { get => position.x; set => position.x = value; }
    public float y { get => position.y; set => position.y = value; }
    public float z { get => position.z; set => position.z = value; }

    public Point(Point other, int neighborCount)
    {
        position = other.position;
        oldPosition = other.position;

        anchor = other.anchor;
        this.neighborCount = neighborCount;
    }

    public Point(float x, float y, float z)
        : this(x, y, z, false)
    {
    }

    public Point(float x, float y, float z, bool anchor)
    {
        oldPosition = position = new Vector3(x, y, z);
        this.anchor = anchor;
        neighborCount = 0;
    }

    public void Damp(in float invDamping)
    {
        position += (position - oldPosition) * invDamping;
    }

    public void SetPrevious(in float startX, in float startY, in float startZ)
    {
        oldPosition = new Vector3(startX, startY, startZ);
    }

    internal void SetPrevious(in Vector3 v)
    {
        oldPosition = v;
    }
}
