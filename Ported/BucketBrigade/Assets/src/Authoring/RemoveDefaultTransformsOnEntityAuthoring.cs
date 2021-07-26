using System;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace src.Components
{
    public class RemoveDefaultTransformsOnEntityAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        public bool KeepTranslation, KeepScale, KeepRotation, KeepLinkedEntityGroup;
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            if(! KeepTranslation) dstManager.RemoveComponent<Translation>(entity);
            if(! KeepRotation) dstManager.RemoveComponent<Rotation>(entity);
            if(! KeepScale) dstManager.RemoveComponent<Scale>(entity);
            if(! KeepLinkedEntityGroup) dstManager.RemoveComponent<LinkedEntityGroup>(entity);
        }
    }
}
