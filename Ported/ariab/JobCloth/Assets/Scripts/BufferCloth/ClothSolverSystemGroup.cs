using Unity.Entities;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(ClothPredictiveContactGenerationSystem))]
public class ClothSolverSystemGroup : ComponentSystemGroup
{
    private ClothConstraintSolverSystem m_ConstraintSolverSystem;
    private ClothPredictiveContactSolverSystem m_ContactSolverSystem;
    private ClothHierarchicalSolverSystem m_HierarchicalSolverSystem;
    
    protected override void OnCreate()
    {
        m_ConstraintSolverSystem = World.GetOrCreateSystem<ClothConstraintSolverSystem>();
        m_ContactSolverSystem = World.GetOrCreateSystem<ClothPredictiveContactSolverSystem>();
        m_HierarchicalSolverSystem = World.GetOrCreateSystem<ClothHierarchicalSolverSystem>();
        
        //m_systemsToUpdate.Add(m_ConstraintSolverSystem);
        //m_systemsToUpdate.Add(m_ContactSolverSystem);
    }

    protected override void OnUpdate()
    {
        for (int i = 0; i < 8; ++i)
        {
            m_ConstraintSolverSystem.Update();
            m_ContactSolverSystem.Update();
            m_HierarchicalSolverSystem.Update();
        }
    }
}