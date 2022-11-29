using Unity.Entities;
using UnityEngine;

public class RatAuthoring : MonoBehaviour
{

}

class RatBaker : Baker<RatAuthoring>
{
    public override void Bake(RatAuthoring authoring)
    {
        AddComponent<IsAliveComponent>();
        AddComponent<PositionComponent>();
        AddComponent(new UnitMovementComponent(){ direction = MovementDirection.East, speed = 1.0f });
    }
}