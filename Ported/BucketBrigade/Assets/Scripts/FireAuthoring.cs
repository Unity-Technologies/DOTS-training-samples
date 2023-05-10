using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

public class FireAuthoring : MonoBehaviour
{
    public Color startingColor;
    public Color fireColor;

    class Baker : Baker<FireAuthoring>
    {
        public override void Bake(FireAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic | TransformUsageFlags.NonUniformScale);
            AddComponent<Fire>(entity);
            AddComponent(entity, new Burner { startingColor = (Vector4)authoring.startingColor, fullBurningColor = (Vector4)authoring.fireColor });
            AddComponent(entity, new URPMaterialPropertyBaseColor { Value = (Vector4)authoring.startingColor });
        }
    }
}

public struct Fire : IComponentData
{

}

public struct Burner : IComponentData, IQueryTypeParameter {
    public Vector4 startingColor;
    public Vector4 fullBurningColor;
}
