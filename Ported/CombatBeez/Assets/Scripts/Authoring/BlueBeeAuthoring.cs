using Unity.Entities;

// Authoring MonoBehaviours are regular GameObject components.
// They constitute the inputs for the baking systems which generates ECS data.
public class BlueBeeAuthoring : UnityEngine.MonoBehaviour
{
}

// Bakers convert authoring MonoBehaviours into entities and components.
public class BlueBeeBaker : Baker<BlueBeeAuthoring>
{
    public override void Bake(BlueBeeAuthoring authoring)
    {
        AddComponent<BlueBee>();
        AddComponent<Bee>();
    }
}