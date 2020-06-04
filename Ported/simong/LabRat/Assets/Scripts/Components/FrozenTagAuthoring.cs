using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

public class FrozenTagAuthoring : UnityEngine.MonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddSharedComponentData(entity, new FrozenRenderSceneTag { SceneGUID = new Unity.Entities.Hash128(1, 1, 1, 1), SectionIndex = 0, HasStreamedLOD = 0 });
    }
}