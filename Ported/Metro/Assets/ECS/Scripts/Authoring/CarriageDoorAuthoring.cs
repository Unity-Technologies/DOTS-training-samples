using Unity.Entities;

// REMEBER: Add this component to the CarriageDoor GameObject.

public class CarriageDoorAuthoring : UnityEngine.MonoBehaviour
{
    public UnityEngine.Transform door_LEFT;
    public UnityEngine.Transform door_RIGHT;
}

class CarriageDoorBaker : Baker<CarriageDoorAuthoring>
{
    public override void Bake(CarriageDoorAuthoring authoring)
    {
        // By default, components are zero-initialized.
        AddComponent(new CarriageDoor {
            door_LEFT = GetEntity(authoring.door_LEFT),
            door_RIGHT = GetEntity(authoring.door_RIGHT),
            left_CLOSED_X = authoring.door_LEFT.localPosition.x,
            left_OPEN_X = (authoring.door_LEFT.localPosition.x - authoring.door_LEFT.localScale.x)
        });    
    }
}
