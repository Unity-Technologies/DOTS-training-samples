using Unity.Entities;

class PassengerAuthoring : UnityEngine.MonoBehaviour
{
}
 
class PassengerBaker : Baker<PassengerAuthoring>
{
    public override void Bake(PassengerAuthoring authoring)
    {
        // maybe this should be added dynamically instead, when taking a train
        //AddComponent(new PassengerTag());
        AddComponent(new LocationInfo());
        AddComponent(new IdleTime());
    }
}