using Unity.Entities;
using UnityEngine;

[RequireComponent(typeof(VelocityAuthoring))]
class JitterAuthoring : UnityEngine.MonoBehaviour
{
    
}

class JitterBaker : Baker<JitterAuthoring>
{
    public override void Bake(JitterAuthoring authoring)
    {
        AddComponent(new Jitter());
    }
}