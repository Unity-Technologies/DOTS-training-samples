using System;
using Unity.Assertions;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace src.Components
{
    public class RemoveDefaultTransformsOnEntityAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        public bool KeepTranslation, KeepScale, KeepRotation, KeepLocalToWorld, KeepLinkedEntityGroup;
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            bool assertDidRemoveAll = true;
            if(! KeepTranslation) assertDidRemoveAll &= dstManager.RemoveComponent<Translation>(entity);
            if(! KeepRotation)  assertDidRemoveAll &= dstManager.RemoveComponent<Rotation>(entity);
            if(! KeepScale) assertDidRemoveAll &= dstManager.RemoveComponent<Scale>(entity);
            if(! KeepLocalToWorld)  assertDidRemoveAll &= dstManager.RemoveComponent<LocalToWorld>(entity);
            if(! KeepLinkedEntityGroup)  assertDidRemoveAll &= dstManager.RemoveComponent<LinkedEntityGroup>(entity);
            Assert.IsTrue(assertDidRemoveAll, $"Failed to remove all specified components! '{dstManager.World.Name}', Prefab '{name}'!");
            
            // todo - investigate how to order these so that LinkedEntityGroup WILL be removed from the entity.
        }
    }
}
