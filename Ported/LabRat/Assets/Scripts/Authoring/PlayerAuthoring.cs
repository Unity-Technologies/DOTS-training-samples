using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using MonoBehaviour = UnityEngine.MonoBehaviour;

public class PlayerAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public UnityEngine.Color color;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new Color { Value = new float4(color.r, color.g, color.b, color.a) });
        dstManager.AddComponentData<Position>(entity, new Position { Value = new float2(0f, 0f) });
        dstManager.AddComponentData(entity, new Score { Value = 0 });
        dstManager.AddComponentData(entity, new PlayerSpawnArrow { Value = false });
    }
}
