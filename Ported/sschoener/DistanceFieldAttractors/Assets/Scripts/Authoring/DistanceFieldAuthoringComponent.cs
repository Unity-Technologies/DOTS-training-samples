using System;
using Unity.Entities;
using UnityEngine;

namespace Systems {
    public class DistanceFieldAuthoringComponent : MonoBehaviour, IConvertGameObjectToEntity
    {
        public DistanceFieldModel Model;
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new DistanceFieldComponent
            {
                ModelType = Model
            });
        }
    }
}
