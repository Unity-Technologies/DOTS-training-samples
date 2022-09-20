using Unity.Entities;
using Unity.Mathematics;

struct FieldConfig : IComponentData
{
    public Entity FieldMesh;
    public float3 FieldScale;
    public float FieldGravity;
}