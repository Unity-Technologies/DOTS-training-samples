namespace Unity.Properties
{
    public interface IContainerTypeCallback
    {
        void Invoke<T>();
    }

    public interface IPropertyBag
    {
        void Accept<TVisitor>(ref object container, ref TVisitor visitor, ref ChangeTracker changeTracker)
            where TVisitor : IPropertyVisitor;

        void Cast<TCallback>(ref TCallback callback)
            where TCallback : IContainerTypeCallback;
    }

    public interface IPropertyBag<TContainer> : IPropertyBag
    {
        void Accept<TVisitor>(ref TContainer container, ref TVisitor visitor, ref ChangeTracker changeTracker)
            where TVisitor : IPropertyVisitor;

        bool FindProperty<TAction>(string name, ref TContainer container, ref ChangeTracker changeTracker, ref TAction action)
            where TAction : IPropertyGetter<TContainer>;
    }

    public abstract class PropertyBag<TContainer> : IPropertyBag<TContainer>
    {
        public void Accept<TVisitor>(ref object container, ref TVisitor visitor, ref ChangeTracker changeTracker)
            where TVisitor : IPropertyVisitor
        {
            var unboxed = (TContainer) container;
            Accept(ref unboxed, ref visitor, ref changeTracker);
            container = unboxed;
        }

        public void Cast<TCallback>(ref TCallback callback) where TCallback : IContainerTypeCallback
        {
            callback.Invoke<TContainer>();
        }

        public abstract void Accept<TVisitor>(ref TContainer container, ref TVisitor visitor, ref ChangeTracker changeTracker) where TVisitor : IPropertyVisitor;
        public abstract bool FindProperty<TAction>(string name, ref TContainer container, ref ChangeTracker changeTracker, ref TAction action) where TAction : IPropertyGetter<TContainer>;
    }
}
