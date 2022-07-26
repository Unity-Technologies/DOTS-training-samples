using Unity.Entities;
using UnityEngine;

public class YellowBeeAuthoring : MonoBehaviour
{
    
}

class YellowBeeBaker : Baker<YellowBeeAuthoring>
{
    public override void Bake(YellowBeeAuthoring authoring)
    {
        var transform = authoring.transform;
        AddComponent(new Bee
        {
            position = transform.position,
            rotation = transform.rotation.eulerAngles,
            state = BeeState.Idle
        });
        AddComponent<YellowTeam>();
    }
}
