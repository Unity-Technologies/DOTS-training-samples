using Unity.Entities;

//@rename TankAuthoring_Step1 TankAuthoring
//@rename TankBaker_Step1 TankBaker

// TODO - ChildrenWithRenderer can be a non-temporary BakingType, needs package update

#region step1
class TankAuthoring_Step1 : UnityEngine.MonoBehaviour
{
}

class TankBaker_Step1 : Baker<TankAuthoring_Step1>
{
    public override void Bake(TankAuthoring_Step1 authoring)
    {
        AddComponent<Tank>();
    }
}
#endregion

#region step2
class TankAuthoring : UnityEngine.MonoBehaviour
{
}

// We want to add a color component to every entity which is part of the tank.
// Bakers can read from many authoring GameObjects, but can only modify their "own" entity.
// So we'll make a baker that gathers the set of entities that need a color component,
// and subsequently add the component in a baking system (TankBakingSystem).
// The purpose of the dynamic buffer of type ChildrenWithRenderer is to communicate that
// set of entities from the baker to the baking system.
// Since that set is only used during baking, it should not remain on the runtime entity.
// The BakingType attribute on ChildrenWithRenderer will make the baking system ignore that
// type when building the runtime entity.
[BakingType]
struct ChildrenWithRenderer : IBufferElementData
{
    public Entity Value;
}

class TankBaker : Baker<TankAuthoring>
{
    public override void Bake(TankAuthoring authoring)
    {
        AddComponent<Tank>();

        // Store the entities of all the children authoring GameObjects having a renderer.
        var buffer = AddBuffer<ChildrenWithRenderer>().Reinterpret<Entity>();
        foreach (var renderer in GetComponentsInChildren<UnityEngine.MeshRenderer>())
        {
            buffer.Add(GetEntity(renderer));
        }
    }
}
#endregion