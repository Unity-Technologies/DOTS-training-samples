using Unity.Entities;
using UnityEngine;

public class BlueBeeAuthoring : MonoBehaviour
{
    
}

class BlueBeeBaker : Baker<BlueBeeAuthoring>
{
    public override void Bake(BlueBeeAuthoring authoring)
    {
        AddComponent(new Bee
        {
            state = BeeState.Idle
        });
        AddComponent<BlueTeam>();
        AddComponent<Idle>();
    }
}
