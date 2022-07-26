using Unity.Entities;
using UnityEngine;

[RequireComponent(typeof(VelocityAuthoring))]
class GravityAuthoring : UnityEngine.MonoBehaviour
{
}

class GravityBaker : Baker<GravityAuthoring>
{
    public override void Bake(GravityAuthoring authoring)
    {
        AddComponent(new Gravity());
    }
}