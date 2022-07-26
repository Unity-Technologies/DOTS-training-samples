using Unity.Entities;
using UnityEngine;

class VelocityAuthoring : MonoBehaviour
{
    public Vector3 Value;
}

class VelocityBaker : Baker<VelocityAuthoring>
{
    public override void Bake(VelocityAuthoring authoring)
    {
        AddComponent(new Velocity
        {
            Value = authoring.Value
        });
    }
    
}
