using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityGameObject = UnityEngine.GameObject;
using UnityRangeAttribute = UnityEngine.RangeAttribute;
using UnityMonoBehaviour = UnityEngine.MonoBehaviour;

namespace Dots
{
    [DisallowMultipleComponent]
    public class BeamSpawnerAuthoring : UnityMonoBehaviour
        , IConvertGameObjectToEntity
    {
        public int buildingCount = 1;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new Dots.BeamSpawnerData
            {                
                buildingCount = buildingCount
            });
        }
    }
}
