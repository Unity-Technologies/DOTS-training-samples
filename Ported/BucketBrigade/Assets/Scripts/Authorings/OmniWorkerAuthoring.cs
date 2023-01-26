using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

public class OmniWorkerAuthoring : MonoBehaviour
{
    class Baker : Baker<OmniWorkerAuthoring>
    {
        public override void Bake(OmniWorkerAuthoring authoring)
        {
            AddComponent<Position>();
            AddComponent<Target>();
            AddComponent<CarriedBucket>();
            AddComponent<MoveInfo>();
            AddComponent<HasReachedDestinationTag>();
            AddComponent<CarriesBucketTag>();
            SetComponentEnabled<CarriesBucketTag>(GetEntity(), false);
            AddComponent(new OmniWorkerAIState() { omniWorkerState = OmniWorkerState.Idle});
            AddComponent<BucketTargetPosition>();
            AddComponent<URPMaterialPropertyBaseColor>();
        }
    }
}
