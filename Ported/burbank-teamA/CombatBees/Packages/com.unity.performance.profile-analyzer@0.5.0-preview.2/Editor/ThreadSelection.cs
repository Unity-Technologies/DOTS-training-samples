using System.Collections.Generic;

namespace UnityEditor.Performance.ProfileAnalyzer
{
    public class ThreadSelection
    {
        public List<string> groups;
        public List<string> selection;

        public ThreadSelection()
        {
            groups = new List<string>();
            selection = new List<string>();
        }

        public ThreadSelection(ThreadSelection threadSelection)
        {
            groups = new List<string>();
            selection = new List<string>();

            Set(threadSelection);
        }

        public void SetAll()
        {
            groups.Clear();
            selection.Clear();

            ThreadIdentifier allThreadSelection = new ThreadIdentifier("All",ThreadIdentifier.kAll);
            groups.Add(allThreadSelection.threadNameWithIndex);
        }

        public void Set(string name)
        {
            groups.Clear();
            selection.Clear();
            selection.Add(name);
        }

        public void SetGroup(string groupName)
        {
            groups.Clear();
            selection.Clear();

            ThreadIdentifier allThreadSelection = new ThreadIdentifier(groupName,ThreadIdentifier.kAll);
            groups.Add(allThreadSelection.threadNameWithIndex);
        }

        public void Set(ThreadSelection threadSelection)
        {
            groups.Clear();
            selection.Clear();

            if (threadSelection.groups != null)
                groups.AddRange(threadSelection.groups);
            if (threadSelection.selection != null)
                selection.AddRange(threadSelection.selection);
        }
    }
}
