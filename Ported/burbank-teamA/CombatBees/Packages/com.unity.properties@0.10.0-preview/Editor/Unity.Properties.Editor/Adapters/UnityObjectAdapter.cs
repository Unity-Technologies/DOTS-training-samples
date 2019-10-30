namespace Unity.Properties.Editor
{
    sealed class UnityObjectAdapter<T> : InspectorAdapter<T>
        , IVisitContainerAdapter
    {
        public UnityObjectAdapter(InspectorVisitor<T> visitor) : base(visitor)
        {
        }

        VisitStatus IVisitContainerAdapter.BeginContainer<TProperty, TValue, TContainer>(
            IPropertyVisitor visitor,
            TProperty property,
            ref TContainer container,
            ref TValue value,
            ref ChangeTracker changeTracker)
        {
            if (!typeof(UnityEngine.Object).IsAssignableFrom(typeof(TValue)))
            {
                return VisitStatus.Unhandled;
            }

            var obj = value as UnityEngine.Object;
            var field = GuiFactory.Construct<TProperty, TContainer, TValue>(property, ref container,
                obj, VisitorContext);
            field.objectType = typeof(TValue);

            return VisitStatus.Override;
        }

        void IVisitContainerAdapter.EndContainer<TProperty, TValue, TContainer>(
            IPropertyVisitor visitor,
            TProperty property,
            ref TContainer container,
            ref TValue value,
            ref ChangeTracker changeTracker)
        {
        }
    }
}
