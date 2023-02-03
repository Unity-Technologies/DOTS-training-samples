using System.Security.Cryptography.X509Certificates;
using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;

class HumanAuthoring : UnityEngine.MonoBehaviour
{
    class Baker : Baker<HumanAuthoring>
    {
        public override void Bake(HumanAuthoring authoring)
        {
            AddComponent<Human>();
            AddComponent<PostTransformScale>();
            AddComponent<URPMaterialPropertyBaseColor>();
            AddBuffer<BridgeRouteWaypoint>();
            AddComponent<HumanWaitForRouteTag>();
        }
    }
}