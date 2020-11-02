using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

namespace Unity.Entities.Editor
{
    sealed class AggregateBinding : IBinding
    {
        readonly List<IBinding> m_Bindings;

        public AggregateBinding(params IBinding[] bindings)
        {
            m_Bindings = bindings.ToList();
        }

        public void AddBinding(IBinding binding)
        {
            if (m_Bindings.Contains(binding))
                return;
            m_Bindings.Add(binding);
        }

        public bool RemoveBinding(IBinding binding)
        {
            return m_Bindings.Remove(binding);
        }

        public void PreUpdate()
        {
            foreach (var binding in m_Bindings)
            {
                binding.PreUpdate();
            }
        }

        public void Update()
        {
            foreach (var binding in m_Bindings)
            {
                binding.Update();
            }
        }

        public void Release()
        {
            foreach (var binding in m_Bindings)
            {
                binding.Release();
            }
        }
    }
}
