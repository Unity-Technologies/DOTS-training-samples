using Unity.Entities;

// Authoring MonoBehaviours are regular GameObject components.
// They constitute the inputs for the baking systems which generates ECS data.
class CommuterAuthoring : UnityEngine.MonoBehaviour
{
}

// Bakers convert authoring MonoBehaviours into entities and components.
class CommuterBaker : Baker<CommuterAuthoring>
{
    public override void Bake(CommuterAuthoring authoring)
    {
        AddComponent<CommuterTag>();
        AddComponent<CommuterPos>();
    }
}