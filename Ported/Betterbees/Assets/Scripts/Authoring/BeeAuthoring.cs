using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class BeeAuthoring : MonoBehaviour
{
}

public class BeeBaker : Baker<BeeAuthoring>
{
    public override void Bake(BeeAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic);

        AddComponent(entity, new BeeState()
        {
            state = BeeState.State.IDLE
        });
    }
}