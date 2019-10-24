using Unity.Entities;
using Unity.Mathematics;

[UpdateInGroup(typeof(ThrowerArmsGroupSystem))]
public class ArmSpawnerSystem : ComponentSystem
{
    private int m_BoneCount = 17;
    private int m_JointCount = 23;

    public float m_ArmBoneLength = 1.0f;
    public float m_ArmBoneThickness = 0.15f;
    public float m_ArmBendStrength;
    public float m_MaxReachLength;
    public float m_ReachDuration;
    public float m_MaxHandSpeed;
    
    //[Range(0f,1f)]
    //public float grabTimerSmooth;

//    public float[] fingerBoneLengths;
//    public float[] fingerThicknesses;
    public float m_FingerBoneLength;
    public float m_FingerThickness = 0.06f;
    public float m_FingerXOffset;
    public float m_FingerSpacing;
    public float m_FingerBendStrength;

    public float m_ThumbBoneLength;
    public float m_ThumbThickness = 0.06f;
    public float m_ThumbBendStrength;
    public float m_ThumbXOffset;

//    public float windupDuration;
//    public float throwDuration;
//    public AnimationCurve throwCurve;
//    public float baseThrowSpeed;
//    public float targetXRange;

    private EntityArchetype m_ArmArchetype;
    protected override void OnCreate()
    {
        m_ArmArchetype = EntityManager.CreateArchetype(ComponentType.ReadWrite<BoneJoint>(),
            ComponentType.ReadWrite<HandAxis>(), ComponentType.ReadWrite<ArmTarget>());
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
                    JointPos = new float3(i * 2.0f, 0, 0)
                };

                EntityManager.SetComponentData(armEntity, new HandAxis{ Up = math.up() });

                var hardcodedSeed = 10;
                float3 idleHandTarget = jointBuf[0].JointPos + new float3(math.sin(hardcodedSeed)*.35f,
                                            1f+math.cos(hardcodedSeed*1.618f)*.5f,1.5f);

                EntityManager.SetComponentData(armEntity, new ArmTarget{ Value = idleHandTarget });

                CreateArm(spawnerData.BoneEntityPrefab, armEntity);
            }
            EntityManager.DestroyEntity(e);
        });
    }
}
