using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
[GenerateAuthoringComponent]
public struct FireMaterialComponent : IComponentData
{
    public Color FireColor;
    public Color GrassColor;
    public float Amount;
}
