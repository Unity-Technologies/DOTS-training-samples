using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityCamera = UnityEngine.Camera;
using UnityGameObject = UnityEngine.GameObject;
using UnityInput = UnityEngine.Input;
using UnityKeyCode = UnityEngine.KeyCode;
using UnityMeshRenderer = UnityEngine.MeshRenderer;
using UnityMonoBehaviour = UnityEngine.MonoBehaviour;
using UnityRangeAttribute = UnityEngine.RangeAttribute;

public class BoardSpawner : SystemBase
{
    protected override void OnCreate()
    {
    }
    
    protected override void OnUpdate()
    {
        if (TryGetSingleton(out GameConfig gameConfig))
        {
            for (int x = 0; x < gameConfig.BoardDimensions.x; x++)
            {
                for (int y = 0; y < gameConfig.BoardDimensions.y; y++)
                {
                    Entity cell = EntityManager.Instantiate(gameConfig.CellPrefab);
                    EntityManager.SetComponentData(cell, new Translation() { Value = new float3(x, 0, y) });

                    float4 color = (x + y) % 2 == 0 ? gameConfig.TileColor1 : gameConfig.TileColor2;
                    EntityManager.AddComponentData(cell, new URPMaterialPropertyBaseColor() { Value = color });

                }
            }
            
            Enabled = false;
        }
    }
}
