using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Collections;

//Buffer for all of the pheremones data
public struct PheromonesBufferData : IBufferElementData
{
    public static implicit operator float(PheromonesBufferData e) { return e.Value; }
    public static implicit operator PheromonesBufferData(float e) { return new PheromonesBufferData { Value = e }; }

    public float Value;
}
