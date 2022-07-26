using Unity.Entities;
using UnityEngine;

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