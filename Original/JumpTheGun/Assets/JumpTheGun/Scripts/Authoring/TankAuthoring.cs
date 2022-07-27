using Unity.Entities;

// Authoring MonoBehaviours are regular GameObject components.
// They constitute the inputs for the baking systems which generates ECS data.
class TankAuthoring : UnityEngine.MonoBehaviour
{
    public int row;
    public int column;
}

// Bakers convert authoring MonoBehaviours into entities and components.
class TankBaker : Baker<TankAuthoring>
{
    public override void Bake(TankAuthoring authoring)
    {
        AddComponent(new Tank
        {
            row = authoring.row,
            column = authoring.column
        });
    }
}
