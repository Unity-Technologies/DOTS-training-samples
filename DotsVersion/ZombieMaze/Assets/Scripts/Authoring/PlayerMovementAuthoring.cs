using Unity.Entities;
using Unity.Mathematics;


public class PlayerMovementAuthoring : UnityEngine.MonoBehaviour
{
    public float2 direction;
    public float speed = 15;
    public float3 position;
    public float3 spawnerPos;

}

class PlayerMovementBaker : Baker<PlayerMovementAuthoring>
{
    public override void Bake(PlayerMovementAuthoring authoring)
    {
        AddComponent(new PlayerData
        {
            speed = authoring.speed,
            direction = authoring.direction,
            position = authoring.position,
            spawnerPos=authoring.spawnerPos,
        });
    }
}
