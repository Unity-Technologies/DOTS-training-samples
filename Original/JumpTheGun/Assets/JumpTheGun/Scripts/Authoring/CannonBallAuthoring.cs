using Unity.Entities;

// Authoring MonoBehaviours are regular GameObject components.
// They constitute the inputs for the baking systems which generates ECS data.
class CannonBallAuthoring : UnityEngine.MonoBehaviour
{
}

// Bakers convert authoring MonoBehaviours into entities and components.
class CannonBallBaker : Baker<CannonBallAuthoring>
{
    public override void Bake(CannonBallAuthoring authoring)
    {
        AddComponent<CannonBall>();
    }
}