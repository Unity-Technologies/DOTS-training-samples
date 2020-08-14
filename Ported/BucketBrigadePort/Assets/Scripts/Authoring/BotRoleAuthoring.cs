using Unity.Entities;

public struct BotRoleFinder : IComponentData
{
    Entity Dependent;
}
public struct BotRoleTosser : IComponentData
{
    Entity Dependent;
}
public struct BotRoleFiller : IComponentData
{
    Entity Dependent;
}
public struct BotRolePasserFull : IComponentData
{
    Entity Dependent;
}
public struct BotRolePasserEmpty : IComponentData
{
    Entity Dependent;
}
public struct BotRoleOmni : IComponentData
{
}