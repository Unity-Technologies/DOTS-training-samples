using Unity.Entities;
using Unity.Mathematics;


struct PlayerData : IComponentData
{
    public float2 position;
    public float2 direction;
    public float speed;

}
