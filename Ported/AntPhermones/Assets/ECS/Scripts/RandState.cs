using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct RandState : IComponentData
{
    public Unity.Mathematics.Random Random;
}
