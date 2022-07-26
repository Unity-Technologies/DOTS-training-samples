using Components;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Authoring
{
    public class LaneAuthoring : MonoBehaviour
    {
        
    }

    public class LaneBaker : Baker<LaneAuthoring>
    {
        public override void Bake(LaneAuthoring authoring)
        {
            AddComponent<Lane>();
        }
    }
}
