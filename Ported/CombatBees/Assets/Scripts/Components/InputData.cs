using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

[Serializable]
[GenerateAuthoringComponent]
public struct InputData : IComponentData
{
    public KeyCode spaceKey;
    public MouseButton mouseButton;
}
