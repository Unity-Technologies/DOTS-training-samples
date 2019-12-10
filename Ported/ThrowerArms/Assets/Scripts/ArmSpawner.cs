using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class ArmSpawner : MonoBehaviour
{
    public GameObject ArmPrefab;
    
    public int Count = 100;
    public float Spacing = 1;

    public static float ArmRowWidth;
    
    private void Awake()
    {
        ArmRowWidth = (Count - 1) * Spacing;
    }

    void Start()
    {
        GameObjectConversionSettings settings = 
            GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, blobAssetStore: null);
        
        Entity prefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(ArmPrefab, settings);
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        for (var i = 0; i < Count; i++)
        {
            Entity instance = entityManager.Instantiate(prefab);

            entityManager.SetComponentData(instance, new Translation
            {
                Value = Spacing * i * Vector3.right
            });
            entityManager.SetComponentData(instance, new Rotation
            {
                Value = quaternion.identity
            });
        }
    }
}