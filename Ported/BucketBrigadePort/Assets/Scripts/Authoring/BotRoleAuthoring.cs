using Unity.Entities;
using Unity.Mathematics;

public struct BotRoleFinder : IComponentData
{
}
public struct BotRoleTosser : IComponentData
{
    public Entity BotFiller;
}
public struct BotRoleFiller : IComponentData
{
}
public struct BotRolePasserFull : IComponentData
{
}
public struct BotRolePasserEmpty : IComponentData
{
}
public struct BotRoleOmni : IComponentData
{
}