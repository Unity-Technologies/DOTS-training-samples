using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class PheromonesPresentationAuthority : MonoBehaviour
{
    class Baker : Baker<PheromonesPresentationAuthority>
    {
        public override void Bake(PheromonesPresentationAuthority authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Renderable);
            AddComponent(entity, new PheromonesPresentationTexture());
        }
    }
}

public struct PheromonesPresentationTexture : IComponentData
{
}
