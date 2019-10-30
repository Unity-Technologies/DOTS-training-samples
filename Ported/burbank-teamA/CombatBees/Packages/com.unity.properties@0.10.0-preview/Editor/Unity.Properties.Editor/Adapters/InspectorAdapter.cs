namespace Unity.Properties.Editor
{
    abstract class InspectorAdapter<T> : IPropertyVisitorAdapter
    {
        protected readonly InspectorVisitor<T> Visitor;
        protected readonly InspectorVisitorContext VisitorContext;
        // TODO: Support multiple targets when UIElements will have built-in mixed values support.
        protected readonly T Target;

        protected InspectorAdapter(InspectorVisitor<T> visitor)
        {
            Visitor = visitor;
            VisitorContext = visitor.VisitorContext;
            Target = visitor.Target;
        }
    }
}
