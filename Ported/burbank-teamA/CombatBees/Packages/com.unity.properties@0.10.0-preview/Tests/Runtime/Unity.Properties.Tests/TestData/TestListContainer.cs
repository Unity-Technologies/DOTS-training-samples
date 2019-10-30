using System.Collections.Generic;

namespace Unity.Properties.Tests
{
    public struct TestListContainer
    {
        public IList<int> Int32List;
        public List<TestPrimitiveContainer> TestContainerList;
    }

    public class TestListContainerPropertyBag : PropertyBag<TestListContainer>
    {
        readonly ListProperty<TestListContainer, int> m_Int32List = new ListProperty<TestListContainer, int>(
            nameof(TestListContainer.Int32List),
            (ref TestListContainer container) => container.Int32List,
            (ref TestListContainer container, IList<int> value) => container.Int32List = (IList<int>) value
        );

        readonly ListProperty<TestListContainer, TestPrimitiveContainer> m_TestStructList = new ListProperty<TestListContainer, TestPrimitiveContainer>(
            nameof(TestListContainer.TestContainerList),
            (ref TestListContainer container) => container.TestContainerList,
            (ref TestListContainer container, IList<TestPrimitiveContainer> value) => container.TestContainerList = (List<TestPrimitiveContainer>) value
        );

        public override void Accept<TVisitor>(ref TestListContainer container, ref TVisitor visitor, ref ChangeTracker changeTracker)
        {
            visitor.VisitCollectionProperty<ListProperty<TestListContainer, int>, TestListContainer, IList<int>>(m_Int32List, ref container, ref changeTracker);
            visitor.VisitCollectionProperty<ListProperty<TestListContainer, TestPrimitiveContainer>, TestListContainer, IList<TestPrimitiveContainer>>(m_TestStructList, ref container, ref changeTracker);
        }

        public override bool FindProperty<TAction>(string name, ref TestListContainer container, ref ChangeTracker changeTracker, ref TAction action)
        {
            if (string.Equals(name, m_Int32List.GetName()))
            {
                action.VisitCollectionProperty<ListProperty<TestListContainer, int>, IList<int>>(m_Int32List, ref container, ref changeTracker);
                return true;
            }

            if (string.Equals(name, m_TestStructList.GetName()))
            {
                action.VisitCollectionProperty<ListProperty<TestListContainer, TestPrimitiveContainer>, IList<TestPrimitiveContainer>>(m_TestStructList, ref container, ref changeTracker);
                return true;
            }

            return false;
        }
    }
}