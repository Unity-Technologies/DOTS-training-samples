using Unity.Entities;
using UnityEngine;

public class BlueBeeAuthoring : MonoBehaviour
{
    
}

class BlueBeeBaker : Baker<BlueBeeAuthoring>
{
    public override void Bake(BlueBeeAuthoring authoring)
    {
        AddComponent<Bee>();
        AddComponent<BlueTeam>();
    }
}
