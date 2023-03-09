using Unity.Entities;
using UnityEngine;

public class FoodAuthoring : MonoBehaviour
{
    public GameObject foodPrefab;
    public class Baker : Baker<FoodAuthoring>
    {
        public override void Bake(FoodAuthoring authoring)
        {
            AddComponent(new Food
            {
                foodPrefab = GetEntity(authoring.foodPrefab)
            });
        }
    }
}
public struct Food : IComponentData
{
    public Entity foodPrefab;
}