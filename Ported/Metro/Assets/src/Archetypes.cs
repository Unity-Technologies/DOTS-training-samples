using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace src
{
    public class Archetypes
    {
        public static EntityArchetype RailMarkerArchetype()
        {
            return World.Active.EntityManager.CreateArchetype(
                ComponentType.ReadWrite<Translation>(),
                ComponentType.ReadWrite<Rotation>(),
                ComponentType.ReadWrite<LocalToWorld>(),
                ComponentType.ReadWrite<MeshRenderer>());
        }
    }
}