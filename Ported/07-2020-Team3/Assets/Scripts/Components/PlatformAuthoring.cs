using System;
using Unity.Entities;
using UnityEngine;

public class PlatformAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public bool Inbound;
    public GameObject HeadStairsBottom;
    public GameObject HeadStairsTop;
    public GameObject FootStairsBottom;
    public GameObject FootStairsTop;
    public GameObject Queue0;
    public GameObject Queue1;
    public GameObject Queue2;
    public GameObject Queue3;
    public GameObject Queue4;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        conversionSystem.DeclareDependency(gameObject, HeadStairsBottom);
        conversionSystem.DeclareDependency(gameObject, HeadStairsTop);
        conversionSystem.DeclareDependency(gameObject, FootStairsBottom);
        conversionSystem.DeclareDependency(gameObject, FootStairsTop);
        conversionSystem.DeclareDependency(gameObject, Queue0);
        conversionSystem.DeclareDependency(gameObject, Queue1);
        conversionSystem.DeclareDependency(gameObject, Queue2);
        conversionSystem.DeclareDependency(gameObject, Queue3);
        conversionSystem.DeclareDependency(gameObject, Queue4);

        if (!conversionSystem.HasPrimaryEntity(HeadStairsBottom)
         || !conversionSystem.HasPrimaryEntity(HeadStairsTop)
         || !conversionSystem.HasPrimaryEntity(FootStairsBottom)
         || !conversionSystem.HasPrimaryEntity(FootStairsTop)
         || !conversionSystem.HasPrimaryEntity(Queue0)
         || !conversionSystem.HasPrimaryEntity(Queue1)
         || !conversionSystem.HasPrimaryEntity(Queue2)
         || !conversionSystem.HasPrimaryEntity(Queue3)
         || !conversionSystem.HasPrimaryEntity(Queue4))
            return;

        var platform = new Platform
        {
            Inbound = Inbound,
            HeadStairsBottom = conversionSystem.GetPrimaryEntity(HeadStairsBottom),
            HeadStairsTop = conversionSystem.GetPrimaryEntity(HeadStairsTop),
            FootStairsBottom = conversionSystem.GetPrimaryEntity(FootStairsBottom),
            FootStairsTop = conversionSystem.GetPrimaryEntity(FootStairsTop),
            Queue0 = conversionSystem.GetPrimaryEntity(Queue0),
            Queue1 = conversionSystem.GetPrimaryEntity(Queue1),
            Queue2 = conversionSystem.GetPrimaryEntity(Queue2),
            Queue3 = conversionSystem.GetPrimaryEntity(Queue3),
            Queue4 = conversionSystem.GetPrimaryEntity(Queue4)
        };

        dstManager.AddComponentData(entity, platform);
        dstManager.AddBuffer<AdjacentPlatform>(entity);
    }
}


// [GenerateAuthoringComponent]
public struct Platform : IComponentData
{
    public bool Inbound;
    public Entity HeadStairsBottom;
    public Entity HeadStairsTop;
    public Entity FootStairsBottom;
    public Entity FootStairsTop;
    public Entity Queue0;
    public Entity Queue1;
    public Entity Queue2;
    public Entity Queue3;
    public Entity Queue4;
    [NonSerialized] public Entity NextPlatform;
    [NonSerialized] public Entity OppositePlatform;
}

[InternalBufferCapacity(2)]
public struct AdjacentPlatform : IBufferElementData
{
    public Entity Value;
}
