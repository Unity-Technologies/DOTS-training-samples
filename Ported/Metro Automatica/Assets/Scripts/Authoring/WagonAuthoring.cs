using System.Security.Cryptography.X509Certificates;
using Unity.Entities;

class WagonAuthoring : UnityEngine.MonoBehaviour
{
    class WagonBaker : Baker<WagonAuthoring>
    {
        public override void Bake(WagonAuthoring authoring)
        {
            AddComponent<Wagon>();
        }
    }
}