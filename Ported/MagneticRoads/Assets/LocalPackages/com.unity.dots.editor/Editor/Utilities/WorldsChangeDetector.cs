namespace Unity.Entities.Editor
{
    class WorldsChangeDetector
    {
        int m_Count;
        ulong m_CurrentHash;

        public bool WorldsChanged()
        {
            var newCount = World.All.Count;
            ulong newHash = 0;
            foreach (var world in World.All)
                newHash += world.SequenceNumber;

            var hasChanged = m_Count != newCount || newHash != m_CurrentHash;
            m_CurrentHash = newHash;
            m_Count = newCount;
            return hasChanged;
        }
    }
}
