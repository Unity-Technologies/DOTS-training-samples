using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class FoodBuilderAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    [SerializeField] protected internal GameObject foodPrefab = null;
    [Range(0.01F, 10.0F)] [SerializeField] protected internal float foodRadius = 0.5F;
    [SerializeField] protected internal Color foodColor = new Color(0, 0, 1, 1);
    [Range(1, 10)] [SerializeField] private int numFood = 1;
    
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity,
            new FoodBuilder()
            {
                foodPrefab = conversionSystem.GetPrimaryEntity(foodPrefab),
                foodRadius = foodRadius,
                foodColor = new float4(foodColor.r, foodColor.g, foodColor.b, foodColor.a),
                numFood = numFood
            });
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(foodPrefab);
    }
}
