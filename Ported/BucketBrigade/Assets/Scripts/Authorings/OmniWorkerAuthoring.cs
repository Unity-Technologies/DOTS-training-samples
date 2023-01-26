using Unity.Entities;
using Unity.Rendering;
using Unity.Mathematics;
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
            AddComponent<MoveInfo>(new MoveInfo() { destinationPosition = new float2(0,0), speed = 5});
            AddComponent<HasReachedDestinationTag>();
            AddComponent<CarriesBucketTag>();
            SetComponentEnabled<CarriesBucketTag>(GetEntity(), false);
            AddComponent(new OmniWorkerAIState() { omniWorkerState = OmniWorkerState.Idle});
            AddComponent<BucketTargetPosition>();
            AddComponent<URPMaterialPropertyBaseColor>();
        }
    }
}
