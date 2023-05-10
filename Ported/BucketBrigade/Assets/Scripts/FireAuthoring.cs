using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

public class FireAuthoring : MonoBehaviour
{
    public Color startingColor;
    public int StartFireNumber;

    class Baker : Baker<FireAuthoring>
    {
        public override void Bake(FireAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic | TransformUsageFlags.NonUniformScale);
            AddComponent<Fire>(entity);
            AddComponent<Burner>(entity);
            AddComponent(entity, new URPMaterialPropertyBaseColor { Value = (Vector4)authoring.startingColor });
        }
    }
}

public struct Fire : IComponentData
{

}

public struct Burner : IComponentData, IEnableableComponent {
    // (the component should actually be empty, but this dummy field
    // was added as workaround for a bug in source generation)
    public float Value;
}
