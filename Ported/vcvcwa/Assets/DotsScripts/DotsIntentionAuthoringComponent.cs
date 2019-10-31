using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

// ReSharper disable once InconsistentNaming
[RequiresEntityConversion]
public class DotsIntentionAuthoringComponent : MonoBehaviour, IConvertGameObjectToEntity
{
    public DotsIntention dotsIntention;

    // Lets you convert the editor data representation to the entity optimal runtime representation
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var data = new DotsIntentionComponent{intention = dotsIntention};
        dstManager.AddComponentData(entity, data);
    }
}
