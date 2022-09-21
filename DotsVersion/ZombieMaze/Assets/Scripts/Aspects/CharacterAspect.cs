using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;


readonly partial struct CharacterAspect : IAspect
{
    readonly RefRW<PlayerData> character;


    readonly TransformAspect transform;
    public float speed
    {
        get => character.ValueRW.speed;
        set => character.ValueRW.speed = value;
    }
    public float3 position
    {
        get => transform.Position;
        set => transform.Position = value;
    }
    public int StartXIndex
    {
        get => character.ValueRO.StartXIndex;

    }
    public int StartYIndex
    {
        get => character.ValueRO.StartYIndex;
    }
    public float3 spawnerPos
    {
        get => character.ValueRW.spawnerPos;
        set => character.ValueRW.spawnerPos = value;
    }
}
