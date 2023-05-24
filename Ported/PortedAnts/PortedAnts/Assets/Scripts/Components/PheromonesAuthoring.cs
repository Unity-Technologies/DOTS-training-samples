using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Components
{
    public class PheromoneAuthoring : MonoBehaviour
    {

    }

    class PheromoneBaker : Baker<PheromoneAuthoring>
    {
        public override void Bake(PheromoneAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddBuffer<Pheromone>(entity);
        }
    }
    
    public struct Pheromone : IBufferElementData
    {
        public byte Value;
    }
}