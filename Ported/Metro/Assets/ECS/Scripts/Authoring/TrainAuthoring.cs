using Unity.Entities;

// Authoring MonoBehaviours are regular GameObject components.
// They constitute the inputs for the baking systems which generates ECS data.
class TrainAuthoring : UnityEngine.MonoBehaviour
{
}

// Bakers convert authoring MonoBehaviours into entities and components.
class TrainBaker : Baker<TrainAuthoring>
{
    public override void Bake(TrainAuthoring authoring)
    {
        AddComponent<Train>();
    }
}
