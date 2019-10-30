using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

// ReSharper disable once InconsistentNaming
[RequiresEntityConversion]
public class GridComponent_Authoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public List<int> values = new List<int>(100);

    public int size;
    // The MonoBehaviour data is converted to ComponentData on the entity.
    // We are specifically transforming from a good editor representation of the data (Represented in degrees)
    // To a good runtime representation (Represented in radians)
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var data = new GridComponent() {Size = size};
        
        var buffer = dstManager.AddBuffer<GridTile>(entity);
        for (int i = 0; i < values.Count; ++i)
        {
            buffer.Add(values[i]);
        }
        

        dstManager.AddComponentData(entity, data);
    }
}