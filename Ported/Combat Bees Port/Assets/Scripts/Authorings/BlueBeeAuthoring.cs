using Unity.Entities;
using UnityEngine;

public class BlueBeeAuthoring : MonoBehaviour
{
    
}

class BlueBeeBaker : Baker<BlueBeeAuthoring>
{
    public override void Bake(BlueBeeAuthoring authoring)
    {
        var transform = authoring.transform;
        AddComponent(new Bee
        {
            position = transform.position,
            rotation = transform.rotation.eulerAngles,
            state = BeeState.Idle
        });
        AddComponent<BlueTeam>();
    }
}
