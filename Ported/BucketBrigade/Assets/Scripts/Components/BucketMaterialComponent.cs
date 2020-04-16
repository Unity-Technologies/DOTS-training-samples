using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using Unity.Rendering;

[MaterialProperty("_BucketAmount", MaterialPropertyFormat.Float)][GenerateAuthoringComponent]
public struct BucketMaterialComponent : IComponentData
{
    public float Amount;
}
