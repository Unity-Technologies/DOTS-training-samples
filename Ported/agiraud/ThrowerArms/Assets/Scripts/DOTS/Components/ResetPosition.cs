using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[Serializable]
public struct ResetPosition : IComponentData
{
    public bool needReset;
}
