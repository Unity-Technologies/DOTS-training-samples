using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;
using Unity.Rendering;

public class BallAuthoring : MonoBehaviour
{
    class BallBaker : Baker<BallAuthoring>
    {
        public override void Bake(BallAuthoring authoring)
        {
            AddComponent<Ball>();
            AddComponent(new Speed
            {
                dragFactor = 0.01f,
                bounceFactor = 0.9f
            });
        }
    }
}

struct Ball : IComponentData
{
    public URPMaterialPropertyBaseColor Color;
}

