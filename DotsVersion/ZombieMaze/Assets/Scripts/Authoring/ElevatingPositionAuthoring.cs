
    using Unity.Entities;
    using UnityEngine;

    public class ElevatingPositionAuthoring: MonoBehaviour
    {
    }
    class ElevatingPositionBaker : Baker<ElevatingPositionAuthoring>
    {
        public override void Bake(ElevatingPositionAuthoring authoring)
        {
            AddComponent(new ElevatingPosition
            {
                
            });
        }
    }
