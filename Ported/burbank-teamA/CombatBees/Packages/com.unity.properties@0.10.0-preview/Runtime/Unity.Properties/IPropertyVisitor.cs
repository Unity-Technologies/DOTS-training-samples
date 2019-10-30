namespace Unity.Properties
{
    /// <summary>
    /// Interface to visit a property bag.
    /// </summary>
    public interface IPropertyVisitor
    {
        VisitStatus VisitProperty<TProperty, TContainer, TValue>(TProperty property, ref TContainer container, ref ChangeTracker changeTracker)
            where TProperty : IProperty<TContainer, TValue>;

        VisitStatus VisitCollectionProperty<TProperty, TContainer, TValue>(TProperty property, ref TContainer container, ref ChangeTracker changeTracker)
            where TProperty : ICollectionProperty<TContainer, TValue>;
    }

    /// <summary>
    /// Interface to get a typed property from a <see cref="IPropertyBag"/>.
    /// </summary>
    public interface IPropertyGetter<TContainer>
    {
        void VisitProperty<TProperty, TValue>(TProperty property, ref TContainer container, ref ChangeTracker changeTracker)
            where TProperty : IProperty<TContainer, TValue>;

        void VisitCollectionProperty<TProperty, TValue>(TProperty property, ref TContainer container, ref ChangeTracker changeTracker)
            where TProperty : ICollectionProperty<TContainer, TValue>;
    }
    
    /// <summary>
    /// Interface to get a typed property from a <see cref="ICollectionProperty{TContainer,TValue}"/>
    /// </summary>
    public interface ICollectionElementPropertyGetter<TContainer>
    {
        void VisitProperty<TElementProperty, TElement>(TElementProperty property, ref TContainer container, ref ChangeTracker changeTracker)
            where TElementProperty : ICollectionElementProperty<TContainer, TElement>;

        void VisitCollectionProperty<TElementProperty, TElement>(TElementProperty property, ref TContainer container, ref ChangeTracker changeTracker)
            where TElementProperty : ICollectionProperty<TContainer, TElement>, ICollectionElementProperty<TContainer, TElement>;
    }
}
