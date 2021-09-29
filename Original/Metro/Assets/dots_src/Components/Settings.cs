using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct Settings : IComponentData
{
    public float MaxSpeed;
    public float CarriageSizeWithMargins;
    public float TimeAtStation;
}
