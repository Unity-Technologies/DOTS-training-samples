using Unity.Entities;

public class CarBaker : Baker<CarAuthoring>
{
    public override void Bake(CarAuthoring authoring)
    {
        AddComponent<Car>();

        // Store the entities of all the children authoring GameObjects having a renderer.
        var buffer = AddBuffer<ChildrenWithRenderer>().Reinterpret<Entity>();
        foreach (var renderer in GetComponentsInChildren<UnityEngine.MeshRenderer>())
        {
            buffer.Add(GetEntity(renderer));
        }
    }
}

[TemporaryBakingType] struct ChildrenWithRenderer : IBufferElementData { public Entity Value; }

