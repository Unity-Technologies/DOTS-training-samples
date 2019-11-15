using System;
using UnityEngine;

public struct Orbiter
{
    public Vector3 position;
    public Vector3 velocity;
    public Vector4 color;

    public Orbiter(Vector3 pos)
    {
        position = pos;
        velocity = Vector3.zero;
        color = Color.black;
    }
}
