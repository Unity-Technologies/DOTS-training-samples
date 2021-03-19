using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
/// <summary>
/// animating the hand means traveling from <seealso cref="AnimStartPosition"/> to
/// <seealso cref="TargetPosition"/> in function of <seealso cref="Timer"/>
/// </summary>
[GenerateAuthoringComponent, Serializable]
public struct AnimStartPosition : IComponentData
{
    public float3 Value;
    public float StartHandStrength;
}
