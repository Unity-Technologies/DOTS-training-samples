
using System;

namespace UnityEditor.Performance.ProfileAnalyzer
{
    public struct ThreadIdentifier
    {
        public string threadNameWithIndex { get; private set; }
        public string name { get; private set; }
        public int index { get; private set; }

        public static int kAll = -1;
        public static int kSingle = 0;

        public ThreadIdentifier(string name, int index)
        {
            this.name = name;
            this.index = index;
            if (index == kAll)
                threadNameWithIndex = string.Format("All:{1}", index, name);
            else
                threadNameWithIndex = string.Format("{0}:{1}", index, name);
        }

        public ThreadIdentifier(ThreadIdentifier threadIdentifier)
        {
            name = threadIdentifier.name;
            index = threadIdentifier.index;
            threadNameWithIndex = threadIdentifier.threadNameWithIndex;
        }

        public ThreadIdentifier(string threadNameWithIndex)
        {
            this.threadNameWithIndex = threadNameWithIndex;

            string[] tokens = threadNameWithIndex.Split(':');
            if (tokens.Length >= 2)
            {
                name = tokens[1];
                string indexString = tokens[0];
                if (indexString == "All")
                {
                    index = kAll;
                }
                else
                {
                    int intValue;
                    if (Int32.TryParse(tokens[0], out intValue))
                        index = intValue;
                    else
                        index = kSingle;
                }
            }
            else
            {
                index = kSingle;
                name = threadNameWithIndex;
            }
        }

        void UpdateThreadNameWithIndex()
        {
            if (index == kAll)
                threadNameWithIndex = string.Format("All:{1}", index, name);
            else
                threadNameWithIndex = string.Format("{0}:{1}", index, name);
        }

        public void SetName(string newName)
        {
            name = newName;
            UpdateThreadNameWithIndex();
        }

        public void SetIndex(int newIndex)
        {
            index = newIndex;
            UpdateThreadNameWithIndex();
        }

        public void SetAll()
        {
            SetIndex(kAll);
        }
    }
}
