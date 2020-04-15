using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct InitData : IComponentData
{

    public Entity FirePrefab;
    //public  GridBoundingBox;//maybe read bounds from Translation
}
