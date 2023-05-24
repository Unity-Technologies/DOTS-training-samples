using Unity.Entities;
using UnityEngine;

public class SmokeAuthoring : MonoBehaviour
{
}

public class SmokeBaker : Baker<SmokeAuthoring>
{
    public override void Bake(SmokeAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new VelocityComponent());
        AddComponent(entity, new BloodComponent());
        //AddComponent(entity, new GravityComponent());
    }
}
