using Unity.Entities;
using Unity.Transforms;

[GenerateAuthoringComponent]
public struct NewTranslation: IComponentData
{
    public Translation translation;
}
