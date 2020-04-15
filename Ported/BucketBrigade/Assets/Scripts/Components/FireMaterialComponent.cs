using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using Unity.Rendering;

[MaterialProperty("_Amount", MaterialPropertyFormat.Float)]
public struct FireMaterialComponent : IComponentData
{
    public float Amount;
}
