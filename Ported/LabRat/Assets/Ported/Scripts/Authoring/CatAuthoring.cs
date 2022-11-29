using Unity.Entities;
using UnityEngine;

public class CatAuthoring : MonoBehaviour
{

}

class CatBaker : Baker<CatAuthoring>
{
    public override void Bake(CatAuthoring authoring)
    {
        AddComponent<PositionComponent>();
        AddComponent(new UnitMovementComponent(){ direction = MovementDirection.East, speed = 0.65f });
    }
}