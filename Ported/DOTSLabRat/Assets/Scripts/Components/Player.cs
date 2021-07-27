using Unity.Entities;
using UnityEngine;

namespace DOTSRATS
{
    [GenerateAuthoringComponent]
    public struct Player : IComponentData
    {
        public int playerNumber;
        public UnityEngine.Color color;
        [HideInInspector] public int score;
    }
}