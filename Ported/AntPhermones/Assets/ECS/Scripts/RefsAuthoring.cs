using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.PlayerLoop;

public class RefsAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    [SerializeField] MeshRenderer PheromoneMapRenderer;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var pheromoneMap = new Texture2D(256, 256);

        PheromoneMapRenderer.material.SetTexture("_BaseMap", pheromoneMap);

        // Exemple code writing in the texture to be sure that it works
        // Doesn't work in systems from what I've tested. The final Texture2D will probably need to be done with normal code.
        // Remove it later
        for (int i = 0; i < 64; ++i)
        {
            for (int j = 0; j < 64; ++j)
            {
                pheromoneMap.SetPixel(i, j, Color.blue);
            }
        }
        pheromoneMap.Apply();

        var refs = new Refs
        {
            PheromoneMap = pheromoneMap
        };

        dstManager.AddComponentData(entity, refs);
    }
}
