using DefaultNamespace;
using Unity.Entities;

// Authoring MonoBehaviours are regular GameObject components.
// They constitute the inputs for the baking systems which generates ECS data.
class BaseAuthoring : UnityEngine.MonoBehaviour
{
    public BasePosition yellowBase;
    public BasePosition blueBase;
}

// Bakers convert authoring MonoBehaviours into entities and components.
class BaseBaker : Baker<BaseAuthoring>
{
    public override void Bake(BaseAuthoring authoring)
    {
        AddComponent(new Base
        {
            blueBase = authoring.blueBase,
            yellowBase = authoring.yellowBase
        });
    }
}
