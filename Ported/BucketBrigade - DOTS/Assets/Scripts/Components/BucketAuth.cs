using Unity.Entities;
using Unity.Rendering;


class BucketAuth : UnityEngine.MonoBehaviour
{
    public float MaxCapacity;
        
    class BucketAuthBaker : Baker<BucketAuth>
    {
        public override void Bake(BucketAuth authoring)
        {
            AddComponent(new Bucket
            {
                MaxCapacity = authoring.MaxCapacity,
                CurrCapacity = 0
            });
            AddComponent<FillingTag>();
            AddComponent<EmptyingTag>();
            AddComponent<EmptyTag>();
            AddComponent<FullTag>();
            AddComponent<FreeTag>();
            AddComponent<URPMaterialPropertyBaseColor>();
        }
    }

}
    
public struct Bucket : IComponentData
{
    public float MaxCapacity;
    public float CurrCapacity;
}
    
public struct FillingTag : IComponentData, IEnableableComponent
{
}

public struct EmptyingTag : IComponentData, IEnableableComponent
{
}
    
public struct EmptyTag : IComponentData, IEnableableComponent
{
}
    
public struct FullTag : IComponentData, IEnableableComponent
{
}

public struct FreeTag : IComponentData, IEnableableComponent
{
}
