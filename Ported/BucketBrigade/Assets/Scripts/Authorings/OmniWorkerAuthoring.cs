using Unity.Entities;
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
        }
    }
}
