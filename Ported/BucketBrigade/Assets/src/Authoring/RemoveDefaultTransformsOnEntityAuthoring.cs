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
            bool assertDidRemoveAll = true;
            if(! KeepTranslation) assertDidRemoveAll &= dstManager.RemoveComponent<Translation>(entity);
            if(! KeepRotation)  assertDidRemoveAll &= dstManager.RemoveComponent<Rotation>(entity);
            var hasNonUniformScale = transform.localScale != Vector3.one;
            if(! KeepScale && hasNonUniformScale) assertDidRemoveAll &= dstManager.RemoveComponent<Scale>(entity);
            if(! KeepLocalToWorld)  assertDidRemoveAll &= dstManager.RemoveComponent<LocalToWorld>(entity);
            // NW: Fixed in Spawner.cs - if(! KeepLinkedEntityGroup)  assertDidRemoveAll &= dstManager.RemoveComponent<LinkedEntityGroup>(entity);
            Debug.Assert(assertDidRemoveAll, $"Failed to remove all specified components! '{dstManager.World.Name}', Prefab '{name}'!");
        }
    }
}
