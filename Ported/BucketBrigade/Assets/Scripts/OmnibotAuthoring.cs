using Unity.Entities;
using UnityEngine;

public class OmnibotAuthoring : MonoBehaviour
{
    class Baker : Baker<OmnibotAuthoring>
    {
        public override void Bake(OmnibotAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent<Omnibot>(entity);
        }
    }
}

public struct Omnibot : IComponentData
{

}