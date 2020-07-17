using Unity.Entities;
using Unity.Transforms;

namespace Water
{
    public class BucketFollowAttachedEntitySystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var leaderTranslationLookup = GetComponentDataFromEntity<Translation>(true);
            Entities
                .WithReadOnly(leaderTranslationLookup)
                .WithNativeDisableContainerSafetyRestriction(leaderTranslationLookup)
                .ForEach((Entity Entity, ref Translation translation, in Attached attachedEntity) =>
                {
                    var leaderTranslation = leaderTranslationLookup[attachedEntity.Value];
                    translation.Value = attachedEntity.Offset + leaderTranslation.Value;
                }).Schedule();
        }
    }
}