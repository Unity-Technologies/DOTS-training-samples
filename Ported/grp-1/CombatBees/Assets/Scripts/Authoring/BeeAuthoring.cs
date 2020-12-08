using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

public class BeeAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager dstManager
       , GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponent<Bee>(entity);
        dstManager.AddComponent<URPMaterialPropertyBaseColor>(entity);
    }
}
