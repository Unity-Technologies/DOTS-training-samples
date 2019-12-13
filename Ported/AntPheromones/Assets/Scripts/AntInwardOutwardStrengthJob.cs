using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
public struct AntInwardOutwardStrengthJob : IJobForEach<AntComponent, Translation>
{
	[ReadOnly] public float InwardStrength;
	[ReadOnly] public float OutwardStrength;
	[ReadOnly] public float2 ColonyPosition;
	
	public void Execute(ref AntComponent ant, [ReadOnly] ref Translation translation)
	{
		float2 antPosition = translation.Value.xy;
		float inwardOrOutward = -OutwardStrength;
		float pushRadius = .4f;
		
		if (ant.state == 1) {
			inwardOrOutward = InwardStrength;
			pushRadius = 1.0f;
		}
		
		float2 delta = ColonyPosition - antPosition;
		float dist = math.lengthsq(delta);
		inwardOrOutward *= 1.0f - math.clamp(dist / pushRadius, 0.0f, 1.0f);
		ant.speed += dist * inwardOrOutward;
		ant.velocity = delta / dist * inwardOrOutward;
	}
}
