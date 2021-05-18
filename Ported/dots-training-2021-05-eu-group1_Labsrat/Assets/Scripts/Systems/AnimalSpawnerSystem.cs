using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;
using UnityCamera = UnityEngine.Camera;
using UnityGameObject = UnityEngine.GameObject;
using UnityInput = UnityEngine.Input;
using UnityKeyCode = UnityEngine.KeyCode;
using UnityMeshRenderer = UnityEngine.MeshRenderer;
using UnityMonoBehaviour = UnityEngine.MonoBehaviour;
using UnityRangeAttribute = UnityEngine.RangeAttribute;

public class AnimalSpawnerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var random = new Random(1234);
        
        if (TryGetSingleton(out GameConfig gameConfig))
        {
            for (int i = 0; i < gameConfig.NumOfCats; i++)
            {
                var xPos = random.NextInt(gameConfig.BoardDimensions.x);
                var yPos = random.NextInt(gameConfig.BoardDimensions.y);

                var randNum = random.NextInt(3);
                var rotation = Unity.Mathematics.quaternion.RotateY(Mathf.PI * randNum / 2);
                
                Entity cat = EntityManager.Instantiate(gameConfig.CatPrefab);
                EntityManager.SetComponentData(cat, new Translation() { Value = new float3(xPos, 1, yPos) });
                EntityManager.SetComponentData(cat, new Rotation() {Value = rotation});

                EntityManager.AddComponent<Position>(cat);
                EntityManager.SetComponentData(cat, new Position() { Value = new float2(xPos, yPos) });
            }
        
            Enabled = false;
        }
    }
}
