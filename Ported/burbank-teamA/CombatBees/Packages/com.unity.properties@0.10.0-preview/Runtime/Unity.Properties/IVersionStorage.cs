namespace Unity.Properties
{
    /// <summary>
    /// Interface to implement to get change detection for operations on a property tree.
    /// </summary>
    public interface IVersionStorage
    {
        void IncrementVersion<TProperty, TContainer, TValue>(TProperty property, ref TContainer container)
            where TProperty : IProperty<TContainer, TValue>;
    }

    public struct ChangeTracker
    {
        int m_Version;

        public IVersionStorage VersionStorage { get; }

        public bool IsChanged() => m_Version > 0;
        public void MarkChanged() => m_Version++;

        public ChangeTracker(IVersionStorage versionStorage)
        {
            VersionStorage = versionStorage;
            m_Version = 0;
        }

        internal void IncrementVersion<TProperty, TContainer, TValue>(TProperty property, ref TContainer container)
            where TProperty : IProperty<TContainer, TValue>
        {
            VersionStorage?.IncrementVersion<TProperty, TContainer, TValue>(property, ref container);
            m_Version++;
        }
    }
}
