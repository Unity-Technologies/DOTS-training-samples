using Unity.Entities;

// Authoring MonoBehaviours are regular GameObject components.
// They constitute the inputs for the baking systems which generates ECS data.
class BeeAuthoring : UnityEngine.MonoBehaviour
{
}

// Bakers convert authoring MonoBehaviours into entities and components.
class BeeBaker : Baker<BeeAuthoring>
{
    public override void Bake(BeeAuthoring authoring)
    {
        AddComponent<Bee>();
    }
}