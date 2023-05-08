using System;
using Unity.Entities;
using UnityEngine;

public class RandomAuth : MonoBehaviour
{
    private readonly int _randomSeed = DateTime.Now.Ticks.GetHashCode();
    public class RandomBaker : Baker<RandomAuth>
    {
        public override void Bake(RandomAuth authoring)
        {
            AddComponent(new Random
            {
                Value = new Unity.Mathematics.Random((uint)authoring._randomSeed)
            });
        }
    }
}

public struct Random : IComponentData
{
    public Unity.Mathematics.Random Value;
}
