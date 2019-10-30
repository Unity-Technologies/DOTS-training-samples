using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Unity.Burst.Editor
{
    internal class LongTextArea
    {
        private const int kMaxFragment = 2048;

        private struct Fragment
        {
            public int lineCount;
            public string text;
        }

        private string m_Text = "";
        private List<Fragment> m_Fragments = null;

        public string Text
        {
            get {
                return m_Text;
            }
            set {
                if (value != m_Text)
                {
                    m_Text = value;
                    m_Fragments = RecomputeFragments(m_Text);
                }
            }
        }

        public void Render(GUIStyle style)
        {
            style.richText = true;

            foreach (var fragment in m_Fragments)
            {
                GUILayout.Label(fragment.text, style);
            }
        }

        private static List<Fragment> RecomputeFragments(string text)
        {
            List<Fragment> result = new List<Fragment>();

            string[] pieces = text.Split('\n');

            StringBuilder b = new StringBuilder();
            int lineCount = 0;

            foreach (var piece in pieces)
            {
                if (b.Length >= kMaxFragment)
                {
                    AddFragment(b, lineCount, result);
                    lineCount = 0;
                }

                if (b.Length > 0)
                    b.Append('\n');

                b.Append(piece);
                lineCount++;
            }

            AddFragment(b, lineCount, result);

            return result;
        }

        private static void AddFragment(StringBuilder b, int lineCount, List<Fragment> result)
        {
            result.Add(new Fragment() { text = b.ToString(), lineCount = lineCount });
            b.Length = 0;
        }
    }

}
