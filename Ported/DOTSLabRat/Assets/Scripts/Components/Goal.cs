using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace DOTSRATS
{
    [GenerateAuthoringComponent]
    public struct Goal : IComponentData
    {
        [HideInInspector] public int playerNumber;
    }
}