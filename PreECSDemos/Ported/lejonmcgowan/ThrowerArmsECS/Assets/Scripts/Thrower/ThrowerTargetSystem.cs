using System.Diagnostics;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class ThrowerTargetSystem: JobComponentSystem
{
    private BeginSimulationEntityCommandBufferSystem m_targetECB;

    protected override void OnCreate()
    {
        m_targetECB = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
    }
    
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        float t = (float)Time.ElapsedTime;
        Entities.WithStructuralChanges().ForEach((Entity entity, ref ArmBaseComponentData baseComponentData) =>
        {
            
            Translation idlePosition = new Translation
            {
                Value = new float3(baseComponentData.anchorPosition.Value + new float3(math.sin(t) * 0.35f,
                    1f + math.cos(t * 1.618f) * 0.5f,
                    1.5f))
            };
            
            baseComponentData.target = idlePosition;
            NativeArray<float3> armChain = new NativeArray<float3>(3,Allocator.Temp);
            FABIK.Solve(armChain, 1f, baseComponentData.anchorPosition.Value, idlePosition.Value, 0.1f * baseComponentData.LastHandUp);
            
            float3 handForward = math.normalize(armChain[armChain.Length - 1] - armChain[armChain.Length - 2]);
            float3 handUp = math.normalize(math.cross(handForward, baseComponentData.anchorRight));
            float3 handRight = math.cross(handUp, handForward);

            baseComponentData.LastHandUp = handUp;

            Entity? lastRoot = null;
            Entity root = entity;
            
            for (int armNode = 0; armNode < armChain.Length - 1; armNode++)
            {
                ArmFingerComponentData c = EntityManager.GetComponentData<ArmFingerComponentData>(root);
                
                c.position = armChain[armNode];
                c.forward = math.normalize(armChain[armNode + 1] - armChain[armNode]);
                c.anchorUp = handUp;
                
                EntityManager.SetComponentData(root,c);

                lastRoot = root;
                root = EntityManager.GetBuffer<ChildComponentData>(root)[0].child;
            }
            
            //last arm should have fingers as targets. update parameters for fingers
            Debug.Assert(lastRoot != null, nameof(lastRoot) + " != null");
            
            var children = EntityManager.GetBuffer<ChildComponentData>(lastRoot.Value);
            for (int i = 0; i < children.Length; i++)
            {
                Entity child = children[i].child;
                NativeArray<float3> fingerChain = new NativeArray<float3>(4,Allocator.Temp);
                float3 fingerPos = armChain[armChain.Length - 1] + handRight * (-0.12f + i * 0.08f);
                float3 fingerTarget = fingerPos + handForward * 0.5f;
                    
                FABIK.Solve(fingerChain,0.2f,fingerPos,fingerTarget,0.2f * handUp);
                
                Entity fingerRoot = child;
                for (int fingerNode = 0; fingerNode < fingerChain.Length - 1; fingerNode++)
                {
                    ArmFingerComponentData c = EntityManager.GetComponentData<ArmFingerComponentData>(fingerRoot);
                
                    c.position = fingerChain[fingerNode];
                    c.forward = math.normalize(fingerChain[fingerNode + 1] - fingerChain[fingerNode]);
                    c.anchorUp = handUp;
                
                    EntityManager.SetComponentData(fingerRoot,c);
                    
                    if(EntityManager.HasComponent<ChildComponentData>(fingerRoot))
                        fingerRoot = EntityManager.GetBuffer<ChildComponentData>(fingerRoot)[0].child;
                }
                
            }
            
        }).Run();
        
        return inputDeps;
    }
}
