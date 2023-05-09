using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

public class FireAuthoring : MonoBehaviour
{
    class Baker : Baker<FireAuthoring>
    {
        public override void Bake(FireAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic | TransformUsageFlags.NonUniformScale);
            AddComponent<Fire>(entity);
            AddComponent<URPMaterialPropertyBaseColor>(entity);
        }
    }
}

public struct Fire : IComponentData
{

}