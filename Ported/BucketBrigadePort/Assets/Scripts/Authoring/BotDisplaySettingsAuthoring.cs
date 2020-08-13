using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct BotDisplaySettings : IComponentData
{
    public float4 BotRoleFinder;
    public float4 BotRoleTosser;
    public float4 BotRoleFiller;
    public float4 BotRolePasserFull;
    public float4 BotRolePasserEmpty;
    public float4 BotRoleOmni;
}

public class BotDisplaySettingsAuthoring : UnityEngine.MonoBehaviour, IConvertGameObjectToEntity
{
    public UnityEngine.Color ColorBotRoleFinder;
    public UnityEngine.Color ColorBotRoleTosser;
    public UnityEngine.Color ColorBotRoleFiller;
    public UnityEngine.Color ColorBotRolePasserFull;
    public UnityEngine.Color ColorBotRolePasserEmpty;
    public UnityEngine.Color ColorBotRoleOmni;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new BotDisplaySettings
        {
            BotRoleFinder = new float4(ColorBotRoleFinder.r, ColorBotRoleFinder.g, ColorBotRoleFinder.b, ColorBotRoleFinder.a),
            BotRoleTosser = new float4(ColorBotRoleTosser.r, ColorBotRoleTosser.g, ColorBotRoleTosser.b, ColorBotRoleTosser.a),
            BotRoleFiller = new float4(ColorBotRoleFiller.r, ColorBotRoleFiller.g, ColorBotRoleFiller.b, ColorBotRoleFiller.a),
            BotRolePasserFull = new float4(ColorBotRolePasserFull.r, ColorBotRolePasserFull.g, ColorBotRolePasserFull.b, ColorBotRolePasserFull.a),
            BotRolePasserEmpty = new float4(ColorBotRolePasserEmpty.r, ColorBotRolePasserEmpty.g, ColorBotRolePasserEmpty.b, ColorBotRolePasserEmpty.a),
            BotRoleOmni = new float4(ColorBotRoleOmni.r, ColorBotRoleOmni.g, ColorBotRoleOmni.b, ColorBotRoleOmni.a)
        });
    }
}