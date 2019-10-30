using System;
using JetBrains.Annotations;

namespace Unity.Coding.Utils
{
    public class DelegateDisposable : IDisposable
    {
        readonly Action m_DisposeAction;

        public DelegateDisposable([NotNull] Action disposeAction) => m_DisposeAction = disposeAction;
        public void Dispose() => m_DisposeAction();
    }
}
