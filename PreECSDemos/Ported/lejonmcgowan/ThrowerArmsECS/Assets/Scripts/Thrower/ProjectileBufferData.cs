using Unity.Entities;

[InternalBufferCapacity(20)]
public struct ProjectileBufferData: IBufferElementData
{
    public static implicit operator ProjectileComponentData(ProjectileBufferData e) { return e.Value; }
    public static implicit operator ProjectileBufferData(ProjectileComponentData e) { return new ProjectileBufferData() { Value = e }; }
    
    public  ProjectileComponentData Value;
    
    
}