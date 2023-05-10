using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

public class FireAuthoring : MonoBehaviour {
    class Baker : Baker<FireAuthoring> {
        public override void Bake(FireAuthoring authoring) {
            var entity = GetEntity(TransformUsageFlags.Dynamic | TransformUsageFlags.NonUniformScale);
            AddComponent(entity, new Fire { });
            AddComponent(entity, new URPMaterialPropertyBaseColor { });
        }
    }
}

public struct Fire : IComponentData {
    public float t;         // Fire value from 0.0 -> 1.0
    public float random;    // Random variance for animation
}
