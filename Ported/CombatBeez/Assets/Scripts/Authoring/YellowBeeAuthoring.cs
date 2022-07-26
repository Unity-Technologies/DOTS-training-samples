using Unity.Entities;

// Authoring MonoBehaviours are regular GameObject components.
// They constitute the inputs for the baking systems which generates ECS data.
public class YellowBeeAuthoring : UnityEngine.MonoBehaviour
{
}

// Bakers convert authoring MonoBehaviours into entities and components.
public class YellowBeeBaker : Baker<YellowBeeAuthoring>
{
    public override void Bake(YellowBeeAuthoring authoring)
    {
        AddComponent<YellowBee>();
        AddComponent<Bee>();
    }
}