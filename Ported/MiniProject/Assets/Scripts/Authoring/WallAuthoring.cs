using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;

public class WallAuthoring : MonoBehaviour
{
    class WallBaker : Baker<WallAuthoring>
    {
        public override void Bake(WallAuthoring authoring)
        {
            AddComponent<Wall>();
        }
    }
}

struct Wall : IComponentData
{
}

