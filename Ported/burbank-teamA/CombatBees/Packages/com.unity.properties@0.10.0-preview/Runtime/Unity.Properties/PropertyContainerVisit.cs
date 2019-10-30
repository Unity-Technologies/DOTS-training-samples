namespace Unity.Properties
{
    public static partial class PropertyContainer
    {
        public static void Visit<TContainer, TVisitor>(
            TContainer container, 
            TVisitor visitor, 
            IVersionStorage versionStorage = null)
            where TVisitor : IPropertyVisitor
        {
            var changeTracker = new ChangeTracker(versionStorage);
            Visit(ref container, ref visitor, ref changeTracker);
        }

        public static void Visit<TContainer, TVisitor>(
            ref TContainer container, 
            TVisitor visitor, 
            IVersionStorage versionStorage = null)
            where TVisitor : IPropertyVisitor
        {
            var changeTracker = new ChangeTracker(versionStorage);
            Visit(ref container, ref visitor, ref changeTracker);
        }

        public static void Visit<TContainer, TVisitor>(
            TContainer container, 
            ref TVisitor visitor, 
            IVersionStorage versionStorage = null)
            where TVisitor : IPropertyVisitor
        {
            var changeTracker = new ChangeTracker(versionStorage);
            Visit(ref container, ref visitor, ref changeTracker);
        }
        
        public static void Visit<TContainer, TVisitor>(
            ref TContainer container, 
            ref TVisitor visitor, 
            IVersionStorage versionStorage = null)
            where TVisitor : IPropertyVisitor
        {
            var changeTracker = new ChangeTracker(versionStorage);
            Visit(ref container, ref visitor, ref changeTracker);
        }

        public static void Visit<TContainer, TVisitor>(
            ref TContainer container, 
            TVisitor visitor, 
            ref ChangeTracker changeTracker)
            where TVisitor : IPropertyVisitor
        {
            Visit(ref container, ref visitor, ref changeTracker);
        }

        public static void Visit<TContainer, TVisitor>(
            ref TContainer container, 
            ref TVisitor visitor, 
            ref ChangeTracker changeTracker)
            where TVisitor : IPropertyVisitor
        {
            if (!RuntimeTypeInfoCache<TContainer>.IsValueType() && null != container && typeof(TContainer) != container.GetType())
            {
                var boxed = (object) container;
                PropertyBagResolver.Resolve(container.GetType())?.Accept(ref boxed, ref visitor, ref changeTracker);
                container = (TContainer) boxed;
            }
            else
            {
                PropertyBagResolver.Resolve<TContainer>()?.Accept(ref container, ref visitor, ref changeTracker);
            }
        }
        
        public static void VisitAtPath<TContainer, TVisitor>(ref TContainer container, PropertyPath path, TVisitor visitor, ref ChangeTracker changeTracker)
            where TVisitor : IPropertyVisitor
        {
            Actions.VisitAtPath(ref container, path, 0, visitor, ref changeTracker);
        }
        
        public static bool TryVisitAtPath<TContainer, TVisitor>(ref TContainer container, PropertyPath path, TVisitor visitor, ref ChangeTracker changeTracker)
            where TVisitor : IPropertyVisitor
        {
            return Actions.TryVisitAtPath(ref container, path, 0, visitor, ref changeTracker);
        }
    }
}