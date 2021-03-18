using Components;
using Unity.Entities;
using Unity.Rendering;

namespace Systems
{
    public class ColorPropagateSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var cdfe = GetComponentDataFromEntity<URPMaterialPropertyBaseColor>();

            Entities.WithoutBurst().ForEach((in DynamicBuffer<LinkedEntityGroup> entityGroup, in PropagateColor color) =>
            {
                for (var i = 0; i < entityGroup.Length; i++)
                {
                    cdfe[entityGroup[i].Value] = new URPMaterialPropertyBaseColor() {Value = color.color};
                }
            }).Run();
        }
    }
}