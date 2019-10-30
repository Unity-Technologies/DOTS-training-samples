using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Unity.Properties.Editor
{
    class InspectorVisitorContext
    {
        internal struct ParentScope : IDisposable
        {
            readonly InspectorVisitorContext m_Context;
            readonly VisualElement m_Parent;
            
            public ParentScope(InspectorVisitorContext context, VisualElement parent)
            {
                m_Context = context;
                m_Parent = parent;
                m_Context.PushParent(m_Parent);
            }
            
            public void Dispose()
            {
                m_Context.PopParent(m_Parent);
            }
        }
        
        readonly Stack<VisualElement> m_ParentStack;
        public readonly PropertyElement Binding;
        
        internal InspectorVisitorContext(PropertyElement binding)
        {
            m_ParentStack = new Stack<VisualElement>();
            Binding = binding;
        }

        public ParentScope MakeParentScope(VisualElement parent)
        {
            return new ParentScope(this, parent);
        }
        
        private void PushParent(VisualElement parent)
        {
            m_ParentStack.Push(parent);
        }

        private void PopParent(VisualElement parent)
        {
            if (m_ParentStack.Peek() == parent)
            {
                m_ParentStack.Pop();
            }
            else
            {
                Debug.LogError($"{nameof(InspectorVisitorContext)}.{nameof(MakeParentScope)} was not properly disposed for parent: {parent?.name}");
            }
        }

        public VisualElement Parent
        {
            get
            {
                if (m_ParentStack.Count > 0)
                {
                    return m_ParentStack.Peek();
                }
                throw new InvalidOperationException($"A parent element must be set.");
            }
        }
    }
}
