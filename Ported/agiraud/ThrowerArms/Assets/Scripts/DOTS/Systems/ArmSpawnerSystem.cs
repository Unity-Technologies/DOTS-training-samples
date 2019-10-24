using System;
using Unity.Entities;
using Unity.Mathematics;
using Random = Unity.Mathematics.Random;

[UpdateInGroup(typeof(ThrowerArmsGroupSystem))]
public class ArmSpawnerSystem : ComponentSystem
{
    private int m_BoneCount = 17;
    private int m_JointCount = 23;
    public float m_ArmBoneThickness = 0.15f;
    public float m_FingerThickness = 0.06f;
    public float m_ThumbThickness = 0.06f;

    private Random m_RandomGenerator;

    private EntityArchetype m_ArmArchetype;
    protected override void OnCreate()
    {
        m_ArmArchetype = EntityManager.CreateArchetype(
            ComponentType.ReadWrite<BoneJoint>(),
            ComponentType.ReadWrite<HandAxis>(), 
            ComponentType.ReadWrite<ArmTarget>(),
            ComponentType.ReadWrite<Timers>()
            );

        m_RandomGenerator = new Random((uint)Environment.TickCount);
    }

    private void CreateArm(Entity bonePrefab, Entity parent)
    {
        int b = 0;
        // Arm
        for(int i = 0; i < 2; i++)
        {
            SpawnBone(b, bonePrefab, parent, m_ArmBoneThickness);
            b++;
        }

        // Fingers
        for (var finger = 0; finger < 4; finger++)
        {
            for (var i = 0; i < 3; i++)
            {
                SpawnBone(b, bonePrefab, parent, m_FingerThickness);
                b++;
            }
        }
        
        // Thumb
        for(int i = 0; i < 3; i++)
        {
            SpawnBone(b, bonePrefab, parent, m_ThumbThickness);
            b++;
        }
    }

    private void SpawnBone(int index, Entity bonePrefab, Entity parent, float thickness)
    {
        var boneInstance = EntityManager.Instantiate(bonePrefab);
        var data = new BoneData
        {
            Parent = parent,
            ChainIndex = index,
            Thickness = thickness
        };
        EntityManager.AddComponentData(boneInstance, data);
    }
    
    protected override void OnUpdate()
    {
        Entities.ForEach((Entity e, ref ArmSpawnerData spawnerData) =>
        {
            //float armSpacing = math.abs(SceneParameters.Instance.RockSpawnBoxMin.x - SceneParameters.Instance.RockSpawnBoxMax.x) / spawnerData.Count;
            float armSpacing = 0.2f;
            
            for (var i = 0; i < spawnerData.Count; i++)
            {
                var armEntity = EntityManager.CreateEntity(m_ArmArchetype);
                var jointBuf = EntityManager.GetBuffer<BoneJoint>(armEntity);
                jointBuf.ResizeUninitialized(m_JointCount);
                for (var j = 0; j < m_JointCount; j++)
                {
                    jointBuf[j] = new BoneJoint
                    {
                        JointPos = float3.zero
                    };
                }
                // Set anchor value
                jointBuf[0] = new BoneJoint
                {
                    JointPos = new float3(SceneParameters.Instance.RockSpawnBoxMin.x * 0.5f + i * armSpacing, 0, -0.8f)
                };

                EntityManager.SetComponentData(armEntity, new HandAxis{ Up = math.up() });

                var hardcodedSeed = 10;
                float3 idleHandTarget = jointBuf[0].JointPos + new float3(math.sin(hardcodedSeed)*.35f,
                                            1f+math.cos(hardcodedSeed*1.618f)*.5f,1.5f);

                EntityManager.SetComponentData(armEntity, new ArmTarget{ Value = idleHandTarget });

                var timers = new Timers
                {
                    TimeOffset = m_RandomGenerator.NextFloat(0.0f, 100.0f),
                    Reach = 0.0f,
                    Throw = 0.0f,
                    Windup = 0.0f
                };
                EntityManager.SetComponentData(armEntity, timers);
                
                CreateArm(spawnerData.BoneEntityPrefab, armEntity);
            }
            EntityManager.DestroyEntity(e);
        });
    }
}
