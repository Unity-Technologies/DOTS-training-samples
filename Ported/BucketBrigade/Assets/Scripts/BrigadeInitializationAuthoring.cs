using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;


// this isn't correct, we need to have arrays 
// of empty passers / full passers
[GenerateAuthoringComponent]
public struct BrigadeInitialization : IComponentData
{
    public Entity bot;
    public float botSpeed;
    public int emptyPassers;
    public int fullPassers;
    public int brigadeCount;
}