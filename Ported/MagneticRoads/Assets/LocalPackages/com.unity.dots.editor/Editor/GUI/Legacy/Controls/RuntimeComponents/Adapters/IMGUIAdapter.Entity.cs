using Unity.Entities;
using Unity.Properties;
using Unity.Properties.Adapters;

namespace Unity.Editor.Legacy
{
    sealed partial class RuntimeComponentsDrawer :
        IVisit<Entity>
    {
        public VisitStatus Visit<TContainer>(Property<TContainer, Entity> property, ref TContainer container, ref Entity value)
        {
            LabelField(property, $"Index: <b>{value.Index}</b> Version: <b>{value.Version}</b>", IsMixedValue(property, value));
            return VisitStatus.Stop;
        }
    }
}
