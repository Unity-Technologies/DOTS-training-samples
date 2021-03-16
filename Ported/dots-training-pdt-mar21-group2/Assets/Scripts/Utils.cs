using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct Utils
{
    static public bool WorldIsOutOfBounds(Unity.Mathematics.float3 position, float width, float ground)
    {
        return position.x < 0.0f ||
                position.x > width ||
                position.y < ground;
    }
}