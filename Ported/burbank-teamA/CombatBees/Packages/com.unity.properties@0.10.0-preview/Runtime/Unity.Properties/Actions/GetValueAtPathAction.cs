using System;

namespace Unity.Properties
{
    static partial class Actions
    {
        public static bool TryGetValue<TContainer, TTargetValue>(ref TContainer container, PropertyPath propertyPath,
            int propertyPathIndex, ref ChangeTracker changeTracker, out TTargetValue value)
        {
            return TryGetValueImpl(ref container, propertyPath, propertyPathIndex, ref changeTracker, out value) == VisitErrorCode.Ok;
        }
        
        public static TTargetValue GetValue<TContainer, TTargetValue>(ref TContainer container, PropertyPath propertyPath,
            int propertyPathIndex, ref ChangeTracker changeTracker)
        {
            var status = TryGetValueImpl(ref container, propertyPath, propertyPathIndex, ref changeTracker, out TTargetValue value);
            switch (status)
            {
                case VisitErrorCode.InvalidPath:
                    throw new ArgumentException($"Cannot get collection count at `{propertyPath}`");
                case VisitErrorCode.InvalidCast:
                    throw new InvalidCastException($"Could not get value of type {typeof(TTargetValue).Name} at `{propertyPath}`");
            }

            return value;
        }

        static VisitErrorCode TryGetValueImpl<TContainer, TTargetValue>(ref TContainer container, PropertyPath propertyPath,
            int propertyPathIndex, ref ChangeTracker changeTracker, out TTargetValue value)
        {
            var action = new GetValueAtPathAction<TContainer, TTargetValue>(propertyPath, propertyPathIndex);
            if (PropertyBagResolver.Resolve<TContainer>()
                .FindProperty(propertyPath[propertyPathIndex].Name, ref container, ref changeTracker, ref action))
            {
                value = action.Value;
                return action.ErrorCode;
            }

            if (typeof(TContainer) != container.GetType())
            {
                return GetValueFromActualTypeCallback<TTargetValue>.TryExecute(container, propertyPath, propertyPathIndex, ref changeTracker, out value);
            }

            value = default;
            return VisitErrorCode.InvalidPath;
        }
        
        static VisitErrorCode VisitGetValueProperty<TContainer, TProperty, TValue, TTargetValue>(TProperty property,
            ref TContainer container, PropertyPath propertyPath, int propertyPathIndex, ref ChangeTracker changeTracker, out TTargetValue value)
            where TProperty : IProperty<TContainer, TValue>
        {
            if (propertyPathIndex < propertyPath.PartsCount - 1)
            {
                var sub = property.GetValue(ref container);
                return TryGetValueImpl(ref sub, propertyPath, propertyPathIndex + 1, ref changeTracker, out value);
            }

            if (TypeConversion.TryConvert(property.GetValue(ref container), out value))
            {
                return VisitErrorCode.Ok;
            }

            return VisitErrorCode.InvalidCast;
        }

        static VisitErrorCode VisitCollectionGetValueProperty<TContainer, TProperty, TValue, TTargetValue>(
            TProperty property, ref TContainer container, PropertyPath propertyPath, int propertyPathIndex,
            ref ChangeTracker changeTracker, out TTargetValue value)
            where TProperty : ICollectionProperty<TContainer, TValue>
        {
            if (propertyPathIndex < propertyPath.PartsCount - 1 || propertyPath[propertyPathIndex].IsListItem)
            {
                var callback = new GetCollectionItemGetter<TContainer, TTargetValue>(propertyPath, propertyPathIndex);
                property.GetPropertyAtIndex(ref container, propertyPath[propertyPathIndex].Index, ref changeTracker,
                    ref callback);
                value = callback.Value;
                return callback.ErrorCode;
            }

            if (TypeConversion.TryConvert(property.GetValue(ref container), out value))
            {
                return VisitErrorCode.Ok;
            }

            return VisitErrorCode.InvalidCast;
        }

        struct GetValueAtPathAction<TContainer, TTargetValue> : IPropertyGetter<TContainer>
        {
            readonly PropertyPath m_PropertyPath;
            readonly int m_PropertyPathIndex;
            public VisitErrorCode ErrorCode;

            public TTargetValue Value;

            internal GetValueAtPathAction(PropertyPath propertyPath, int propertyPathIndex)
            {
                m_PropertyPath = propertyPath;
                m_PropertyPathIndex = propertyPathIndex;
                Value = default;
                ErrorCode = VisitErrorCode.Ok;
            }

            void IPropertyGetter<TContainer>.VisitProperty<TProperty, TPropertyValue>(TProperty property,
                ref TContainer container, ref ChangeTracker changeTracker) =>
                ErrorCode = VisitGetValueProperty<TContainer, TProperty, TPropertyValue, TTargetValue>(property,
                    ref container, m_PropertyPath, m_PropertyPathIndex, ref changeTracker, out Value);

            void IPropertyGetter<TContainer>.VisitCollectionProperty<TProperty, TPropertyValue>(TProperty property,
                ref TContainer container, ref ChangeTracker changeTracker) =>
                ErrorCode = VisitCollectionGetValueProperty<TContainer, TProperty, TPropertyValue, TTargetValue>(property,
                    ref container, m_PropertyPath, m_PropertyPathIndex, ref changeTracker, out Value);
        }

        struct GetCollectionItemGetter<TContainer, TTargetValue> : ICollectionElementPropertyGetter<TContainer>
        {
            readonly PropertyPath m_PropertyPath;
            readonly int m_PropertyPathIndex;
            public VisitErrorCode ErrorCode;
            public TTargetValue Value;

            internal GetCollectionItemGetter(PropertyPath propertyPath, int propertyPathIndex)
            {
                m_PropertyPath = propertyPath;
                m_PropertyPathIndex = propertyPathIndex;
                Value = default;
                ErrorCode = VisitErrorCode.Ok;
            }

            void ICollectionElementPropertyGetter<TContainer>.VisitProperty<TProperty, TPropertyValue>(
                TProperty property, ref TContainer container, ref ChangeTracker changeTracker) =>
                ErrorCode = VisitGetValueProperty<TContainer, TProperty, TPropertyValue, TTargetValue>(property,
                    ref container, m_PropertyPath, m_PropertyPathIndex, ref changeTracker, out Value);

            void ICollectionElementPropertyGetter<TContainer>.VisitCollectionProperty<TProperty, TPropertyValue>(
                TProperty property, ref TContainer container, ref ChangeTracker changeTracker) =>
                ErrorCode = VisitCollectionGetValueProperty<TContainer, TProperty, TPropertyValue, TTargetValue>(property,
                    ref container, m_PropertyPath, m_PropertyPathIndex, ref changeTracker, out Value);
        }

        struct GetValueFromActualTypeCallback<TValue> : IContainerTypeCallback
        {
            readonly object m_Container;
            readonly PropertyPath m_PropertyPath;
            readonly int m_PropertyPathIndex;
            ChangeTracker m_ChangeTracker;
            TValue m_Value;
            VisitErrorCode m_ErrorCode;

            private GetValueFromActualTypeCallback(object container, PropertyPath propertyPath, int propertyPathIndex, ref ChangeTracker changeTracker)
            {
                m_Container = container;
                m_PropertyPath = propertyPath;
                m_PropertyPathIndex = propertyPathIndex;
                m_ChangeTracker = changeTracker;
                m_Value = default;
                m_ErrorCode = VisitErrorCode.Ok;
            }

            public static VisitErrorCode TryExecute(object target, PropertyPath propertyName, int index, ref ChangeTracker changeTracker, out TValue value)
            {
                var action = new GetValueFromActualTypeCallback<TValue>(target, propertyName, index, ref changeTracker);
                PropertyBagResolver.Resolve(target.GetType()).Cast(ref action);
                value = action.m_Value;
                return action.m_ErrorCode;
            }

            public void Invoke<T>()
            {
                var t = (T) m_Container;
                m_ErrorCode = TryGetValueImpl<T, TValue>(ref t, m_PropertyPath, m_PropertyPathIndex, ref m_ChangeTracker, out m_Value); 
            }
        }
    }
}
