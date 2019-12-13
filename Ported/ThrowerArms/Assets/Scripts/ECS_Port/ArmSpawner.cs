using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class ArmSpawner : MonoBehaviour
{
    public GameObject ArmPrefab;
    public static int Count = 200;
    public Mesh ArmMesh;
    public Material ArmMaterial;

    public static float ArmRowWidth{ get; private set;}
    
    public static float Spacing = 1;

    public static Mesh SharedArmMesh;
    public static Material SharedArmMaterial;

    private void Awake()
    {
        ArmRowWidth = (Count - 1) * Spacing;
        SharedArmMaterial = ArmMaterial;
        SharedArmMesh = ArmMesh;
    }

    private void Start()
    {
        GameObjectConversionSettings settings = 
            GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, blobAssetStore: null);
        
        Entity prefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(ArmPrefab, settings);
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        Unity.Mathematics.Random rnd = new Unity.Mathematics.Random(34);
        for (var i = 0; i < Count; i++)
        {
            Entity instance = entityManager.Instantiate(prefab);

            entityManager.AddComponentData(instance, new ArmComponent { HandUp = new float3(0.0f, 1.0f, 0.0f), IdleOffset = rnd.NextFloat() }) ;
            entityManager.AddComponentData(instance, new Finger());
            
            entityManager.AddComponentData(instance, new IdleState());
            entityManager.AddComponentData(instance, new FindGrabbableTargetState());
            
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