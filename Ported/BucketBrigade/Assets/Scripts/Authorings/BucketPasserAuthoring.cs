using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

public class BucketPasserAuthoring : MonoBehaviour
{
    class Baker : Baker<BucketPasserAuthoring>
    {
        public override void Bake(BucketPasserAuthoring authoring)
        {
            // todo add role
            AddComponent<Position>();
            AddComponent<LineInfo>();
            AddComponent<TeamInfo>();
            AddComponent<Target>();
            AddComponent<CarriedBucket>();
            AddComponent<MoveInfo>();
            AddComponent<URPMaterialPropertyBaseColor>();
        }
    }
}
