using Unity.Entities;
using UnityEngine;

public class CatAuthoring : MonoBehaviour
{

}

class CatBaker : Baker<CatAuthoring>
{
    public override void Bake(CatAuthoring authoring)
    {
        AddComponent<UnitMovementComponent>();
        AddComponent<PositionComponent>();
    }
}