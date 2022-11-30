using Unity.Entities;
using UnityEngine;

public class RatAuthoring : MonoBehaviour
{
    public float minSpeed = 3.0f;
    public float maxSpeed = 5.0f;
}

class RatBaker : Baker<RatAuthoring>
{
    public override void Bake(RatAuthoring authoring)
    {
        AddComponent<IsAliveComponent>();
        AddComponent<PositionComponent>();
        AddComponent(new UnitMovementComponent(){ direction = MovementDirection.East, speed = Random.Range(authoring.minSpeed, authoring.maxSpeed) });
    }
}