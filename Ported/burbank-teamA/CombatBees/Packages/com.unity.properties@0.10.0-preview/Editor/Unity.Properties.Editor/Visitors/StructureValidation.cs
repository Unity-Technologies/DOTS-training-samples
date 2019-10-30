using System.Text;

namespace Unity.Properties.Editor
{
    static class StructureValidation
    {
        static readonly StructureVisitor s_StructureVisitor = new StructureVisitor();
        
        public static bool SameStructure<T>(ref T lhs, ref T rhs)
        {
            if (null == lhs)
            {
                return null == rhs;
            }

            if (null == rhs)
            {
                return false;
            }
            
            s_StructureVisitor.Reset();
            PropertyContainer.Visit(ref lhs, s_StructureVisitor);
            var l = s_StructureVisitor.Get;

            s_StructureVisitor.Reset();
            PropertyContainer.Visit(ref rhs, s_StructureVisitor);
            var r = s_StructureVisitor.Get;

            // Both values have the same type and structure, so it's fine to simply update them.
            return (l == r);
        }

        class StructureVisitor : PropertyVisitor
        {
            readonly StringBuilder m_Builder = new StringBuilder(1024);

            public string Get => m_Builder.ToString();
            public void Reset() => m_Builder.Clear();

            protected override VisitStatus Visit<TProperty, TContainer, TValue>(TProperty property,
                ref TContainer container, ref TValue value,
                ref ChangeTracker changeTracker)
            {
                if (null == value)
                {
                    m_Builder.Append('N');
                    return VisitStatus.Override;
                }

                m_Builder.Append('V');
                return base.Visit(property, ref container, ref value, ref changeTracker);
            }

            protected override VisitStatus BeginContainer<TProperty, TContainer, TValue>(TProperty property,
                ref TContainer container,
                ref TValue value, ref ChangeTracker changeTracker)
            {
                if (null == value)
                {
                    m_Builder.Append('N');
                    return VisitStatus.Override;
                }

                m_Builder.Append('C');
                return base.BeginContainer(property, ref container, ref value, ref changeTracker);
            }

            protected override VisitStatus BeginCollection<TProperty, TContainer, TValue>(TProperty property,
                ref TContainer container,
                ref TValue value, ref ChangeTracker changeTracker)
            {
                if (null == value)
                {
                    m_Builder.Append('N');
                    return VisitStatus.Override;
                }

                m_Builder.Append('I');
                return base.BeginCollection(property, ref container, ref value, ref changeTracker);
            }
        }
    }
}
