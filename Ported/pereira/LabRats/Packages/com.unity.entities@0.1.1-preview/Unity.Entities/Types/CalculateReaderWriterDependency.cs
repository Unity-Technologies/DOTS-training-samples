using Unity.Assertions;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace Unity.Entities
{
    internal static class CalculateReaderWriterDependency
    {
        public static bool Add(ComponentType type, ref UnsafeIntList reading, ref UnsafeIntList writing)
        {
            Assert.IsFalse(type == ComponentType.ReadWrite<Entity>());

            if (type.IsZeroSized)
                return false;

            if (type.AccessModeType == ComponentType.AccessMode.ReadOnly)
                return AddReaderTypeIndex(type.TypeIndex, ref reading, ref writing);
            else
                return AddWriterTypeIndex(type.TypeIndex, ref reading, ref writing);
        }

        public static bool AddReaderTypeIndex(int typeIndex, ref UnsafeIntList reading, ref UnsafeIntList writing)
        {
                if (reading.Contains(typeIndex))
                    return false;
                if (writing.Contains(typeIndex))
                    return false;

                reading.Add(typeIndex);
                return true;
        }

        public static bool AddWriterTypeIndex(int typeIndex, ref UnsafeIntList reading, ref UnsafeIntList writing)
        {
            if (writing.Contains(typeIndex))
                return false;

            var readingIndex = reading.IndexOf(typeIndex);
            if (readingIndex != -1)
                reading.RemoveAtSwapBack(readingIndex);

            writing.Add(typeIndex);
            return true;
        }
    }
}
