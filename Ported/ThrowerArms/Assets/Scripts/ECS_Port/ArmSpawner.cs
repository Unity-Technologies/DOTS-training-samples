using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class ArmSpawner : MonoBehaviour
{
    public GameObject ArmPrefab;
    public int Count = 100;
    
    public static float ArmRowWidth{ get; private set;}
    
    private const float Spacing = 1;
    
    private void Awake()
    {
        ArmRowWidth = (Count - 1) * Spacing;
    }

    private void Start()
    {
        GameObjectConversionSettings settings = 
            GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, blobAssetStore: null);
        
        Entity prefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(ArmPrefab, settings);
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        for (var i = 0; i < Count; i++)
        {
            Entity instance = entityManager.Instantiate(prefab);
            
            entityManager.AddComponentData(instance, new ArmComponent());
            entityManager.SetComponentData(instance, new Translation
            {
                Value = Spacing * i * new float3(1, 0, 0)
            });
            entityManager.SetComponentData(instance, new Rotation
            {
                Value = quaternion.identity
            });
        }
    }
}