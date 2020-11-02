using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Unity.Entities.Editor
{
    static class VisualElementExtensions
    {
        public static void ForceUpdateBindings(this VisualElement element)
        {
            using (var pooled = PooledList<IBinding>.Make())
            {
                var list = pooled.List;
                PopulateBindings(element, list);

                foreach (var binding in list)
                {
                    binding.PreUpdate();
                }

                foreach (var binding in list)
                {
                    binding.Update();
                }
            }
        }

        static void PopulateBindings(this VisualElement element, List<IBinding> list)
        {
            if (element is IBindable bindable && null != bindable.binding)
                list.Add(bindable.binding);

            if (element is IBinding binding)
                list.Add(binding);

            foreach (var child in element.Children())
            {
                PopulateBindings(child, list);
            }
        }
    }
}
