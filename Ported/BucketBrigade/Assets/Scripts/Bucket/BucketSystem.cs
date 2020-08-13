using Unity.Entities;
using Unity.Mathematics;
using UnityEngine.Rendering;

public struct Owner : IComponentData
{
    public Entity Value;
}
public class BucketSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities
            .WithAll<Bucket>()
            .WithNone<Color>()
            .WithStructuralChanges()
            .ForEach((Entity e) =>
            {
                EntityManager.AddComponent<Color>(e);
            }).Run();
        // update the color of the bucket based on the fill amount
        Entities
            .ForEach((Entity e, ref Color c, in Water w, in Bucket b) =>
            {
                c.Value = math.lerp(b.emptyColor, b.fullColor, w.volume / w.capacity);
            }).Schedule();
        
    }
}
