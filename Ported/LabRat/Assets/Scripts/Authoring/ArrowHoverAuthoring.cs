using Unity.Entities;
using Unity.Mathematics;
using MonoBehaviour = UnityEngine.MonoBehaviour;

public class ArrowHoverAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public UnityEngine.Color color;
    public bool UserControlled;
    public int Index;
    public uint AISeed;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponent<Player>(entity);
        dstManager.AddComponentData(entity, new CursorPosition { Value = float2.zero });
    }
}
