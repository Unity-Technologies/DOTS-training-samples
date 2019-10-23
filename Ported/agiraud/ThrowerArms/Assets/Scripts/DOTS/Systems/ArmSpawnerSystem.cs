using Unity.Entities;
using Unity.Mathematics;

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
                var armEntity = EntityManager.CreateEntity();
                var jointBuf = EntityManager.AddBuffer<BoneJoint>(armEntity);
                jointBuf.ResizeUninitialized(m_JointCount);
                for (var j = 0; j < m_JointCount; j++)
                {
                    jointBuf[j] = new BoneJoint
                    {
                        JointPos = float3.zero
                    };
                }

                CreateArm(spawnerData.BoneEntityPrefab, armEntity);
            }
            EntityManager.DestroyEntity(e);
        });
    }
}
