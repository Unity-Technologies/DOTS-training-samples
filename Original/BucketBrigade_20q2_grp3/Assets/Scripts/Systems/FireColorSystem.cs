using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(PresentationSystemGroup))]
[UpdateAfter(typeof(RenderMeshSystemV2))]
public class FireColorSystem : SystemBase
{
    public double UpdateFrequency;

    public JobHandle Deps;

    private double m_LastUpdateTime;

    private FirePropagateSystem m_FirePropagateSystem;

    protected override void OnCreate()
    {
        base.OnCreate();

        m_FirePropagateSystem = World.GetOrCreateSystem<FirePropagateSystem>();
    }

    protected override void OnUpdate()
    {
        if (!GridData.Instance.Heat.IsCreated)
            return;

        Dependency = JobHandle.CombineDependencies(Dependency, m_FirePropagateSystem.Deps);

        if (m_LastUpdateTime + UpdateFrequency < Time.ElapsedTime)
        {
            m_LastUpdateTime = Time.ElapsedTime;

            if (GridUtils.GridPlane != null)
            {
                var data = GridData.Instance;
                // for (int x = 0; x < data.Width; x++)
                //     for (int y = 0; y < data.Height; y++)
                //         GridUtils.GridPlane.Texture.SetPixel(x, y, Color.blue);
                //GridUtils.GridPlane.Texture.Apply();

                var job = new ColorFireJob { Data = data, FromColor = new Color(77/255f, 195/255f, 51/255f), ToColor = Color.red}
                    .Schedule(data.Width * data.Height, data.Width, Dependency);
                job.Complete();
                //new CopyColorFireJob{ Data = data }.Run();

                GridUtils.GridPlane.Texture.SetPixels(data.Color.ToArray());
                GridUtils.GridPlane.Texture.Apply();
                //Dependency = JobHandle.CombineDependencies(Dependency, job2);
            }
            else
            {
                var data = GridData.Instance;
                Deps = Entities
                    .WithName("UpdateFireColor")
                    .WithReadOnly(data)
                    .ForEach((ref Unity.Rendering.MaterialColor color, in GridCell cell) =>
                    {
                        var heat = (float)data.Heat[cell.Index] / byte.MaxValue;
                        var value = (1 - heat) * (1 - heat);
                        color.Value = new float4(1 - value, value, 0, 1);
                    }).ScheduleParallel(Dependency);
                Dependency = Deps;
            }
        }
    }

    [BurstCompile]
    private struct ColorFireJob : IJobParallelFor
    {
        public GridData Data;
        public Color FromColor;
        public Color ToColor;

        public void Execute(int index)
        {
            var heat = (float)Data.Heat[index] / byte.MaxValue;
            //var value = (1 - heat) * (1 - heat);
            //Data.Color[index] = new Color(0.5f - value/2f, value / 2 + 0.2f, 60/255f, 1); //0.5f - (value / 2));

            var t = heat;
            var inverse = 1 - t;
            Data.Color[index] = new Color(FromColor.r * inverse + ToColor.r * t,
                FromColor.g * inverse + ToColor.g * t,
                FromColor.b * inverse + ToColor.b * t,
                (1 - inverse * inverse) / 2+ 0.5f);
        }
    }

    private struct CopyColorFireJob : IJob
    {
        public GridData Data;

        public void Execute()
        {
            for (int y = 0; y < Data.Height; y++)
            for (int x = 0; x < Data.Width; x++)
            {
                GridUtils.GridPlane.Texture.SetPixel(x, y, Data.Color[x + y * Data.Width]);
            }
            GridUtils.GridPlane.Texture.Apply();
        }
    }
}
