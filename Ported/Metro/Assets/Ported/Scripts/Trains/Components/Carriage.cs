using System;
using Unity.Entities;

namespace MetroECS.Trains
{
    [GenerateAuthoringComponent]
    public struct Carriage : IComponentData
    {
        public const float LENGTH = 5f;
        public const int CAPACITY = 10;
        public const float SPACING = 0.25f;
    
        public int ID;
        public Entity Train;
    }
}