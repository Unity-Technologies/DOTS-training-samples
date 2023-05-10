using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class OmnibotAuthoring : MonoBehaviour
{
    class Baker : Baker<OmnibotAuthoring>
    {
        public override void Bake(OmnibotAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new Omnibot
            {
                omnibotState = OmnibotState.LookForWater, 
                t = 0
            });
            
        }
    }
}

public enum OmnibotState
{
    LookForWater,
    TravelToWater,
    GatherWater,
    LookForFire,
    TravelToFire,
    TurnOffFire
}

public struct Omnibot : IComponentData
{
    public float t;
    public float3 OmniboatTargetPos;
    public Entity targetEntity;

    public OmnibotState omnibotState;
}