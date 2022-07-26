using Unity.Entities;

// Authoring MonoBehaviours are regular GameObject components.
// They constitute the inputs for the baking systems which generates ECS data.
public class FoodResourceAuthoring : UnityEngine.MonoBehaviour
{
}

// Bakers convert authoring MonoBehaviours into entities and components.
public class FoodResourceBaker : Baker<FoodResourceAuthoring>
{
    public override void Bake(FoodResourceAuthoring authoring)
    {
        AddComponent<FoodResource>();
    }
}