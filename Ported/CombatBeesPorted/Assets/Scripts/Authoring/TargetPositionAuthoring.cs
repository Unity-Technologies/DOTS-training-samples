using UnityEngine;
using Unity.Entities;

class TargetPositionAuthoring : MonoBehaviour
{
}

class TargetPositionBaker : Baker<TargetPositionAuthoring>
{
    public override void Bake(TargetPositionAuthoring authoring)
    {
        AddComponent<TargetPosition>();
    }
}
