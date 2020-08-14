using Unity.Entities;
using Unity.Mathematics;

public struct BotRoleFinder : IComponentData
{
    public Entity Dependent;
}
public struct BotRoleTosser : IComponentData
{
    public Entity Dependent;
    public Entity BotFiller;
}
public struct BotRoleFiller : IComponentData
{
    public Entity Dependent;
}
public struct BotRolePasserFull : IComponentData
{
    public Entity Dependent;
}
public struct BotRolePasserEmpty : IComponentData
{
    public Entity Dependent;
}
public struct BotRoleOmni : IComponentData
{
}