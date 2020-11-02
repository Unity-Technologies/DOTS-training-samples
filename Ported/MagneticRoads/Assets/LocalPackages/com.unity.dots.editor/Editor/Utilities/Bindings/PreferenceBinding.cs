using Unity.Properties;
using Unity.Serialization.Editor;
using UnityEngine.UIElements;

namespace Unity.Entities.Editor
{
    abstract class PreferenceBinding<TSetting, TValue> : IBinding
        where TSetting : class, ISetting, new()
    {
        VisualElement m_Target;
        protected abstract string SettingsKey { get; }

        protected TValue Value
        {
            get
            {
                var settings = UserSettings<TSetting>.GetOrCreate(SettingsKey);
                return PropertyContainer.TryGetValue<TSetting, TValue>(ref settings, PreferencePath, out var value) ? value : default;
            }
        }

        public PropertyPath PreferencePath;
        public VisualElement Target
        {
            get => m_Target;
            set
            {
                if (m_Target == value)
                    return;

                if (null != m_Target)
                    ReleaseTarget();

                m_Target = value;

                if (null != m_Target)
                    SetupTarget();
            }
        }

        void IBinding.PreUpdate()
        {
        }

        void IBinding.Update()
        {
            OnUpdate(Value);
        }

        void IBinding.Release()
        {
        }

        protected abstract void OnUpdate(TValue value);

        protected virtual void ReleaseTarget()
        {
        }

        protected virtual void SetupTarget()
        {
        }
    }
}
