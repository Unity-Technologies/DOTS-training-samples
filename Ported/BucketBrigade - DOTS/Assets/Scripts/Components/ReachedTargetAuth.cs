using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class ReachedTargetAuth : MonoBehaviour
{
    class ReachedTargetBaker : Baker<ReachedTargetAuth>
    {
        public override void Bake(ReachedTargetAuth authoring)
        {
            AddComponent<ReachedTarget>();
        }
    }
}

public struct ReachedTarget : IComponentData, IEnableableComponent
{
    
}
