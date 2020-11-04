using Unity.Entities;

namespace MetroECS.Comuting
{
    [GenerateAuthoringComponent]
    public struct LineID : IComponentData
    {
        public int Value;
    }
}