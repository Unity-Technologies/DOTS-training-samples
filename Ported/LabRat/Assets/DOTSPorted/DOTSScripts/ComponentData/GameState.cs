using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct GameState : IComponentData
{
    public int timer;
    public float collisionRadius;
}
