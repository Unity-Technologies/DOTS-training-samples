using Unity.Entities;
using Unity.Mathematics;

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

        AddComponent<CommuterSpeed>();

        //var random = Random.CreateFromIndex(3849);
        //float3 speed = new float3(random.NextFloat(0f, 1f), 0f, random.NextFloat(0f, 1f));
        //AddComponent(new CommuterPos { Value = speed });
    }
}