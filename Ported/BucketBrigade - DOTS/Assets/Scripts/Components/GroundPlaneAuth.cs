using Unity.Entities;
using UnityEngine;

public class GroundPlaneAuth : MonoBehaviour
{
    public class Baker : Baker<GroundPlaneAuth>
    {
        public override void Bake(GroundPlaneAuth authoring)
        {
            AddComponent<GroundPlaneTag>();
        }
    }
}

public struct GroundPlaneTag : IComponentData { }
