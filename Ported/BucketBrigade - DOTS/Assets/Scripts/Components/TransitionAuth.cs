using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class TransitionAuth : MonoBehaviour
{
    public class TransitionBaker : Baker<TransitionAuth>
    {
        public override void Bake(TransitionAuth authoring)
        {
            AddComponent<Transition>();
        }
    }
}


public struct Transition : IComponentData
{
    
}

public struct botSpawnCompleteTag : IComponentData
{
    
}

public struct tileSpawnCompleteTag : IComponentData
{
    
}



public struct botChainCompleteTag : IComponentData
{
}

