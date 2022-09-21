using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

struct PlayerData : IComponentData
{
    public float3 position;
    public float2 direction;
    public float speed;
    public int StartXIndex;
    public int StartYIndex;




}

readonly partial struct CharacterAspect : IAspect
{
    readonly RefRW<PlayerData> character;


    readonly TransformAspect transform;
    public float speed
    {
        get =>character.ValueRW.speed;
        set => character.ValueRW.speed = value;
    }
    public float3 position
    {
        get => transform.Position;
        set => transform.Position = value;
    }
    public int StartXIndex
    {
        get=> character.ValueRO.StartXIndex;
        
    }
    public int StartYIndex
    {
        get => character.ValueRO.StartYIndex;
    }
}
