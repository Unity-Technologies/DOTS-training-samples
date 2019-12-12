
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public struct AntInwardOutwardStrengthJob : IJobForEach<AntComponent, Translation>
{
	[ReadOnly] public float InwardStrength;
	[ReadOnly] public float OutwardStrength;
	[ReadOnly] public float2 ColonyPosition;
	
	public void Execute(ref AntComponent ant, [ReadOnly] ref Translation translation)
	{
		float3 antPosition = translation.Value;
		float inwardOrOutward = -OutwardStrength;
		float pushRadius = .4f;
		
		if (ant.state == 1) {
			inwardOrOutward = InwardStrength;
			pushRadius = 1.0f;
		}
		
		float dist = math.distance(ColonyPosition, antPosition.xy);
		inwardOrOutward *= 1.0f - math.clamp(dist / pushRadius, 0.0f, 1.0f);
		ant.speed += dist * inwardOrOutward;
	}
}
