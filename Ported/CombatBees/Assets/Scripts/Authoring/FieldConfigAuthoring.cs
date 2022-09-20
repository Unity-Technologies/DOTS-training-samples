using Unity.Entities;
using Unity.Mathematics;

class FieldConfigAuthoring : UnityEngine.MonoBehaviour
{
    public UnityEngine.GameObject FieldMesh;
    public float3 FieldScale = new float3(100f,20f,30f);
    public float FieldGravity = -20f;
}

class FieldConfigBaker : Baker<FieldConfigAuthoring>
{
    public override void Bake(FieldConfigAuthoring authoring)
    {
        AddComponent(new FieldConfig
        {
            FieldMesh = GetEntity(authoring.FieldMesh),
            FieldScale = authoring.FieldScale,
            FieldGravity = authoring.FieldGravity,
        });
    }
}
