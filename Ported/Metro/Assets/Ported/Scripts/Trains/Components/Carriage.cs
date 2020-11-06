using System;
using Unity.Entities;
using UnityEngine;

namespace MetroECS.Trains
{
    [GenerateAuthoringComponent]
    public struct Carriage : IComponentData
    {
        public const float LENGTH = 5f;
        public const int CAPACITY = 10;
        public const float SPACING = 0.25f;
    
        [HideInInspector]
        public int ID;
        [HideInInspector]
        public float Position;
        [HideInInspector]
        public Entity Train;
        
        public Entity TrainDoorsLeft;
        public Entity TrainDoorsRight;
    }
}