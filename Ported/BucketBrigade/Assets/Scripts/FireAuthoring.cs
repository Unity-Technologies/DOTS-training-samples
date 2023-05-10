using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

public class FireAuthoring : MonoBehaviour {
    public Color startingColor;
    public Color fireColor;

    class Baker : Baker<FireAuthoring> {
        public override void Bake(FireAuthoring authoring) {
            var entity = GetEntity(TransformUsageFlags.Dynamic | TransformUsageFlags.NonUniformScale);
            AddComponent<Fire>(entity);
            AddComponent(entity, new URPMaterialPropertyBaseColor { });
        }
    }
}

public struct Fire : IComponentData {
    public float t;
}
