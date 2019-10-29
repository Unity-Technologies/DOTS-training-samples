#if !UNITY_DISABLE_MANAGED_COMPONENTS

using System;
using UnityEngine;

namespace Unity.Entities
{
    [UnityEngine.AddComponentMenu("")]
    class CompanionLink : MonoBehaviour
    {
        public static string GenerateCompanionName(Entity entity) => $"Companion of {entity}";
    }

    class CompanionLinkSystemState : ISystemStateComponentData, IEquatable<CompanionLinkSystemState>
    {
        public GameObject Link;

        public bool Equals(CompanionLinkSystemState other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(Link, other.Link);
        }

        public override int GetHashCode()
        {
            return ReferenceEquals(Link, null) ? 0 : Link.GetHashCode();
        }
    }
}

#endif
