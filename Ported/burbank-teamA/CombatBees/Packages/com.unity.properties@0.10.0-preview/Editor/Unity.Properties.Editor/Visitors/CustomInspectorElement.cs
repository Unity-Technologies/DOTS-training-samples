using UnityEngine.UIElements;

namespace Unity.Properties.Editor
{
    class CustomInspectorElement<T> : VisualElement, IBindable, IBinding
    {
        readonly IInspector<T> m_Inspector;
        readonly InspectorContext<T> m_Context;

        public IBinding binding
        {
            get => this;
            set { }
        } 
        
        public string bindingPath { get; set; }

        public CustomInspectorElement(IInspector<T> inspector, InspectorContext<T> context)
        {
            m_Inspector = inspector;
            m_Context = context;

            Add(m_Inspector.Build(context));
        }
        
        public void PreUpdate()
        {
        }

        public void Update()
        {
            m_Inspector.Update(m_Context);
        }

        public void Release()
        {
        }
    }
}