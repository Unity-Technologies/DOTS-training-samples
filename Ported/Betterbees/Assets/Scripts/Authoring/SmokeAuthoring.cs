using Unity.Entities;
using UnityEngine;

public class SmokeAuthoring : MonoBehaviour
{
    public float decayRate = 1.5f;

    private class SmokeBaker : Baker<SmokeAuthoring>
    {
        public override void Bake(SmokeAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new VelocityComponent());
            AddComponent(entity, new DecayComponent 
            {
                DecayRate = authoring.decayRate
            });
        }
    }
}
