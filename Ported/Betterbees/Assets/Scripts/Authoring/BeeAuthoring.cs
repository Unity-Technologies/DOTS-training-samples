using Unity.Entities;
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

        AddComponent(entity, new VelocityComponent { });

        AddComponent(entity, new TargetComponent { });

        AddComponent(entity, new ReturnHomeComponent { });
    }
}