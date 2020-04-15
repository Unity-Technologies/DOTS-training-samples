using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
[GenerateAuthoringComponent]
public struct ValueComponent : IComponentData
{
    public byte Value;
}
