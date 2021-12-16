using CombatBees.Testing.BeeFlight;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace CombatBees.Testing.BeeFlight
{
    public partial class ResourceMovement : SystemBase
    {
        protected override void OnCreate()
        {
            RequireSingletonForUpdate<SingeltonBeeMovement>();
        }
        
        protected override void OnUpdate()
        {
            var allTranslations = GetComponentDataFromEntity<Translation>(true);

            Entities.WithAll<Resource>().WithNativeDisableContainerSafetyRestriction(allTranslations).ForEach(
                (ref Translation translation, in Holder holder) =>
                {
                    if (holder.Value != Entity.Null)
                    {
                        Debug.Log(allTranslations[holder.Value].Value);
                        translation.Value = allTranslations[holder.Value].Value;
                    }
                }).Schedule();
        }
    }
}