using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using LogDebug = System.Diagnostics.Debug;
using Debug = UnityEngine.Debug;

[UpdateAfter(typeof(ThrowerSeekSystem))]
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
        float dt = Time.DeltaTime;
        
        Entities.WithStructuralChanges().ForEach((Entity entity, ref ArmBaseComponentData armBaseData) =>
        {

            Translation testPosition = new Translation
            {
                Value = armBaseData.anchorPosition.Value + new float3(0, 0, 4)
            };
            
            Translation idlePosition = new Translation
            {
                Value = new float3(armBaseData.anchorPosition.Value + new float3(math.sin(t) * 0.35f,
                    1f + math.cos(t * 1.618f) * 0.5f,
                    1.5f))
            };
            
            float grabLerpT = math.clamp(armBaseData.grabT / 3.0f,0,1);

            var endPos = float3.zero;

            if (EntityManager.HasComponent<ReserveComponentData>(entity))
            {
                Entity targetProj = EntityManager.GetComponentData<ReserveComponentData>(entity);
                float3 pos = EntityManager.GetComponentData<Translation>(targetProj).Value;
                endPos = pos;
            }
            else
            {
                endPos = testPosition.Value;
            }
                  
            
            armBaseData.target.Value  = math.lerp(idlePosition.Value,endPos,grabLerpT);

            Debug.DrawLine(armBaseData.anchorPosition.Value,endPos,Color.red);
            
            NativeArray<float3> armChain = new NativeArray<float3>(3,Allocator.Temp);
            FABIK.Solve(armChain, 1f, armBaseData.anchorPosition.Value, armBaseData.target.Value, 
                0.1f * armBaseData.LastHandUp);

            armBaseData.grabT += dt;
            
            float3 handForward = math.normalize(armChain[armChain.Length - 1] - armChain[armChain.Length - 2]);
            float3 handUp = math.normalize(math.cross(handForward, armBaseData.anchorRight));
            float3 handRight = math.cross(handUp, handForward);

            armBaseData.LastHandUp = handUp;

            Entity? lastRoot = null;
            Entity root = entity;
            
            for (int armNode = 0; armNode < armChain.Length - 1; armNode++)
            {
                ArmFingerComponentData c = EntityManager.GetComponentData<ArmFingerComponentData>(root);
                
                c.position = armChain[armNode];
                c.delta = armChain[armNode + 1] - armChain[armNode];
                c.anchorUp = handUp;
                
                EntityManager.SetComponentData(root,c);

                lastRoot = root;
                root = EntityManager.GetBuffer<ChildComponentData>(root)[0].child;
            }
            
            //last arm link should have fingers as targets. update parameters for fingers
            LogDebug.Assert(lastRoot != null, nameof(lastRoot) + " != null");
            
            //iterate through finger children of last Arm link
            var children = EntityManager.GetBuffer<ChildComponentData>(lastRoot.Value);
            for (int i = 0; i < children.Length; i++)
            {
                Entity child = children[i].child;
                NativeArray<float3> fingerChain = new NativeArray<float3>(4,Allocator.Temp);
                float3 fingerPos = armChain[armChain.Length - 1] + handRight * (-0.12f + i * 0.08f);
                float3 fingerTarget = fingerPos + handForward * 0.5f;

                //finger wiggle
                fingerTarget += handUp * math.sin((t + i * .2f) * 3f) * .2f;
                
                FABIK.Solve(fingerChain,0.2f,fingerPos,fingerTarget,0.2f * handUp);
                
                //hierarchically update the chain for each child finger  
                Entity fingerRoot = child;
                for (int fingerNode = 0; fingerNode < fingerChain.Length - 1; fingerNode++)
                {
                    ArmFingerComponentData c = EntityManager.GetComponentData<ArmFingerComponentData>(fingerRoot);
                
                    c.position = fingerChain[fingerNode];
                    c.delta = fingerChain[fingerNode + 1] - fingerChain[fingerNode];
                    c.anchorUp = handUp;
                
                    EntityManager.SetComponentData(fingerRoot,c);
                    
                    if(EntityManager.HasComponent<ChildComponentData>(fingerRoot))
                        fingerRoot = EntityManager.GetBuffer<ChildComponentData>(fingerRoot)[0].child;
                    
                    //UnityEngine.Debug.DrawLine(fingerChain[fingerNode],fingerChain[fingerNode + 1]);
                }
                
                //update finger base for later targeting
                LogDebug.Assert(EntityManager.HasComponent<FingerBaseComponentData>(child), 
                    nameof(child) + " doesn't have a FingerBaseComponentData attached to it");
                
                if (EntityManager.HasComponent<ReserveComponentData>(entity))
                {
                    var projEntity = EntityManager.GetComponentData<ReserveComponentData>(entity).reserver;
                    float3 lastFingerPos = EntityManager.GetComponentData<Translation>(fingerRoot).Value;
                
                    Debug.DrawLine(lastFingerPos,endPos,Color.blue);

                    float d = math.distance(lastFingerPos,endPos);
                
                    if (d > 2.0f)
                    {
                        EntityManager.RemoveComponent<ReserveComponentData>(entity);
                        EntityManager.RemoveComponent<ReserveComponentData>(projEntity);
                    }
                
                    if (d < 0.2f)
                    {
                        EntityManager.AddComponent<GrabbedTag>(projEntity);
                    }
                }
            }
            

        }).Run();
        
        return inputDeps;
    }
}
