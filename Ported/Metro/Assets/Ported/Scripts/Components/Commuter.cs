using Unity.Entities;
using UnityEngine;

namespace MetroECS.Comuting
{
    [GenerateAuthoringComponent]
    public struct Commuter : IComponentData
    {
        [HideInInspector]
        public Entity Seat;
        public float movementSpeed;
    }
}