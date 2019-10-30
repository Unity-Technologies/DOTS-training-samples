using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

public class JobBase : JobComponentSystem
{
	EntityQuery m_Group;


	protected override void OnCreate()
	{
		m_Group = GetEntityQuery(typeof(Translation), 
								 ComponentType.ReadOnly<MoveUp_Component>(),
								 ComponentType.ReadOnly<InitialPosition>());
	}


	[BurstCompile]
	struct TheJob : IJobChunk
	{

		public float time;
		[ReadOnly] public ArchetypeChunkComponentType<MoveUp_Component> MoveUpType;
		[ReadOnly] public ArchetypeChunkComponentType<InitialPosition> InitialPosType;
		public ArchetypeChunkComponentType<Translation> TranslationType;

		public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
		{
			var moveUpChunk = chunk.GetNativeArray(MoveUpType);
			var iPosChunk = chunk.GetNativeArray(InitialPosType);
			var transChunk = chunk.GetNativeArray(TranslationType);

			for(var i = 0; i < chunk.Count; i++)
			{
				var moveUpData = moveUpChunk[i];
				var iPosData = iPosChunk[i];

				float increment = moveUpData.Amplitude * math.sin(2 * math.PI * time * moveUpData.Frequency);
				transChunk[i] = new Translation
				{
					Value = iPosData.Value + increment * math.up()
				};
			}


		}
	}
	
	protected override JobHandle OnUpdate(JobHandle inputDependencies)
	{

		var moveUpType = GetArchetypeChunkComponentType<MoveUp_Component>(true);
		var iPosType = GetArchetypeChunkComponentType<InitialPosition>(true);
		var transType = GetArchetypeChunkComponentType<Translation>();

		var job = new TheJob
		{
			time = Time.time,
			MoveUpType = moveUpType,
			InitialPosType = iPosType,
			TranslationType = transType
		};

		return job.Schedule(m_Group, inputDependencies);
	}
}