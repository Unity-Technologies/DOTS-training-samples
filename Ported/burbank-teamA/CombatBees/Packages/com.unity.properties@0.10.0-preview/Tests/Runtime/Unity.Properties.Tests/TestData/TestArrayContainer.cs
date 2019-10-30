namespace Unity.Properties.Tests
{
    public struct TestArrayContainer
    {
        public int[] Int32Array;
        public TestPrimitiveContainer[] TestContainerArray;
    }

    public class TestArrayContainerPropertyBag : PropertyBag<TestArrayContainer>
    {
        readonly ArrayProperty<TestArrayContainer, int> m_Int32Array = new ArrayProperty<TestArrayContainer, int>(
            nameof(TestArrayContainer.Int32Array),
            (ref TestArrayContainer container) => container.Int32Array,
            (ref TestArrayContainer container, int[] value) => container.Int32Array = value
        );

        readonly ArrayProperty<TestArrayContainer, TestPrimitiveContainer> m_TestStructArray = new ArrayProperty<TestArrayContainer, TestPrimitiveContainer>(
            nameof(TestArrayContainer.TestContainerArray),
            (ref TestArrayContainer container) => container.TestContainerArray,
            (ref TestArrayContainer container, TestPrimitiveContainer[] value) => container.TestContainerArray = value
        );

        public override void Accept<TVisitor>(ref TestArrayContainer container, ref TVisitor visitor, ref ChangeTracker changeTracker)
        {
            visitor.VisitCollectionProperty<ArrayProperty<TestArrayContainer, int>, TestArrayContainer, int[]>(m_Int32Array, ref container, ref changeTracker);
            visitor.VisitCollectionProperty<ArrayProperty<TestArrayContainer, TestPrimitiveContainer>, TestArrayContainer, TestPrimitiveContainer[]>(m_TestStructArray, ref container, ref changeTracker);
        }

        public override bool FindProperty<TAction>(string name, ref TestArrayContainer container, ref ChangeTracker changeTracker, ref TAction action)
        {
            if (string.Equals(name, m_Int32Array.GetName()))
            {
                action.VisitCollectionProperty<ArrayProperty<TestArrayContainer, int>, int[]>(m_Int32Array, ref container, ref changeTracker);
                return true;
            }

            if (string.Equals(name, m_TestStructArray.GetName()))
            {
                action.VisitCollectionProperty<ArrayProperty<TestArrayContainer, TestPrimitiveContainer>, TestPrimitiveContainer[]>(m_TestStructArray, ref container, ref changeTracker);
                return true;
            }

            return false;
        }
    }
}
