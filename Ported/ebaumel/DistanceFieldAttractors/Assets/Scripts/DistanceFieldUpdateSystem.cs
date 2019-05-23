using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Burst;

[UpdateBefore(typeof(ParticleUpdateSystem))]
public class DistanceFieldUpdateSystem : ComponentSystem
{
    EntityQuery m_DistanceFields;
    private int m_ModelCount;
    protected override void OnCreateManager()
    {
        m_DistanceFields = GetEntityQuery(ComponentType.ReadWrite<DistanceField>());
        m_ModelCount = System.Enum.GetValues(typeof(DistanceFieldModel)).Length;
    }

    Unity.Mathematics.Random randSeed = new Unity.Mathematics.Random(747);
    protected override void OnUpdate()
    {
        var distanceField = GetSingleton<DistanceField>();

        // Switch to a new distance field model after 10 seconds.
        if (distanceField.switchTimerValue > 0f)
        {
            distanceField.switchTimerValue -= Time.deltaTime;
        }
        else
        {
            distanceField.switchTimerValue = distanceField.switchTimer;
            distanceField.model = (DistanceFieldModel)randSeed.NextInt(0, m_ModelCount - 1);
        }

        SetSingleton(distanceField);

    }
}
