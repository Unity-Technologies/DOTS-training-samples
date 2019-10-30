using NUnit.Framework;

namespace Unity.Entities.PerformanceTests
{
    public class EntityPerformanceTestFixture
    {
        protected World m_PreviousWorld;
        protected World m_World;
        protected EntityManager m_Manager;
        
        
        [SetUp]
        virtual public void Setup()
        {
            m_PreviousWorld = World.DefaultGameObjectInjectionWorld;
            m_World = World.DefaultGameObjectInjectionWorld = new World("Test World");
            m_Manager = m_World.EntityManager;
        }

        [TearDown]
        virtual public void TearDown()
        {
            if (m_Manager != null)
            {
                m_World.Dispose();
                m_World = null;

                World.DefaultGameObjectInjectionWorld = m_PreviousWorld;
                m_PreviousWorld = null;
                m_Manager = null;
            }
        }
    }
}