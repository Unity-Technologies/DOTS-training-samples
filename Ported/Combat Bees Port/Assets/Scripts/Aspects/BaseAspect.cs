using DefaultNamespace;
using Unity.Entities;

readonly partial struct BaseAspect : IAspect<BaseAspect>
{
    readonly RefRO<Base> m_BeeBaseData;

    public BasePosition BlueBase => m_BeeBaseData.ValueRO.blueBase;
    public BasePosition YellowBase => m_BeeBaseData.ValueRO.yellowBase;
}
