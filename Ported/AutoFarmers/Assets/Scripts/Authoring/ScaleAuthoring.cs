using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

class ScaleAuthoring : UnityEngine.MonoBehaviour
{
    public float InitialScale = 1f;
}

class ScaleBaking : Baker<ScaleAuthoring>
{
    public override void Bake(ScaleAuthoring authoring)
    {
        AddComponent(new Scale
        {
            Value = authoring.InitialScale,
        });
    }
}