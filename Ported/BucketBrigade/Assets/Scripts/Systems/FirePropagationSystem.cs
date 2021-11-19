using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;
using System;
using UnityEngine;

public partial class FirePropagationSystem : SystemBase
{
    // TODO : add kXYZ in front of const
    #region Constants
    private const float HeatGrowingSpeed = 0.05f;
    public const float HeatBurningLevel = 0.6f;
    private const float fireExpansionTime = 2.0f;
    private const float OscillationSpeed = 0.33f;
    private const float FireHeightScale = 2.0f;
    #endregion Constants

    private float fireExpansionTimer = 0;

    protected override void OnCreate()
    {
        // Wait for the specified instantiations
        RequireSingletonForUpdate<Spawner>();
        RequireSingletonForUpdate<Heat>();
    }

    // Update is called once per frame
    protected override void OnUpdate()
    {
        var spawner = GetSingleton<Spawner>();
        var gridSize = spawner.GridSize;
        var heatSingleton = EntityManager.GetBuffer<Heat>(GetSingletonEntity<Heat>());
        var random = new Unity.Mathematics.Random(1234);

        float deltaTime = (float)Time.DeltaTime;
        float elapsedTime = (float)Time.ElapsedTime;

        fireExpansionTimer -= deltaTime;
        bool expandFire = fireExpansionTimer < 0;
        if (expandFire)
        {
            fireExpansionTimer = fireExpansionTime;
        }

        Entities
            .WithAll<Firelocation>()
            .ForEach((ref NonUniformScale s, ref Translation t, ref URPMaterialPropertyBaseColor c) =>
            {
                // Access Heat value
                var x = (int)t.Value.x;
                var y = (int)t.Value.z;
                var index = x + y * gridSize;
                var heat = heatSingleton[index].Value;


                // Set Color from orange to red (Burning) or green
                if (heat == 0)
                {
                    c.Value.x = 0;
                    c.Value.y = 1.0f;
                    s.Value.y = 0.01f;
                    t.Value.y = 0;

                    if (expandFire)
                    {
                        float transmittedHeat = 0;

                        for (int i = -1; i <= 1; i++)
                        {
                            var nx = x + i;

                            if (nx >= 0 && nx < gridSize - 1)
                            {
                                for (int j = -1; j <= 1; j++)
                                {
                                    var ny = y + j;

                                    if (ny >= 0 && ny < gridSize - 1)
                                    {
                                        if (i != 0 || j != 0)
                                        {
                                            var neighborIndex = nx + ny * gridSize;
                                            if (heatSingleton[neighborIndex].Value > HeatBurningLevel)
                                            {
                                                transmittedHeat += heatSingleton[neighborIndex].Value * (float)Math.Max(0, random.NextFloat(-0.5f, 0.5f));
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        if (transmittedHeat > 0)
                        {
                            heatSingleton[index] = new Heat
                            {
                                Value = transmittedHeat * 0.000125f
                            };
                        }
                    }
                }
                else
                {
                    // Set scale according to heat
                    var displayedHeat = heat + 0.5f * Mathf.PerlinNoise(elapsedTime * OscillationSpeed, index) * heat;
                    s.Value.y = displayedHeat * FireHeightScale;

                    // Adjust position
                    t.Value.y = s.Value.y * 0.5f;

                    // Set Color
                    c.Value.x = 1.0f;
                    c.Value.y = (1.0f - displayedHeat) * 0.5f;

                    // Update Heat
                    var updatedHeat = (float)Math.Min(1.0f, heatSingleton[index].Value + deltaTime * HeatGrowingSpeed);
                    heatSingleton[index] = new Heat
                    {
                        Value = updatedHeat
                    };
                }

            }).Schedule();
    }
}
