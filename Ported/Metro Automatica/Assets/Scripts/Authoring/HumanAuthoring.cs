using System.Security.Cryptography.X509Certificates;
using Unity.Entities;

class HumanAuthoring : UnityEngine.MonoBehaviour
{
    class Baker : Baker<HumanAuthoring>
    {
        public override void Bake(HumanAuthoring authoring)
        {
            AddComponent<Human>();
        }
    }
}