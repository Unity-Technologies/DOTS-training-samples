using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
// Tag used to sort Rocks/Cans that are available to be grabbed or thrown at
public struct Available : IComponentData
{
    // If a rock/can has been picked during a frame, we need to set JustPicked to true to notify
    // other arms in the foreach loop that the rock/can isn't available anymore.
    // (because removing the Available component through ECB will only be effective at the end of the frame,
    // so other arms in the foreach loop don't see the Available component of the rock/can has been removed)
    public bool JustPicked;
}
