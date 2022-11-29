using Unity.Entities;
using UnityEngine;

public class PassengersConfigAuthoring : MonoBehaviour
{
    public GameObject PassengerPrefab;
    public int PassengerCount;
}

class PassengersConfigBaker : Baker<PassengersConfigAuthoring>
{
    public override void Bake(PassengersConfigAuthoring authoring)
    {
        AddComponent(new PassengersConfig
        {
            passengerCount = authoring.PassengerCount,
            passengerPrefab = GetEntity(authoring.PassengerPrefab)
        });
    }
}
