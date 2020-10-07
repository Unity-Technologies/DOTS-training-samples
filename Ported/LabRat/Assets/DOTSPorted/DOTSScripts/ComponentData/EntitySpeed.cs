using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct EntitySpeed : IComponentData
{
    public float speed;
}
