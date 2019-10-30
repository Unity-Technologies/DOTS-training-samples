using UnityEngine.UIElements;

namespace Unity.Properties.Editor
{
    abstract class BaseFieldInspector<TField, TFieldValue, TValue> : IInspector<TValue>
        where TField : BaseField<TFieldValue>, new()
    {
        protected TField m_Field;
        
        protected abstract Connector<TFieldValue, TValue> Connector { get; }

        public virtual VisualElement Build(InspectorContext<TValue> context)
        {
            m_Field = new TField
            {
                label = context.PrettyName,
                tooltip = context.Tooltip
            };
            m_Field.RegisterValueChangedCallback(evt =>
            {
                var input = m_Field as TextInputBaseField<TFieldValue>;
                if (null != input)
                {
                    input.isDelayed = false;
                }
                OnChanged(evt, context);
                Update(context);
                if (null != input)
                {
                    input.isDelayed = context.IsDelayed;
                }
            });
            return m_Field;
        }

        public virtual void Update(InspectorContext<TValue> context)
        {
            m_Field.SetValueWithoutNotify(Connector.ToField(context.Data));
        }

        protected virtual void OnChanged(ChangeEvent<TFieldValue> evt, InspectorContext<TValue> context)
        {
            context.Data = Connector.ToValue(evt.newValue);
        }
    }
    
    abstract class BaseFieldInspector<TField, TValue> : IInspector<TValue>
        where TField : BaseField<TValue>, new()
    {
        protected TField m_Field;

        public virtual VisualElement Build(InspectorContext<TValue> context)
        {
            m_Field = new TField
            {
                label = context.PrettyName,
                tooltip = context.Tooltip
            };
            m_Field.RegisterValueChangedCallback(evt =>
            {
                var input = m_Field as TextInputBaseField<TValue>;
                if (null != input)
                {
                    input.isDelayed = false;
                }
                OnChanged(evt, context);
                Update(context);
                if (null != input)
                {
                    input.isDelayed = context.IsDelayed;
                }
            });
            return m_Field;
        }

        public virtual void Update(InspectorContext<TValue> context)
        {
            m_Field.SetValueWithoutNotify(context.Data);
        }
        
        protected virtual void OnChanged(ChangeEvent<TValue> evt, InspectorContext<TValue> context)
        {
            context.Data = evt.newValue;
        }
    }
}
