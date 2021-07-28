using System;
using Unity.Assertions;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace src.Components
{
    public class RemoveDefaultTransformsOnEntityAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        public bool KeepTranslation, KeepScale, KeepRotation, KeepLocalToWorld;
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            if(! KeepTranslation) dstManager.RemoveComponent<Translation>(entity);
            if(! KeepRotation) dstManager.RemoveComponent<Rotation>(entity);
            if(! KeepScale) dstManager.RemoveComponent<Scale>(entity);
            if(! KeepLocalToWorld)  dstManager.RemoveComponent<LocalToWorld>(entity);
            // NW: Fixed in Spawner.cs - if(! KeepLinkedEntityGroup)  assertDidRemoveAll &= dstManager.RemoveComponent<LinkedEntityGroup>(entity);
        }
    }
}
