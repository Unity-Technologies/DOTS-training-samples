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

    public static readonly int TexSize = 128;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var pheromoneMap = new Texture2D(TexSize, TexSize);

        PheromoneMapRenderer.material.SetTexture("_BaseMap", pheromoneMap);

        var refs = new Refs
        {
            PheromoneMap = pheromoneMap
        };

        dstManager.AddComponentData(entity, refs);
    }
}
