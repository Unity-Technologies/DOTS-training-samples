using Unity.Entities;
using UnityEngine;

public class YellowBeeAuthoring : MonoBehaviour
{
    
}

class YellowBeeBaker : Baker<YellowBeeAuthoring>
{
    public override void Bake(YellowBeeAuthoring authoring)
    {
        AddComponent(new Bee
        {
            target = Entity.Null,
            state = BeeState.Idle
        });
        AddComponent<YellowTeam>();
    }
}
