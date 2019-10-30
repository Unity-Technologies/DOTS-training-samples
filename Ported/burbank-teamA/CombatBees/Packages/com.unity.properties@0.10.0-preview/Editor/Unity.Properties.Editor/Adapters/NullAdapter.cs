namespace Unity.Properties.Editor
{
    sealed class NullAdapter<T> : InspectorAdapter<T>
        , IVisitAdapter
        , IVisitContainerAdapter
        , IVisitCollectionAdapter
    {
        public NullAdapter(InspectorVisitor<T> visitor) : base(visitor)
        {
        }

        VisitStatus IVisitAdapter.Visit<TProperty, TContainer, TValue>(
            IPropertyVisitor visitor,
            TProperty property,
            ref TContainer container,
            ref TValue value,
            ref ChangeTracker changeTracker)
        {
            if (null != value)
            {
                return VisitStatus.Unhandled;
            }

            var nullElement = new NullElement<TValue>(property.GetName());
            VisitorContext.Parent.contentContainer.Add(nullElement);
            return VisitStatus.Override;
        }

        public VisitStatus BeginContainer<TProperty, TValue, TContainer>(
            IPropertyVisitor visitor,
            TProperty property,
            ref TContainer container,
            ref TValue value,
            ref ChangeTracker changeTracker) 
            where TProperty : IProperty<TContainer, TValue>
        {
            if (null != value)
            {
                return VisitStatus.Unhandled;
            }

            var nullElement = new NullElement<TValue>(property.GetName());
            VisitorContext.Parent.contentContainer.Add(nullElement);
            return VisitStatus.Override;
        }

        public void EndContainer<TProperty, TValue, TContainer>(
            IPropertyVisitor visitor,
            TProperty property,
            ref TContainer container,
            ref TValue value,
            ref ChangeTracker changeTracker)
            where TProperty : IProperty<TContainer, TValue>
        {
        }

        public VisitStatus BeginCollection<TProperty, TContainer, TValue>(
            IPropertyVisitor visitor,
            TProperty property,
            ref TContainer container,
            ref TValue value,
            ref ChangeTracker changeTracker)
            where TProperty : ICollectionProperty<TContainer, TValue>
        {
            if (null != value)
            {
                return VisitStatus.Unhandled;
            }

            var nullElement = new NullElement<TValue>(property.GetName());
            VisitorContext.Parent.contentContainer.Add(nullElement);
            return VisitStatus.Override;
        }

        public void EndCollection<TProperty, TContainer, TValue>(
            IPropertyVisitor visitor,
            TProperty property,
            ref TContainer container,
            ref TValue value,
            ref ChangeTracker changeTracker) 
            where TProperty : ICollectionProperty<TContainer, TValue>
        {
        }
    }
}
