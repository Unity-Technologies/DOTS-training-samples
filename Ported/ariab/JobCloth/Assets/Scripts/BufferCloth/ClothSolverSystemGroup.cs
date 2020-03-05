using Unity.Entities;
using UnityEngine;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(ClothPredictiveContactGenerationSystem))]
public class ClothSolverSystemGroup : ComponentSystemGroup
{
    private ClothTimestepSystem m_ClothTimestepSystem;
    
    private ClothProjectSystem m_ClothProjectSystem;
    private ClothPredictiveContactGenerationSystem m_ClothPredictContacts;
    
    private ClothConstraintSolverSystem m_ConstraintSolverSystem;
    private ClothPredictiveContactSolverSystem m_ContactSolverSystem;
    private ClothHierarchicalSolverSystem m_HierarchicalSolverSystem;
    
    private ClothCopyProjectedToPreviousSystem m_ClothAdvanceSystem;
    private CopyVerticesToMeshSystem m_ClothCopyToMeshSystem;
    
    protected override void OnCreate()
    {
        m_ClothTimestepSystem = World.GetOrCreateSystem<ClothTimestepSystem>();
        
        m_ClothProjectSystem = World.GetOrCreateSystem<ClothProjectSystem>();
        m_ClothPredictContacts = World.GetOrCreateSystem<ClothPredictiveContactGenerationSystem>();

        m_ConstraintSolverSystem = World.GetOrCreateSystem<ClothConstraintSolverSystem>();
        m_ContactSolverSystem = World.GetOrCreateSystem<ClothPredictiveContactSolverSystem>();
        m_HierarchicalSolverSystem = World.GetOrCreateSystem<ClothHierarchicalSolverSystem>();
        
        m_ClothAdvanceSystem = World.GetOrCreateSystem<ClothCopyProjectedToPreviousSystem>();
        m_ClothCopyToMeshSystem = World.GetOrCreateSystem<CopyVerticesToMeshSystem>();
    }

    protected override void OnUpdate()
    {
        m_ClothTimestepSystem.Update();
        
        var integrationData = GetSingleton<ClothTimestepSingleton>();
        var iterationCount = GetSingleton<GlobalIterationCount>();
        for (int integrationCount = 0; integrationCount < integrationData.IntegrationCount; ++integrationCount)
        {
            m_ClothProjectSystem.Update();
            m_ClothPredictContacts.Update();
            
            m_HierarchicalSolverSystem.Update();
            
            for (int i = 0; i < iterationCount.Value; ++i)
            {
                m_ConstraintSolverSystem.Update();
                m_ContactSolverSystem.Update();
            }
            
            m_ClothAdvanceSystem.Update();
        }
       
        m_ClothCopyToMeshSystem.Update();
    }
}