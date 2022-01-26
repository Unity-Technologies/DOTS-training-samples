using Unity.Entities;
using Unity.Mathematics;
using MonoBehaviour = UnityEngine.MonoBehaviour;

public class PlayerAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public UnityEngine.Color color;
    public bool UserControlled;
    public int Index;
    public uint AISeed;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponent<Player>(entity);
        dstManager.AddComponentData(entity, new Color { Value = new float4(color.r, color.g, color.b, color.a) });
        dstManager.AddComponentData(entity, new CursorPosition { Value = float2.zero });
        dstManager.AddComponentData(entity, new Score { Value = 0 });
        dstManager.AddComponentData(entity, new PlayerSpawnArrow { Value = false });
        dstManager.AddComponentData(entity, new PlayerUIIndex { Index = Index });
        dstManager.AddComponent<ArrowsDeployed>(entity);
        if (UserControlled)
        {
            dstManager.AddComponent<PlayerInputTag>(entity);
        }
        else
        {
            dstManager.AddComponentData(entity, new PlayerAIControlled { Random = new Random(AISeed) });
            dstManager.AddComponentData(entity, new CursorLerp { Start = float2.zero, Destination = float2.zero, LerpValue = 1f });
        }
    }
}
