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

public class ArrowSpawnerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities.ForEach((in PlayerInput playerInput) => {

            if (playerInput.TileIndex < BoardSpawner.cells.Length && playerInput.TileIndex >= 0)
            {
                var cell = BoardSpawner.cells[playerInput.TileIndex];
                EntityManager.SetComponentData<URPMaterialPropertyBaseColor>(cell, new URPMaterialPropertyBaseColor() {Value = new float4(1f, 0f, 0f, 1f)});
            }
            
        }).WithoutBurst().Run();
    }
}
