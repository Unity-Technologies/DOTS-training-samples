using System;
using System.Collections.Generic;

namespace Unity.Properties
{
    static partial class Actions
    {
        public static bool TrySetValue<TContainer, TTargetValue>(ref TContainer container, PropertyPath propertyPath,
            int propertyPathIndex, TTargetValue value, ref ChangeTracker changeTracker)
        {
            return TrySetValueImpl(ref container, propertyPath, propertyPathIndex, value, ref changeTracker) == VisitErrorCode.Ok;
        }

        public static void SetValue<TContainer, TTargetValue>(ref TContainer container, PropertyPath propertyPath,
            int propertyPathIndex, TTargetValue value, ref ChangeTracker changeTracker)
        {
            var status = TrySetValueImpl(ref container, propertyPath, propertyPathIndex, value, ref changeTracker);
            switch (status)
            {
                case VisitErrorCode.InvalidPath: throw new ArgumentException($"Could not set value at `{propertyPath}`");
                case VisitErrorCode.InvalidCast: throw new InvalidCastException($"Could not set value of type {typeof(TTargetValue).Name} at `{propertyPath}`");
            }
        }

        static VisitErrorCode TrySetValueImpl<TContainer, TTargetValue>(ref TContainer container, PropertyPath propertyPath,
            int propertyPathIndex, TTargetValue value, ref ChangeTracker changeTracker)
        {
            var action = new SetValueAtPathAction<TContainer, TTargetValue>(propertyPath, propertyPathIndex, value);
            if (PropertyBagResolver.Resolve<TContainer>()
                .FindProperty(propertyPath[propertyPathIndex].Name, ref container, ref changeTracker, ref action))
            {
                return action.ErrorCode;
            }

            if (typeof(TContainer) != container.GetType())
            {
                return SetValueCallback<TTargetValue>.TryExecute(container, propertyPath, propertyPathIndex, value, ref changeTracker);
            }

            return VisitErrorCode.InvalidPath;
        }
        
        static VisitErrorCode VisitSetValueProperty<TContainer, TProperty, TPropertyValue, TTargetValue>(
            TProperty property,
            ref TContainer container, PropertyPath propertyPath, int propertyPathIndex, TTargetValue value,
            ref ChangeTracker changeTracker)
            where TProperty : IProperty<TContainer, TPropertyValue>
        {
            if (propertyPathIndex < propertyPath.PartsCount - 1)
            {
                var sub = property.GetValue(ref container);
                var status = TrySetValueImpl(ref sub, propertyPath, propertyPathIndex + 1, value, ref changeTracker);
                if (status == VisitErrorCode.Ok)
                {
                    property.SetValue(ref container, sub);
                }

                return status;
            }

            if (TypeConversion.TryConvert(value, out TPropertyValue convertedValue))
            {
                property.SetValue(ref container, convertedValue);
                return VisitErrorCode.Ok;
            }

            return VisitErrorCode.InvalidCast;
        }

        static VisitErrorCode VisitCollectionSetValueProperty<TContainer, TProperty, TPropertyValue, TTargetValue>(
            TProperty property, ref TContainer container, PropertyPath propertyPath, int propertyPathIndex,
            TTargetValue value,
            ref ChangeTracker changeTracker)
            where TProperty : ICollectionProperty<TContainer, TPropertyValue>
        {
            if (propertyPathIndex < propertyPath.PartsCount - 1 || propertyPath[propertyPathIndex].IsListItem)
            {
                var getter =
                    new SetCollectionItemGetter<TContainer, TTargetValue>(propertyPath, propertyPathIndex, value);
                property.GetPropertyAtIndex(ref container, propertyPath[propertyPathIndex].Index, ref changeTracker,
                    ref getter);
                return getter.ErrorCode;
            }

            if (TypeConversion.TryConvert(value, out TPropertyValue convertedValue))
            {
                property.SetValue(ref container, convertedValue);
                return VisitErrorCode.Ok;
            }

            return VisitErrorCode.InvalidCast;
        }

        struct SetValueAtPathAction<TContainer, TTargetValue> : IPropertyGetter<TContainer>
        {
            private readonly PropertyPath m_Path;
            private readonly int m_PropertyPathIndex;
            private readonly TTargetValue m_Value;
            public VisitErrorCode ErrorCode;

            internal SetValueAtPathAction(PropertyPath propertyPath, int propertyPathIndex, TTargetValue value)
            {
                m_Path = propertyPath;
                m_PropertyPathIndex = propertyPathIndex;
                m_Value = value;
                ErrorCode = VisitErrorCode.Ok;
            }

            void IPropertyGetter<TContainer>.VisitProperty<TProperty, TPropertyValue>(TProperty property,
                ref TContainer container, ref ChangeTracker changeTracker) =>
                ErrorCode = VisitSetValueProperty<TContainer, TProperty, TPropertyValue, TTargetValue>(property, ref container,
                    m_Path,
                    m_PropertyPathIndex, m_Value, ref changeTracker);

            void IPropertyGetter<TContainer>.VisitCollectionProperty<TProperty, TPropertyValue>(TProperty property,
                ref TContainer container, ref ChangeTracker changeTracker) =>
                ErrorCode = VisitCollectionSetValueProperty<TContainer, TProperty, TPropertyValue, TTargetValue>(property,
                    ref container, m_Path, m_PropertyPathIndex, m_Value, ref changeTracker);
        }

        struct SetCollectionItemGetter<TContainer, TTargetValue> : ICollectionElementPropertyGetter<TContainer>
        {
            private readonly PropertyPath m_PropertyPath;
            private readonly int m_PropertyPathIndex;
            private readonly TTargetValue m_Value;
            public VisitErrorCode ErrorCode;

            internal SetCollectionItemGetter(PropertyPath propertyPath, int propertyPathIndex, TTargetValue value)
            {
                m_PropertyPath = propertyPath;
                m_PropertyPathIndex = propertyPathIndex;
                m_Value = value;
                ErrorCode = VisitErrorCode.Ok;
            }

            void ICollectionElementPropertyGetter<TContainer>.VisitProperty<TProperty, TPropertyValue>(
                TProperty property,
                ref TContainer container, ref ChangeTracker changeTracker) =>
                ErrorCode = VisitSetValueProperty<TContainer, TProperty, TPropertyValue, TTargetValue>(property, ref container,
                    m_PropertyPath,
                    m_PropertyPathIndex, m_Value, ref changeTracker);

            void ICollectionElementPropertyGetter<TContainer>.VisitCollectionProperty<TProperty, TPropertyValue>(
                TProperty property, ref TContainer container, ref ChangeTracker changeTracker) =>
                ErrorCode = VisitCollectionSetValueProperty<TContainer, TProperty, TPropertyValue, TTargetValue>(property,
                    ref container, m_PropertyPath, m_PropertyPathIndex, m_Value, ref changeTracker);
        }

        private struct SetValueCallback<TValue> : IContainerTypeCallback
        {
            readonly object m_Container;
            readonly PropertyPath m_PropertyPath;
            readonly int m_PropertyPathIndex;
            readonly TValue Value;
            ChangeTracker m_ChangeTracker;
            VisitErrorCode m_ErrorCode;

            private SetValueCallback(object container, PropertyPath propertyPath, int propertyPathIndex, TValue value, ref ChangeTracker changeTracker)
            {
                m_Container = container;
                m_PropertyPath = propertyPath;
                m_PropertyPathIndex = propertyPathIndex;
                Value = value;
                m_ChangeTracker = changeTracker;
                m_ErrorCode = VisitErrorCode.Ok;
            }

            public static VisitErrorCode TryExecute(object container, PropertyPath propertyPath, int propertyPathIndex, TValue value, ref ChangeTracker changeTracker)
            {
                var action = new SetValueCallback<TValue>(container, propertyPath, propertyPathIndex, value, ref changeTracker);
                PropertyBagResolver.Resolve(container.GetType()).Cast(ref action);
                changeTracker = action.m_ChangeTracker;
                return action.m_ErrorCode;
            }

            public void Invoke<T>()
            {
                var t = (T) m_Container;
                m_ErrorCode = TrySetValueImpl(ref t, m_PropertyPath, m_PropertyPathIndex, Value, ref m_ChangeTracker);
            }
        }
    }
}