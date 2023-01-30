using System.Security.Cryptography.X509Certificates;
using Unity.Entities;
using Unity.Transforms;

class RailAuthoring : UnityEngine.MonoBehaviour
{
    class RailBaker : Baker<RailAuthoring>
    {
        public override void Bake(RailAuthoring authoring)
        {
            AddComponent<Rail>();
            AddComponent<PostTransformScale>();
        }
    }
}

struct Rail : IComponentData
{
    // public Entity StationPrefab;
}
