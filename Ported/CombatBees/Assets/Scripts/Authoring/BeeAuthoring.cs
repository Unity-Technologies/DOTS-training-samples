using Components;
using DefaultNamespace;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Authoring
{
    public class BeeAuthoring : MonoBehaviour
    {
    }

    public class BeeBaker : Baker<BeeAuthoring>
    {
        public override void Bake(BeeAuthoring authoring)
        {
            AddComponent(new Bee
            {
                State = Beehaviors.Idle
            });

        }
    }
}