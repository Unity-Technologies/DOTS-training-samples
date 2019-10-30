using System;

namespace Unity.Properties
{
    static partial class Actions
    {
        public static bool TrySetCount<TContainer>(ref TContainer container, PropertyPath propertyPath, int propertyPathIndex,
            int count, ref ChangeTracker changeTracker)
        {
            return TrySetCountImpl(ref container, propertyPath, propertyPathIndex, count, ref changeTracker) == VisitErrorCode.Ok;
        }
        
        public static void SetCount<TContainer>(ref TContainer container, PropertyPath propertyPath, int propertyPathIndex,
            int count, ref ChangeTracker changeTracker)
        {
            var status = TrySetCountImpl(ref container, propertyPath, propertyPathIndex, count, ref changeTracker);
            switch (status)
            {
                case VisitErrorCode.InvalidPath: throw new ArgumentException($"Could not set count at `{propertyPath}`");
            }
        }
        
        static VisitErrorCode TrySetCountImpl<TContainer>(ref TContainer container, PropertyPath propertyPath, int propertyPathIndex,
            int count, ref ChangeTracker changeTracker)
        {
            var action = new SetCountAtPathAction<TContainer>(propertyPath, propertyPathIndex, count);
            if (PropertyBagResolver.Resolve<TContainer>()
                .FindProperty(propertyPath[propertyPathIndex].Name, ref container, ref changeTracker, ref action))
            {
                return action.ErrorCode;
            }

            if (typeof(TContainer) != container.GetType())
            {
                return SetCountCallback.TryExecute(container, propertyPath, propertyPathIndex, count, ref changeTracker);
            }

            return VisitErrorCode.InvalidPath;
        }

        static VisitErrorCode VisitSetCountProperty<TContainer, TProperty, TPropertyValue>(TProperty property,
            ref TContainer container, PropertyPath propertyPath, int propertyPathIndex, int count, ref ChangeTracker changeTracker)
            where TProperty : IProperty<TContainer, TPropertyValue>
        {
            if (propertyPathIndex < propertyPath.PartsCount - 1)
            {
                var value = property.GetValue(ref container);
                var status = TrySetCountImpl(ref value, propertyPath, propertyPathIndex + 1, count, ref changeTracker);
                if (status == VisitErrorCode.Ok)
                {
                    property.SetValue(ref container, value);
                }

                return status;
            }

            return VisitErrorCode.InvalidPath;
        }

        static VisitErrorCode VisitCollectionSetCountProperty<TContainer, TProperty, TPropertyValue>(TProperty property,
            ref TContainer container, PropertyPath propertyPath, int propertyPathIndex, int count, ref ChangeTracker changeTracker)
            where TProperty : ICollectionProperty<TContainer, TPropertyValue>
        {
            if (propertyPathIndex < propertyPath.PartsCount - 1)
            {
                var getter = new SetCollectionCountGetter<TContainer>(propertyPath, propertyPathIndex, count);
                property.GetPropertyAtIndex(ref container, propertyPath[propertyPathIndex].Index, ref changeTracker, ref getter);
                return getter.ErrorCode;
            }

            var value = property.GetValue(ref container);
            property.SetCount(ref container, count);
            property.SetValue(ref container, value);
            return VisitErrorCode.Ok;
        }

        struct SetCountAtPathAction<TContainer> : IPropertyGetter<TContainer>
        {
            private readonly PropertyPath m_PropertyPath;
            private readonly int m_PropertyPathIndex;
            private readonly int m_Count;
            public VisitErrorCode ErrorCode;

            internal SetCountAtPathAction(PropertyPath propertyPath, int propertyPathIndex, int count)
            {
                m_PropertyPath = propertyPath;
                m_PropertyPathIndex = propertyPathIndex;
                m_Count = count;
                ErrorCode = VisitErrorCode.Ok;
            }

            void IPropertyGetter<TContainer>.VisitProperty<TProperty, TPropertyValue>(TProperty property,
                ref TContainer container, ref ChangeTracker changeTracker) =>
                ErrorCode = VisitSetCountProperty<TContainer, TProperty, TPropertyValue>(property, ref container, m_PropertyPath, m_PropertyPathIndex,
                    m_Count, ref changeTracker);

            void IPropertyGetter<TContainer>.VisitCollectionProperty<TProperty, TPropertyValue>(TProperty property,
                ref TContainer container, ref ChangeTracker changeTracker) =>
                ErrorCode = VisitCollectionSetCountProperty<TContainer, TProperty, TPropertyValue>(property, ref container, m_PropertyPath,
                    m_PropertyPathIndex, m_Count, ref changeTracker);
        }

        struct SetCollectionCountGetter<TContainer> : ICollectionElementPropertyGetter<TContainer>
        {
            readonly PropertyPath m_PropertyPath;
            readonly int m_PropertyPathIndex;
            readonly int m_Count;
            public VisitErrorCode ErrorCode;

            internal SetCollectionCountGetter(PropertyPath propertyPath, int propertyPathIndex, int count)
            {
                m_PropertyPath = propertyPath;
                m_PropertyPathIndex = propertyPathIndex;
                m_Count = count;
                ErrorCode = VisitErrorCode.Ok;
            }

            void ICollectionElementPropertyGetter<TContainer>.VisitProperty<TProperty, TPropertyValue>(
                TProperty property, ref TContainer container, ref ChangeTracker changeTracker) =>
                ErrorCode = VisitSetCountProperty<TContainer, TProperty, TPropertyValue>(property, ref container, m_PropertyPath, m_PropertyPathIndex,
                    m_Count, ref changeTracker);

            void ICollectionElementPropertyGetter<TContainer>.VisitCollectionProperty<TProperty, TPropertyValue>(
                TProperty property, ref TContainer container, ref ChangeTracker changeTracker) =>
                ErrorCode = VisitCollectionSetCountProperty<TContainer, TProperty, TPropertyValue>(property, ref container, m_PropertyPath,
                    m_PropertyPathIndex, m_Count, ref changeTracker);
        }

        private struct SetCountCallback : IContainerTypeCallback
        {
            readonly object m_Container;
            readonly PropertyPath m_PropertyPath;
            readonly int m_PropertyPathIndex;
            readonly int m_Count;
            ChangeTracker m_ChangeTracker;
            VisitErrorCode m_ErrorCode;

            private SetCountCallback(object container, PropertyPath propertyPath, int propertyPathIndex, int count, ref ChangeTracker changeTracker)
            {
                m_Container = container;
                m_PropertyPath = propertyPath;
                m_PropertyPathIndex = propertyPathIndex;
                m_Count = count;
                m_ChangeTracker = changeTracker;
                m_ErrorCode = VisitErrorCode.Ok;
            }

            public static VisitErrorCode TryExecute(object container, PropertyPath propertyPath, int propertyPathIndex, int count, ref ChangeTracker changeTracker)
            {
                var action = new SetCountCallback(container, propertyPath, propertyPathIndex, count, ref changeTracker);
                PropertyBagResolver.Resolve(container.GetType()).Cast(ref action);
                changeTracker = action.m_ChangeTracker;
                return action.m_ErrorCode;
            }

            public void Invoke<T>()
            {
                var t = (T) m_Container;
                m_ErrorCode = TrySetCountImpl(ref t, m_PropertyPath, m_PropertyPathIndex, m_Count, ref m_ChangeTracker);
            }
        }
    }
}
