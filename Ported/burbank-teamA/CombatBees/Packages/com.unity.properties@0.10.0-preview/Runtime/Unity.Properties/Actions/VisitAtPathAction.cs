using System;

namespace Unity.Properties
{
    static partial class Actions
    {
        public static bool TryVisitAtPath<TContainer>(ref TContainer container, PropertyPath propertyPath,
            int propertyPathIndex, IPropertyVisitor visitor, ref ChangeTracker changeTracker)
        {
            return TryVisitAtPathImpl(ref container, propertyPath, propertyPathIndex, visitor, ref changeTracker) == VisitErrorCode.Ok;
        }

        public static void VisitAtPath<TContainer>(ref TContainer container, PropertyPath propertyPath,
            int propertyPathIndex, IPropertyVisitor visitor, ref ChangeTracker changeTracker)
        {
            var status = TryVisitAtPathImpl(ref container, propertyPath, propertyPathIndex, visitor, ref changeTracker);
            switch (status)
            {
                case VisitErrorCode.InvalidPath:
                    throw new ArgumentException($"`{propertyPath}` is not a valid path.");
            }
        }
        
        static VisitErrorCode TryVisitAtPathImpl<TContainer>(ref TContainer container, PropertyPath propertyPath,
            int propertyPathIndex, IPropertyVisitor visitor, ref ChangeTracker changeTracker)
        {
            var action = new VisitAtPathGetter<TContainer>(propertyPath, propertyPathIndex, visitor);
            if (PropertyBagResolver.Resolve<TContainer>()
                .FindProperty(propertyPath[propertyPathIndex].Name, ref container, ref changeTracker, ref action))
            {
                return action.ErrorCode;
            }

            if (typeof(TContainer) != container.GetType())
            {
                return VisitAtPathCallback.TryExecute(container, propertyPath, propertyPathIndex, visitor, ref changeTracker);
            }

            return VisitErrorCode.InvalidPath;
        }
        
        static VisitErrorCode VisitPropertyAtPath<TContainer, TProperty, TPropertyValue>(TProperty property,
            ref TContainer container, PropertyPath propertyPath, int propertyPathIndex, IPropertyVisitor visitor,
            ref ChangeTracker changeTracker)
            where TProperty : IProperty<TContainer, TPropertyValue>
        {
            if (propertyPathIndex < propertyPath.PartsCount - 1)
            {
                var sub = property.GetValue(ref container);
                var status = TryVisitAtPathImpl(ref sub, propertyPath, propertyPathIndex + 1, visitor, ref changeTracker); 
                if (status == VisitErrorCode.Ok)
                {
                    property.SetValue(ref container, sub);
                }
                return status;
            }
            else
            {
                visitor.VisitProperty<TProperty, TContainer, TPropertyValue>(property, ref container, ref changeTracker);
            }
            return VisitErrorCode.Ok;
        }

        private static VisitErrorCode VisitCollectionPropertyAtPath<TContainer, TProperty, TPropertyValue>(TProperty property,
            ref TContainer container, PropertyPath propertyPath, int propertyPathIndex, IPropertyVisitor visitor,
            ref ChangeTracker changeTracker)
            where TProperty : ICollectionProperty<TContainer, TPropertyValue>
        {
            if (propertyPathIndex < propertyPath.PartsCount - 1 || propertyPath[propertyPathIndex].IsListItem)
            {
                var index = propertyPath[propertyPathIndex].Index;
                if (index >= property.GetCount(ref container))
                {
                    return VisitErrorCode.InvalidPath;
                }
                var getter = new VisitCollectionItemAtPathGetter<TContainer>(propertyPath, propertyPathIndex, visitor);
                property.GetPropertyAtIndex(ref container, index, ref changeTracker, ref getter);
                return getter.ErrorCode;
            }

            visitor.VisitCollectionProperty<TProperty, TContainer, TPropertyValue>(property, ref container, ref changeTracker);

            return VisitErrorCode.Ok;
        }

        struct VisitAtPathGetter<TContainer> : IPropertyGetter<TContainer>
        {
            private readonly PropertyPath m_PropertyPath;
            private readonly int m_PropertyPathIndex;
            private readonly IPropertyVisitor m_Visitor;
            public VisitErrorCode ErrorCode;

            internal VisitAtPathGetter(PropertyPath propertyPath, int propertyPathIndex, IPropertyVisitor visitor)
            {
                m_PropertyPath = propertyPath;
                m_PropertyPathIndex = propertyPathIndex;
                m_Visitor = visitor;
                ErrorCode = VisitErrorCode.Ok;
            }

            void IPropertyGetter<TContainer>.VisitProperty<TProperty, TPropertyValue>(TProperty property,
                ref TContainer container, ref ChangeTracker changeTracker) =>
                ErrorCode = VisitPropertyAtPath<TContainer, TProperty, TPropertyValue>(property, ref container, m_PropertyPath,
                    m_PropertyPathIndex, m_Visitor, ref changeTracker);

            void IPropertyGetter<TContainer>.VisitCollectionProperty<TProperty, TPropertyValue>(TProperty property,
                ref TContainer container, ref ChangeTracker changeTracker) =>
                ErrorCode = VisitCollectionPropertyAtPath<TContainer, TProperty, TPropertyValue>(property, ref container,
                    m_PropertyPath, m_PropertyPathIndex, m_Visitor, ref changeTracker);
        }

        struct VisitCollectionItemAtPathGetter<TContainer> : ICollectionElementPropertyGetter<TContainer>
        {
            private readonly PropertyPath m_PropertyPath;
            private readonly int m_PropertyPathIndex;
            private readonly IPropertyVisitor m_Visitor;
            public VisitErrorCode ErrorCode;

            internal VisitCollectionItemAtPathGetter(PropertyPath propertyPath, int propertyPathIndex, IPropertyVisitor visitor)
            {
                m_PropertyPath = propertyPath;
                m_PropertyPathIndex = propertyPathIndex;
                m_Visitor = visitor;
                ErrorCode = VisitErrorCode.Ok;
            }

            void ICollectionElementPropertyGetter<TContainer>.VisitProperty<TProperty, TPropertyValue>(
                TProperty property, ref TContainer container, ref ChangeTracker changeTracker)
            {
                ErrorCode = VisitPropertyAtPath<TContainer, TProperty, TPropertyValue>(property, ref container, m_PropertyPath,
                    m_PropertyPathIndex, m_Visitor, ref changeTracker);
            }

            void ICollectionElementPropertyGetter<TContainer>.VisitCollectionProperty<TProperty, TPropertyValue>(
                TProperty property, ref TContainer container, ref ChangeTracker changeTracker)
            {
                ErrorCode = VisitCollectionPropertyAtPath<TContainer, TProperty, TPropertyValue>(property, ref container,
                    m_PropertyPath, m_PropertyPathIndex, m_Visitor, ref changeTracker);
            }
        }

        struct VisitAtPathCallback : IContainerTypeCallback
        {
            readonly object m_Container;
            readonly PropertyPath m_PropertyPath;
            readonly int m_PropertyPathIndex;
            readonly IPropertyVisitor m_Visitor;
            ChangeTracker m_ChangeTracker;
            VisitErrorCode errorCode;

            private VisitAtPathCallback(object container, PropertyPath propertyPath, int propertyPathIndex,
                IPropertyVisitor visitor, ref ChangeTracker changeTracker)
            {
                m_Container = container;
                m_PropertyPath = propertyPath;
                m_PropertyPathIndex = propertyPathIndex;
                m_ChangeTracker = changeTracker;
                m_Visitor = visitor;
                errorCode = VisitErrorCode.Ok;
            }

            public static VisitErrorCode TryExecute(object target, PropertyPath propertyName, int index, IPropertyVisitor visitor,
                ref ChangeTracker changeTracker)
            {
                var action = new VisitAtPathCallback(target, propertyName, index, visitor, ref changeTracker);
                PropertyBagResolver.Resolve(target.GetType()).Cast(ref action);
                changeTracker = action.m_ChangeTracker;
                return action.errorCode;
            }

            public void Invoke<T>()
            {
                var t = (T) m_Container;
                errorCode = TryVisitAtPathImpl(ref t, m_PropertyPath, m_PropertyPathIndex, m_Visitor, ref m_ChangeTracker);
            }
        }
    }
}
