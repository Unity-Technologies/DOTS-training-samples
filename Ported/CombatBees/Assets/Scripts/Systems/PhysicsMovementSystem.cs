using Unity.Entities;
using Unity.Collections;
using Unity.Transforms;

public partial class PhysicsMovementSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        float deltaTime = Time.DeltaTime;
        float multiplier = 15f;
        float floorY = 0.5f;
        
        Entities
            .WithAll<Food>()
            .WithNone<Grounded>()
            .ForEach((Entity food, ref Translation translation) =>
            {

                translation.Value.y -= deltaTime * multiplier;
                if (translation.Value.y < floorY)
                {
                    translation.Value.y = floorY;// this assume 0 is at the basis of the cylinder
                    ecb.AddComponent(food, new Grounded{});
                }
            }).Run();
    }
}