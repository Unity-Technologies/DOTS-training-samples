using Unity.Entities;
using UnityEngine;

public class CatAuthoring : MonoBehaviour
{
    public float minSpeed = 0.5f;
    public float maxSpeed = 2.0f;
}

class CatBaker : Baker<CatAuthoring>
{
    public override void Bake(CatAuthoring authoring)
    {
        AddComponent<PositionComponent>();
        AddComponent(new UnitMovementComponent(){ direction = MovementDirection.East, speed = Random.Range(authoring.minSpeed, authoring.maxSpeed) });
    }
}