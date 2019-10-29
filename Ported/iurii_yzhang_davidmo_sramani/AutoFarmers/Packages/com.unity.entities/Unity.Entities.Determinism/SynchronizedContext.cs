using System;
using System.Diagnostics;

namespace Unity.Entities.Determinism
{
    internal struct SynchronizedContext : IDisposable, IEquatable<SynchronizedContext>
    {
        public bool IsCreated => (SystemVersion > 0u) && (WorldID > 0u) && (EntityVersion > 0);

        public bool IsValidIn(World world)
        {
            CheckWorldIsCreatedOrThrow(world);
            return CreateFormWorld_Unsynchronized(world).Equals(this);
        }
        
        public void Dispose() => SystemVersion = WorldID = (ulong)(EntityVersion = 0);
        
        public static SynchronizedContext CreateFromWorld(World world)
        {
            CheckWorldIsCreatedOrThrow(world);
            
            world.EntityManager.BeforeStructuralChange();
            return CreateFormWorld_Unsynchronized(world);
        }
        
        ulong SystemVersion;
        ulong WorldID;
        int EntityVersion;

        static SynchronizedContext CreateFormWorld_Unsynchronized(World world)
        {
            CheckWorldIsCreatedOrThrow(world);
            
            var entityManager = world.EntityManager;
            entityManager.BeforeStructuralChange();

            var worldId = world.SequenceNumber;
            var systemVersion = entityManager.GlobalSystemVersion;
            var entityVersion = entityManager.Version;
            return new SynchronizedContext(worldId, systemVersion, entityVersion);            
        }
        
        SynchronizedContext(ulong worldId, ulong systemVersion, int entityVersion)
        {
            WorldID = worldId;
            SystemVersion = systemVersion;
            EntityVersion = entityVersion;
        }

        [Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
        static void CheckWorldIsCreatedOrThrow(World world)
        {
            if (!world.IsCreated)
            {
                throw new ArgumentException("World has not been created.");
            }
        }

        public bool Equals(SynchronizedContext other)
        {
            return SystemVersion == other.SystemVersion && WorldID == other.WorldID && EntityVersion == other.EntityVersion;
        }
    }

}