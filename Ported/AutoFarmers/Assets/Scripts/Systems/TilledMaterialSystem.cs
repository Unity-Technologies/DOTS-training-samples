using Unity.Entities;
using UnityEngine;

namespace AutoFarmers
{
    class TilledMaterialSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.WithNone<Tilled>().ForEach((ref TilledMaterialProperty tilledMaterialProperty) =>
            {
                tilledMaterialProperty.Value = 0f;
            }).ScheduleParallel();
            
            Entities.WithAll<Tilled>().ForEach((ref TilledMaterialProperty tilledMaterialProperty) =>
            {
                tilledMaterialProperty.Value = 1f;
            }).ScheduleParallel();
        }
    }
}