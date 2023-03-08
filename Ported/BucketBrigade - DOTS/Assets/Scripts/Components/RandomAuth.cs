using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class RandomAuth : MonoBehaviour
{
    public class RandomBaker : Baker<RandomAuth>
    {
        public override void Bake(RandomAuth authoring)
        {
            AddComponent(new Random
            {
                Value = new Unity.Mathematics.Random(5), // This always has the same seed
            });
        }
    }
}

public struct Random : IComponentData
{
    public Unity.Mathematics.Random Value;
}
