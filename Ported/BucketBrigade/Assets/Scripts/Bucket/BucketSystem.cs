using Unity.Entities;
using UnityEngine.Rendering;

public struct Owner : IComponentData
{
    public Entity Value;
}
public class BucketSystem : SystemBase
{
    protected override void OnUpdate()
    {
        
    }
}
