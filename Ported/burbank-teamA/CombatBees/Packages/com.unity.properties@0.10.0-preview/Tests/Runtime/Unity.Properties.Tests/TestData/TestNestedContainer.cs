namespace Unity.Properties.Tests
{
    public struct TestNestedContainer
    {
        public TestPrimitiveContainer TestPrimitiveContainer;
    }

    public class TestNestedContainerPropertyBag : PropertyBag<TestNestedContainer>
    {
        readonly UnmanagedProperty<TestNestedContainer, TestPrimitiveContainer> m_TestContainer = new UnmanagedProperty<TestNestedContainer, TestPrimitiveContainer>(
            nameof(TestNestedContainer.TestPrimitiveContainer),
            0);

        public override void Accept<TVisitor>(ref TestNestedContainer container, ref TVisitor visitor, ref ChangeTracker changeTracker)
        {
            visitor.VisitProperty<UnmanagedProperty<TestNestedContainer, TestPrimitiveContainer>, TestNestedContainer, TestPrimitiveContainer>(m_TestContainer, ref container, ref changeTracker);
        }

        public override bool FindProperty<TCallback>(string name, ref TestNestedContainer container, ref ChangeTracker changeTracker, ref TCallback action)
        {
            if (string.Equals(name, m_TestContainer.GetName()))
            {
                action.VisitProperty<UnmanagedProperty<TestNestedContainer, TestPrimitiveContainer>, TestPrimitiveContainer>(m_TestContainer, ref container, ref changeTracker);
                return true;
            }

            return false;
        }
    }
}