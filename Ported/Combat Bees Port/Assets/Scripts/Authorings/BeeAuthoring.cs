using Unity.Entities;
using UnityEngine;

public class BeeAuthoring : MonoBehaviour
{
    
}

class BeeBaker : Baker<BeeAuthoring>
{
    public override void Bake(BeeAuthoring authoring)
    {
        AddComponent<Bee>();
        AddComponent<YellowTeam>();
    }
}
