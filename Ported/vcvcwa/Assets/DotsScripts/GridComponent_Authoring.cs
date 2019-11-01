using System;
using System.Collections.Generic;
using System.IO;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

// ReSharper disable once InconsistentNaming
[RequiresEntityConversion]
public class GridComponent_Authoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public TextAsset mapFile;

    private List<int> values;
    
    public int size;
    
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var data = new GridComponent() {Size = size};

        var textValues = mapFile.text.Split(',','\r','\n',' ');
        values = new List<int>(textValues.Length);

        foreach (var text in textValues)
        {
            int testValue;
            if (Int32.TryParse(text, out testValue))
            {
                values.Add(Int32.Parse(text));
            }
            else
            {
                Debug.AssertFormat(true, "Importing the grid got broken.");
            }
        }
        
        // todo - make this a bit smarter. Add a sub-grid starting from zero
        DynamicBuffer<GridTile> buffer = dstManager.AddBuffer<GridTile>(entity);
        for (int i = 0; i < size*size; ++i)
        {
            if (i < values.Count)
            {
                buffer.Add(values[i]);
            }
            else
            {
                buffer.Add(0);
            }
        }
        
        dstManager.AddComponentData(entity, data);
        
    }
}