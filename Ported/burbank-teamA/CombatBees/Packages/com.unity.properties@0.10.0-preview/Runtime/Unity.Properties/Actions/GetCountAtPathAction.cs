using System;

namespace Unity.Properties
{
    static partial class Actions
    {
        public static bool TryGetCount<TContainer>(ref TContainer container, PropertyPath propertyPath, int propertyPathIndex,
            ref ChangeTracker changeTracker, out int count)
        {
            return TryGetCountImpl(ref container, propertyPath, propertyPathIndex, ref changeTracker, out count) == VisitErrorCode.Ok;
        }
        
        public static int GetCount<TContainer>(ref TContainer container, PropertyPath propertyPath, int propertyPathIndex,
            ref ChangeTracker changeTracker)
        {
            var status = TryGetCountImpl(ref container, propertyPath, propertyPathIndex, ref changeTracker, out var count);
            switch (status)
            {
                case VisitErrorCode.InvalidPath: throw new ArgumentException($"Cannot get collection count at `{propertyPath}`");
            }

            return count;
        }
        
        static VisitErrorCode TryGetCountImpl<TContainer>(ref TContainer container, PropertyPath propertyPath, int propertyPathIndex,
            ref ChangeTracker changeTracker, out int count)
        {
            var action = new GetCountAtPathGetter<TContainer>(propertyPath, propertyPathIndex);
            if (PropertyBagResolver.Resolve<TContainer>()
                .FindProperty(propertyPath[propertyPathIndex].Name, ref container, ref changeTracker, ref action))
            {
                count = action.Count;
                return action.ErrorCode;
            }

            if (typeof(TContainer) != container.GetType())
            {
                return GetCountFromActualTypeCallback.TryExecute(container, propertyPath, propertyPathIndex, ref changeTracker, out count);
            }

            count = -1;
            return VisitErrorCode.InvalidPath;
        }

        static VisitErrorCode VisitGetCountProperty<TContainer, TProperty, TValue>(TProperty property, ref TContainer container,
            PropertyPath propertyPath, int propertyPathIndex, ref ChangeTracker changeTracker, out int count)
            where TProperty : IProperty<TContainer, TValue>
        {
            if (propertyPathIndex >= propertyPath.PartsCount - 1)
            {
                count = -1;
                return VisitErrorCode.InvalidPath;
            }
            var value = property.GetValue(ref container);
            return TryGetCountImpl(ref value, propertyPath, propertyPathIndex + 1, ref changeTracker, out count);
        }

        static VisitErrorCode VisitCollectionGetCountProperty<TContainer, TProperty, TValue>(TProperty property,
            ref TContainer container, PropertyPath propertyPath, int propertyPathIndex, ref ChangeTracker changeTracker, out int count)
            where TProperty : ICollectionProperty<TContainer, TValue>
        {
            if (propertyPathIndex < propertyPath.PartsCount - 1 || propertyPath[propertyPathIndex].IsListItem)
            {
                var callback = new GetCollectionCountGetter<TContainer>(propertyPath, propertyPathIndex);
                property.GetPropertyAtIndex(ref container, propertyPath[propertyPathIndex].Index, ref changeTracker, ref callback);
                count = callback.Count;
                return callback.ErrorCode;
            }

            count = property.GetCount(ref container);
            return VisitErrorCode.Ok;
        }

        struct GetCountAtPathGetter<TContainer> : IPropertyGetter<TContainer>
        {
            private PropertyPath m_PropertyPath;
            private int m_PropertyPathIndex;
            public VisitErrorCode ErrorCode;

            public int Count;

            internal GetCountAtPathGetter(PropertyPath propertyPath, int propertyPathIndex)
            {
                m_PropertyPath = propertyPath;
                m_PropertyPathIndex = propertyPathIndex;
                Count = -1;
                ErrorCode = default;
            }

            void IPropertyGetter<TContainer>.VisitProperty<TProperty, TPropertyValue>(TProperty property,
                ref TContainer container, ref ChangeTracker changeTracker) =>
                ErrorCode = VisitGetCountProperty<TContainer, TProperty, TPropertyValue>(property, ref container, m_PropertyPath,
                    m_PropertyPathIndex, ref changeTracker, out Count);

            void IPropertyGetter<TContainer>.VisitCollectionProperty<TProperty, TPropertyValue>(TProperty property,
                ref TContainer container, ref ChangeTracker changeTracker) =>
                ErrorCode = VisitCollectionGetCountProperty<TContainer, TProperty, TPropertyValue>(property, ref container,
                    m_PropertyPath, m_PropertyPathIndex, ref changeTracker, out Count);
        }

        struct GetCollectionCountGetter<TContainer> : ICollectionElementPropertyGetter<TContainer>
        {
            readonly PropertyPath m_PropertyPath;
            readonly int m_PropertyPathIndex;
            public VisitErrorCode ErrorCode;

            public int Count;

            internal GetCollectionCountGetter(PropertyPath propertyPath, int propertyPathIndex)
            {
                m_PropertyPath = propertyPath;
                m_PropertyPathIndex = propertyPathIndex;
                Count = -1;
                ErrorCode = VisitErrorCode.Ok;
            }

            void ICollectionElementPropertyGetter<TContainer>.VisitProperty<TProperty, TPropertyValue>(
                TProperty property, ref TContainer container, ref ChangeTracker changeTracker) =>
                ErrorCode = VisitGetCountProperty<TContainer, TProperty, TPropertyValue>(property, ref container, m_PropertyPath,
                    m_PropertyPathIndex, ref changeTracker, out Count);

            void ICollectionElementPropertyGetter<TContainer>.VisitCollectionProperty<TProperty, TPropertyValue>(
                TProperty property, ref TContainer container, ref ChangeTracker changeTracker) =>
                ErrorCode = VisitCollectionGetCountProperty<TContainer, TProperty, TPropertyValue>(property, ref container,
                    m_PropertyPath, m_PropertyPathIndex, ref changeTracker, out Count);
        }
        
        struct GetCountFromActualTypeCallback : IContainerTypeCallback
        {
            readonly object m_Container;
            readonly PropertyPath m_PropertyPath;
            readonly int m_PropertyPathIndex;
            ChangeTracker m_ChangeTracker;
            int m_Count;
            VisitErrorCode m_ErrorCode;

            private GetCountFromActualTypeCallback(object container, PropertyPath propertyPath, int propertyPathIndex, ref ChangeTracker changeTracker)
            {
                m_Container = container;
                m_PropertyPath = propertyPath;
                m_PropertyPathIndex = propertyPathIndex;
                m_ChangeTracker = changeTracker;
                m_Count = -1;
                m_ErrorCode = VisitErrorCode.Ok;
            }

            public static VisitErrorCode TryExecute(object target, PropertyPath propertyName, int index, ref ChangeTracker changeTracker, out int count)
            {
                var action = new GetCountFromActualTypeCallback(target, propertyName, index, ref changeTracker);
                PropertyBagResolver.Resolve(target.GetType()).Cast(ref action);
                changeTracker = action.m_ChangeTracker;
                count = action.m_Count;
                return action.m_ErrorCode;
            }

            public void Invoke<T>()
            {
                var t = (T) m_Container;
                m_ErrorCode = TryGetCountImpl(ref t, m_PropertyPath, m_PropertyPathIndex, ref m_ChangeTracker, out m_Count);
            }
        }
    }
}
