using Unity.Entities;
using Unity.Mathematics;

public enum DistanceFieldModel
{
    SpherePlane,
    Metaballs,
    SpinMixer,
    SphereField,
    FigureEight,
    PerlinNoise,
}

public struct DistanceField : IComponentData
{
    public DistanceFieldModel Value;
    public float SwitchCooldown;
    public Random rng;
}

public class DistanceFieldAuthoring : UnityEngine.MonoBehaviour, IConvertGameObjectToEntity
{
    public DistanceFieldModel model = DistanceFieldModel.FigureEight;
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new DistanceField { Value = model, SwitchCooldown = 0f, rng = new Random(11906237) });
    }
}
