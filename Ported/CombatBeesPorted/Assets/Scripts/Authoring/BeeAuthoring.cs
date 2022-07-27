using Unity.Entities;
using UnityEngine;

[RequireComponent(typeof(VelocityAuthoring))]
class BeeAuthoring : UnityEngine.MonoBehaviour
{
    
}

class JitterBaker : Baker<BeeAuthoring>
{
    public override void Bake(BeeAuthoring authoring)
    {
        AddComponent(new Bee());
    }
}