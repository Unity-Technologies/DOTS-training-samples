using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Authoring
{
    public class RandomAuthoring : MonoBehaviour
    {
        public bool useRandomInitValue = true;
    }

    public class RandomBaker : Baker<RandomAuthoring>
    {
        public override void Bake(RandomAuthoring authoring)
        {
            uint randInitVal = authoring.useRandomInitValue ? (uint)System.DateTime.Now.Millisecond : 1;
            AddComponent(new RandomComponent()
            {
                random = new Unity.Mathematics.Random(randInitVal)
            });
        }
    }
}