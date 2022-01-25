using Unity.Entities;
using Unity.Mathematics;
using MonoBehaviour = UnityEngine.MonoBehaviour;

public class PlayerAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public UnityEngine.Color color;
    public bool UserControlled;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponent<Player>(entity);
        dstManager.AddComponentData(entity, new Color { Value = new float4(color.r, color.g, color.b, color.a) });
        dstManager.AddComponentData(entity, new CursorPosition { Value = new float2(0f, 0f) });
        dstManager.AddComponentData(entity, new Score { Value = 0 });
        dstManager.AddComponentData(entity, new PlayerSpawnArrow { Value = false });
        dstManager.AddComponent<ArrowsDeployed>(entity);
        if (UserControlled)
        {
            dstManager.AddComponent<PlayerInputTag>(entity);
        }
    }
}
