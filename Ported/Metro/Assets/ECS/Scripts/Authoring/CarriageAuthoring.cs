using Unity.Entities;

// REMEBER: Add this component to the Carriage GameObject.

public class CarriageAuthoring : UnityEngine.MonoBehaviour
{
    public UnityEngine.Transform CarriageDoorLeft;
    public UnityEngine.Transform CarriageDoorRight;
}

class CarriageBaker : Baker<CarriageAuthoring>
{
    public override void Bake(CarriageAuthoring authoring)
    {
        AddComponent(new Carriage
        {
            // By default, each authoring GameObject turns into an Entity.
            // Given a GameObject (or authoring component), GetEntity looks up the resulting Entity.
            CarriageDoorLeft = GetEntity(authoring.CarriageDoorLeft),
            CarriageDoorRight = GetEntity(authoring.CarriageDoorRight),
        });
    }
}
