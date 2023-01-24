using Unity.Entities;
using UnityEngine;

public class BucketPasserAuthoring : MonoBehaviour
{
    class Baker : Baker<BucketPasserAuthoring>
    {
        public override void Bake(BucketPasserAuthoring authoring)
        {
            // todo add role
            AddComponent<Position>();
            AddComponent<IndexInLine>();
            AddComponent<TeamInfo>();
            AddComponent<Target>();
            AddComponent<CarriedBucket>();
        }
    }
}
