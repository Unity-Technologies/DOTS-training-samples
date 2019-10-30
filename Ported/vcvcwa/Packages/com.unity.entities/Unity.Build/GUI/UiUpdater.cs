using System;
using UnityEngine.UIElements;

namespace Unity.Build
{
    static class UIUpdaters
    {
        public static UIUpdater<TSource, TElement> MakeBinding<TSource, TElement>(TSource source, TElement element)
            where TElement : VisualElement
        {
            return new UIUpdater<TSource, TElement>(source, element);
        }
    }
    
    sealed class UIUpdater<TSource, TElement> : IBinding
        where TElement : VisualElement
    {
        public readonly TSource Source;
        public readonly TElement Element;
        
        public event Action<UIUpdater<TSource, TElement>> OnPreUpdate = delegate { };
        public event Action<UIUpdater<TSource, TElement>> OnUpdate = delegate { };
        public event Action<UIUpdater<TSource, TElement>> OnRelease = delegate { };

        public UIUpdater(TSource source, TElement element)
        {
            Source = source;
            Element = element;
        }
        
        public void PreUpdate()
        {
            OnPreUpdate(this);
        }

        public void Update()
        {
            OnUpdate(this);
        }

        public void Release()
        {
            OnRelease(this);
        }
    }
}