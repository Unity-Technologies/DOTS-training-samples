using System;

#if WIP

// the intention here is to provide something similar to ITreeEnumerable, part of an
// ability to write abstract graph algorithms and then easily map onto types with
// existing parent structures.

namespace Unity.Coding.Utils
{
    public interface IHasParent<out T>
    {
        T Parent { get; }
    }

    public class HasParentDelegate<T> : IHasParent<T>
    {
        Func<T, T> m_ParentGetter;

        public HasParentDelegate(T @this, Func<T, T> parentGetter)
        {
            This = @this;
            m_ParentGetter = parentGetter;
        }

        public T Parent => m_ParentGetter(This);
        public T This { get; }
    }

    public static class HasParentDelegate
    {
        public static HasParentDelegate<T> Create<T>(T @this, Func<T, T> parentGetter)
            => new HasParentDelegate<T>(@this, parentGetter);
    }
}
#endif
