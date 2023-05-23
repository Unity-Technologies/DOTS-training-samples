using Unity.Entities;
using UnityEngine;

public class BeeSettingsSingletonAuthoring : MonoBehaviour
{
    public float flightJitter = 10f;

    [Range(0f, 1f)]
    public float damping = 0.01f;

    [Min(0f)]
    public float chaseForce = 1f;

    [Min(0f)]
    public float interactionDistance = 0.1f;

    private class BeeSettingsSingletonBaker : Baker<BeeSettingsSingletonAuthoring>
    {
        public override void Bake(BeeSettingsSingletonAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new BeeSettingsSingletonComponent()
            {
                flightJitter = authoring.flightJitter,
                damping = authoring.damping,
                chaseForce = authoring.chaseForce,
                interactionDistanceSquared = authoring.interactionDistance * authoring.interactionDistance
            });
        }
    }
}
