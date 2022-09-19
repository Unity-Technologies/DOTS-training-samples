using Unity.Entities;
using Unity.Mathematics;

#region step1
// Same approach for the cannon ball, we are creating a component to identify the entities.
// But this time it's not a tag component (empty) because it contains data: the Speed field.
// It won't be used immediately, but will become relevant when we implement motion.
struct CannonBall : IComponentData
{
    public float3 Speed;
}
#endregion