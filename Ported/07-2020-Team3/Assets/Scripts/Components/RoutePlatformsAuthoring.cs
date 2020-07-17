using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct RoutePlatform : IBufferElementData
{
    public Entity Value;
}

public class RoutePlatformsAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public GameObject[] PlatformGameObjects;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        for (var i = 0; i < PlatformGameObjects.Length; ++i)
        {
            var platformGO = PlatformGameObjects[i];
            conversionSystem.DeclareDependency(gameObject, platformGO);
            if (!conversionSystem.HasPrimaryEntity(platformGO))
                return;
        }

        var platformsBuffer = dstManager.AddBuffer<RoutePlatform>(entity);
        platformsBuffer.Capacity = PlatformGameObjects.Length;
        for (var i = 0; i < PlatformGameObjects.Length; ++i)
        {
            var platformGO = PlatformGameObjects[i];
            platformsBuffer.Add(new RoutePlatform { Value = conversionSystem.GetPrimaryEntity(platformGO) });
        }
    }
}
