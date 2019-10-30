using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

class MoveAndRedirectSystem : JobComponentSystem
{
    EntityQuery m_Query;

    struct MoveJob : IJobChunk
    {
        public float DeltaTime;
        public ArchetypeChunkComponentType<Translation> TranslationType;
        public ArchetypeChunkComponentType<Direction> DirectionType;
        public ArchetypeChunkComponentType<Speed> SpeedType;

        public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
        {
            var chunkTranslations = chunk.GetNativeArray(TranslationType);
            var chunkDirections = chunk.GetNativeArray(DirectionType);
            var chunkSpeeds = chunk.GetNativeArray(SpeedType);

            for (var i = 0; i < chunk.Count; i++)
            {
                var translation = chunkTranslations[i];
                var direction = chunkDirections[i];
                var speed = chunkSpeeds[i];

                chunkTranslations[i] = new Translation
                {
                    Value = translation.Value + DeltaTime * speed.Value * direction.Value.GetWorldDirection()
                };
            }
        }
    }

    protected override void OnCreate()
    {
        m_Query = EntityManager.CreateEntityQuery(ComponentType.ReadWrite<Direction>(), ComponentType.ReadWrite<Translation>(), ComponentType.ReadWrite<Speed>());
    }

    protected override JobHandle OnUpdate(JobHandle deps)
    {
        var job = new MoveJob()
        {
            TranslationType = GetArchetypeChunkComponentType<Translation>(false),
            DirectionType = GetArchetypeChunkComponentType<Direction>(false),
            SpeedType = GetArchetypeChunkComponentType<Speed>(false),
            DeltaTime = Time.deltaTime
        };
        return job.Schedule(m_Query, deps);
    }
}
