using System;
using System.Collections.Generic;
using System.Text;

namespace Unity.Properties
{
    public class PropertyPath
    {
        public const int InvalidListIndex = -1;

        public struct Part
        {
            public string Name;
            public int Index;
            public bool IsListItem => Index >= 0;
        }

        readonly List<Part> m_Parts;

        public int PartsCount => m_Parts.Count;
        public Part this[int index] => m_Parts[index];

        public PropertyPath()
            :this(string.Empty)
        {
        }

        public PropertyPath(string path)
        {
            m_Parts = new List<Part>(32);
            ConstructFromPath(path);   
        }

        public void Push(string name, int index = InvalidListIndex)
        {
            if (index < 0)
            {
                index = InvalidListIndex;
            }

            m_Parts.Add(new Part
            {
                Name = name,
                Index = index
            });
        }

        public void Append(PropertyPath path)
        {
            for (var i = 0; i < path.PartsCount; ++i)
            {
                var part = path[i];
                Push(part.Name, part.Index);
            }
        }

        public void Pop()
        {
            m_Parts.RemoveAt(m_Parts.Count - 1);
        }

        public void Clear()
        {
            m_Parts.Clear();
        }

        public override string ToString()
        {
            if (m_Parts.Count == 0)
            {
                return string.Empty;
            }

            var builder = new StringBuilder(32);

            foreach (var part in m_Parts)
            {
                if (builder.Length > 0)
                {
                    builder.Append('.');
                }

                builder.Append(part.Name);
                if (part.IsListItem)
                {
                    builder.Append($"[{part.Index}]");
                }
            }

            return builder.ToString();
        }

        private void ConstructFromPath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return;
            }
            
            var parts = path.Split('.');
            for (var i = 0; i < parts.Length; ++i)
            {
                var part = parts[i];

                if (part.EndsWith("]"))
                {
                    var lastIndex = part.LastIndexOf("[", StringComparison.InvariantCultureIgnoreCase);
                    if (lastIndex > 0)
                    {
                        var indexStr = part.Substring(lastIndex + 1, part.Length - 2 - lastIndex);
                        if (int.TryParse(indexStr, out var index))
                        {
                            if (index < 0)
                            {
                                throw new ArgumentException($"Negative indices in {nameof(PropertyPath)} are not supported.");    
                            }
                            Push(part.Remove(lastIndex), index);
                        }
                        else
                        {
                            throw new ArgumentException($"Indices in {nameof(PropertyPath)} must be a numeric value.");
                        }
                    }
                    else
                    {
                        throw new ArgumentException($"Indices may not be at the root of a {nameof(PropertyPath)}");
                    }
                }
                else
                {
                    Push(part);
                }
            }
        }
    }
}