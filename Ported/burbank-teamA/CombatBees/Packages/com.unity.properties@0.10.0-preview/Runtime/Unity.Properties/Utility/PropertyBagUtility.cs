namespace Unity.Properties
{
    /// <summary>
    /// Helper methods to query an <see cref="IPropertyBag{TContainer}"/>.
    /// </summary>
    public static class PropertyBagUtility
    {
        struct FoundAction<TContainer> : IPropertyGetter<TContainer>
        {
            public bool Found;
            
            public void VisitProperty<TProperty, TValue>(TProperty property, ref TContainer container, ref ChangeTracker changeTracker) 
                where TProperty : IProperty<TContainer, TValue>
                => Found = true;

            public void VisitCollectionProperty<TProperty, TValue>(TProperty property, ref TContainer container, ref ChangeTracker changeTracker) 
                where TProperty : ICollectionProperty<TContainer, TValue>
                => Found = true;
        }

        /// <summary>
        /// Checks if the given <see cref="IPropertyBag{TContainer}"/> has any property with the given name.
        /// </summary>
        public static bool HasProperty<TContainer>(this IPropertyBag<TContainer> propertyBag, string name)
        {
            var instance = default(TContainer);
            var changeTracker = default(ChangeTracker);
            var action = default(FoundAction<TContainer>);
            propertyBag.FindProperty(name, ref instance, ref changeTracker, ref action);
            return action.Found;
        }
    }
}