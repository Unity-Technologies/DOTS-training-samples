using Unity.Entities;
using UnityEngine;

public class BeeSettingsSingletonAuthoring : MonoBehaviour
{
    public float flightJitter = 10f;

    [Range(0f, 1f)]
    public float damping = 0.01f;

    [Min(0f)]
    public float chaseForce = 2f;

    [Min(0f)]
    public float carryForce = 1f;

    [Min(0f)]
    public float attackForce = 5f;
    
    [Min(0f)]
    public float grabDistance = 0.1f;

    [Min(0f)]
    public float attackDistance = 0.1f;

    [Min(0f)]
    public float hitDistance = 1f;

    [Range(0f, 1f)]
    public float aggressionPercentage = 0f;

    public Vector2 minScale = new Vector2(1.0f, 0.2f);
    public Vector2 maxScale = new Vector2(10.0f, 1.0f);
    public float scaleMultiplier = 1;

    private class BeeSettingsSingletonBaker : Baker<BeeSettingsSingletonAuthoring>
    {
        public override void Bake(BeeSettingsSingletonAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new BeeSettingsSingletonComponent()
            {
                flightJitter = authoring.flightJitter,
                damping = authoring.damping,
                aggressionPercentage = authoring.aggressionPercentage,
                chaseForce = authoring.chaseForce,
                carryForce = authoring.carryForce,
                attackForce = authoring.attackForce,
                grabDistance = authoring.grabDistance,
                attackDistance = authoring.attackDistance,
                hitDistance = authoring.hitDistance,
                minScale = authoring.minScale,
                maxScale = authoring.maxScale,
                scaleMultiplier = authoring.scaleMultiplier
            });
        }
    }
}
