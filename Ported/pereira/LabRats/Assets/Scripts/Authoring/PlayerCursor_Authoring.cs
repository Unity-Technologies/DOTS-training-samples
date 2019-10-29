using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Serialization;

// ReSharper disable once InconsistentNaming
[RequiresEntityConversion]
public class PlayerCursor_Authoring : MonoBehaviour,IDeclareReferencedPrefabs, IConvertGameObjectToEntity
{
    public Players PlayerId = Players.Player1;
    public GameObject ArrowPrefab;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new LbPlayer { Value = (byte)PlayerId });
        dstManager.AddComponentData(entity, new LbCursor());
        dstManager.AddComponentData(entity, new LbCursorInit());
        dstManager.AddComponentData(entity, new LbArrowPrefab { Value = conversionSystem.GetPrimaryEntity(ArrowPrefab) });
        dstManager.AddComponentData(entity, new LbMovementTarget() { From = float3.zero, To = float3.zero });
        dstManager.AddComponentData(entity, new LbDistanceToTarget() { Value = 0.0f });
        dstManager.AddComponentData(entity, new LbMovementSpeed { Value = 0.5f });
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(ArrowPrefab);
    }
}
