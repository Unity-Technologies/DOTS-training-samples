using System.Security.Cryptography.X509Certificates;
using Unity.Entities;

class StationAuthoring : UnityEngine.MonoBehaviour
{
    class StationBaker : Baker<StationAuthoring>
    {
        public override void Bake(StationAuthoring authoring)
        {
            AddComponent<Station>();
        }
    }
}

struct Station : IComponentData
{
   // public Entity StationPrefab;
}