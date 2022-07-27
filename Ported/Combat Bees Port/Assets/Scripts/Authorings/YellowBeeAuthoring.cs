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
            state = BeeState.Idle
        });
        AddComponent<YellowTeam>();
    }
}
