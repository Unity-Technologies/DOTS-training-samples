using Unity.Entities;
using Unity.Transforms;

namespace Water
{
    public class BucketFollowAttachedEntitySystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var leaderTranslationLookup = GetComponentDataFromEntity<Translation>(true);
            var ltwLookup = GetComponentDataFromEntity<LocalToWorld>(true);
            Entities
                .WithReadOnly(ltwLookup)
                // .WithNativeDisableContainerSafetyRestriction(leaderTranslationLookup)
                .ForEach((ref Translation translation, in Attached attachedEntity) =>
                {
                    // var leaderTranslation = leaderTranslationLookup[attachedEntity.Value];
                    var leaderTranslation = ltwLookup[attachedEntity.Value].Position;
                    translation.Value = attachedEntity.Offset + leaderTranslation;
                }).ScheduleParallel();
        }
    }
}