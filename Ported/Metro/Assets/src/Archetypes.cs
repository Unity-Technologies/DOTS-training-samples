using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace src
{
    public class Archetypes
    {
        public static EntityArchetype RailMarkerArchetype(EntityManager dstManager)
        {
            return dstManager.CreateArchetype(
                ComponentType.ReadWrite<Translation>(),
                ComponentType.ReadWrite<Rotation>(),
                ComponentType.ReadWrite<LocalToWorld>(),
                ComponentType.ReadWrite<SimpleMeshRenderer>());
        }
    }
}