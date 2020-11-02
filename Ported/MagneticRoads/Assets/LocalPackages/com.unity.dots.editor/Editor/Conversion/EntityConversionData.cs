using System;
using System.Collections.Generic;
using Unity.Entities.Conversion;

namespace Unity.Entities.Editor
{
    struct EntityConversionData : IDisposable, IEquatable<EntityConversionData>
    {
        public static readonly EntityConversionData Null = default;

        public Entity PrimaryEntity;
        public List<Entity> AdditionalEntities;
        public List<LogEventData> LogEvents;
        public EntityManager EntityManager;

        public void Dispose()
        {
        }

        public static bool operator ==(EntityConversionData lhs, EntityConversionData rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(EntityConversionData lhs, EntityConversionData rhs)
        {
            return !(lhs == rhs);
        }

        public bool Equals(EntityConversionData other)
        {
            return PrimaryEntity.Equals(other.PrimaryEntity) && Equals(AdditionalEntities, other.AdditionalEntities) && Equals(EntityManager, other.EntityManager);
        }

        public override bool Equals(object obj)
        {
            return obj is EntityConversionData other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = PrimaryEntity.GetHashCode();
                hashCode = (hashCode * 397) ^ (AdditionalEntities != null ? AdditionalEntities.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (EntityManager != default ? EntityManager.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}
