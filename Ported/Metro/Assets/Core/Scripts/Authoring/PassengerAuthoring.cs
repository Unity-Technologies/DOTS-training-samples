using Unity.Entities;

class PassengerAuthoring : UnityEngine.MonoBehaviour
{
}
 
class PassengerBaker : Baker<PassengerAuthoring>
{
    public override void Bake(PassengerAuthoring authoring)
    {
        AddComponent(new PassengerTag());
        AddComponent(new LocationInfo());
    }
}