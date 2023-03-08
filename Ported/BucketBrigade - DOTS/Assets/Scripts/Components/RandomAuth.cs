using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class RandomAuth : MonoBehaviour
{
    public int randomSeed = 1;
    public class RandomBaker : Baker<RandomAuth>
    {
        public override void Bake(RandomAuth authoring)
        {
            AddComponent(new Random
            {
                Value = new Unity.Mathematics.Random((uint)authoring.randomSeed), // This always has the same seed
            });
        }
    }
}

public struct Random : IComponentData
{
    public Unity.Mathematics.Random Value;
    public int randomSeed;
}
