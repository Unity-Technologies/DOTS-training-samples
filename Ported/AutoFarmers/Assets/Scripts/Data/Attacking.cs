using System;
using Unity.Entities;

namespace AutoFarmers
{
    [GenerateAuthoringComponent]
    struct Attacking : IComponentData, IComparable<Attacking>
    {
        public Entity Target;

        public int CompareTo(Attacking other)
        {
            return Target.CompareTo(other.Target);
        }
    }
}