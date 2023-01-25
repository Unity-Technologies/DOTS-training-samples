using Unity.Entities;
using UnityEngine;

public class BucketFetcherAuthoring : MonoBehaviour
{
    class Baker : Baker<BucketFetcherAuthoring>
    {
        public override void Bake(BucketFetcherAuthoring authoring)
        {
            // todo add role
            AddComponent<Position>();
            AddComponent<TeamInfo>();
            AddComponent<Target>();
            AddComponent<CarriedBucket>();
            AddComponent<MoveInfo>();
        }
    }
}
