using System;
using System.Globalization;
using System.Text.RegularExpressions;
using EditorConfig.Core;

namespace Unity.Coding.Utils
{
    /// <summary>
    /// Parser for .editorconfig files optimized for retrieving a single property for a single path.
    /// </summary>
    /// <remarks>
    /// It is about 500x faster than EditorConfig.Core and allocates less than 1% the GC memory.
    /// For complex patterns it falls back to EditorConfig.Core's EditorConfigMinimatcher.
    /// </remarks>
    public static class FastEditorConfigParser
    {
        public static string ParseForSingleValue(NPath path, string key)
        {
            bool hitRoot = false;
            var currentDirectory = path.MakeAbsolute();
            //traverse up the directories until we hit a root editorconfig or the root of the path
            //Do we also want to stop at the project root? Or make it an option?
            while ((currentDirectory = currentDirectory.ParentContaining(".editorconfig")) != null && !hitRoot)
            {
                var relativePath = path.RelativeTo(currentDirectory);
                var value = FindValue(relativePath, currentDirectory, key, currentDirectory.Combine(".editorconfig"), out hitRoot);
                if (value != null)
                    return value;
            }

            return null;
        }

        static string FindValue(string path, NPath currentDirectory, string key, NPath currentEditorConfig,
            out bool isRoot)
        {
            isRoot = false;

            var lines = currentEditorConfig.ReadAllLines();
            int searchBottom = lines.Length - 1;

            do
            {
                var entryMatch = FindMatchingEntryBottomUp(lines, searchBottom, key);
                if (!entryMatch.IsMatch)
                    break;

                var headingMatch = FindSectionHeading(lines, entryMatch.matchIndex - 1);
                if (!headingMatch.IsMatch)
                    break;

                //Try matching on the filename, the relative path, and the relative path with a leading '/'. A match on any is a match.
                //This allows '*.txt', 'dir/*.txt', and '/dir/*.txt' to match.
                if (MatchHeading(headingMatch.value, path, currentDirectory))
                    return entryMatch.value;

                searchBottom = headingMatch.matchIndex - 1;
            }
            while (searchBottom >= 0);

            if (searchBottom >= 0)
            {
                var rootMatch = FindMatchingEntryBottomUp(lines, searchBottom, "root");
                if (rootMatch.IsMatch && bool.TryParse(rootMatch.value, out bool rootValue))
                    isRoot = rootValue;
            }

            return null;
        }

        static readonly char[] k_UnsupportedHeadingCharacters = {'[', ']', '{', '}'};

        internal static bool MatchHeading(string heading, string path, NPath currentDirectory)
        {
            if (!IsSupportedHeading(heading))
            {
                var options = new EditorConfigMinimatcherOptions()
                {
                    //AllowWindowsPaths = true,
                    MatchBase = true,
                    Dot = true,
                    NoExt = true
                };
                var fixedGlob = FixGlobForEditorConfigMinimatcher(heading, currentDirectory);
                var minimatcher = new EditorConfigMinimatcher(fixedGlob, options);
                var fullFilename = currentDirectory.Combine(path);
                return minimatcher.IsMatch(fullFilename.ToString(SlashMode.Forward));
            }

            int headingStart = 0;
            bool negate = false;
            while (headingStart < heading.Length && heading[headingStart] == '!')
            {
                negate = !negate;
                headingStart++;
            }

            var firstSlashIndex = heading.IndexOf('/');
            //skip initial '/', which is redundant
            if (firstSlashIndex == headingStart)
                headingStart++;

            if (headingStart == heading.Length)
                return false;

            if (firstSlashIndex == -1)
            {
                //If there is no '/' we allow a match on just the filename
                heading = "**/" + heading.Substring(headingStart);
                headingStart = 0;
            }

            var isMatch = MatchHeading(heading, headingStart, path, 0);

            if (negate)
                isMatch = !isMatch;

            return isMatch;
        }

        static string FixGlobForEditorConfigMinimatcher(string glob, string directory)
        {
            switch (glob.IndexOf('/'))
            {
                case -1:
                    glob = "**/" + glob;
                    break;
                case 0:
                    glob = glob.Substring(1);
                    break;
            }

            glob = Regex.Replace(glob, @"\*\*", "{*,**/**/**}");

            directory = directory.Replace(@"\", "/");
            if (!directory.EndsWith("/")) directory += "/";

            return directory + glob;
        }

        static bool IsSupportedHeading(string heading)
        {
            for (int i = 0; i < heading.Length; i++)
            {
                var ch = heading[i];

                foreach (var unsupportedHeadingCharacter in k_UnsupportedHeadingCharacters)
                {
                    if (ch == unsupportedHeadingCharacter)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        static bool MatchHeading(string heading, int headingStart, string path, int pathStart)
        {
            var headingIdx = headingStart;
            if (headingStart == heading.Length)
            {
                return pathStart == path.Length;
            }
            else if (heading[headingStart] == '*')
            {
                headingIdx++;
                bool isDirectoryAgnosticWildcard = false;
                while (headingIdx < heading.Length && heading[headingIdx] == '*')
                {
                    isDirectoryAgnosticWildcard = true;
                    headingIdx++;
                }

                // **/a and a/**/b need to match 'a/b/c'
                bool isDirectoryAgnosticSection = false;
                if (isDirectoryAgnosticWildcard && headingIdx != heading.Length && heading[headingIdx] == '/')
                {
                    isDirectoryAgnosticSection = true;
                    headingIdx++;
                }

                var pathIdx = pathStart;
                while (pathIdx < path.Length)
                {
                    if (MatchHeading(heading, headingIdx, path, pathIdx))
                        return true;
                    else if (!isDirectoryAgnosticWildcard && IsPathSeparator(path[pathIdx]))
                        return false; // the wildcard should not match the '/', so we failed the match

                    if (isDirectoryAgnosticSection)
                    {
                        // Skip to the first character after the next path separator
                        while (pathIdx < path.Length && !IsPathSeparator(path[pathIdx]))
                        {
                            pathIdx++;
                        }

                        pathIdx++;
                    }
                    else
                        pathIdx++;
                }

                //The wildcard has consumed the remainder of the path. It is a match if the wildcard is also the end of the heading
                return headingIdx == heading.Length;
            }

            return MatchSequenceStartingWithNonWildcard(heading, headingIdx, path, pathStart);
        }

        static bool MatchSequenceStartingWithNonWildcard(string heading, int headingIdx, string path,
            int pathIdx)
        {
            var seqHeadingIdx = headingIdx;
            var seqPathIdx = pathIdx;
            for (; seqHeadingIdx < heading.Length; seqHeadingIdx++, seqPathIdx++)
            {
                if (seqHeadingIdx == heading.Length || heading[seqHeadingIdx] == '*')
                {
                    return MatchHeading(heading, seqHeadingIdx, path, seqPathIdx);
                }

                var headingChar = heading[seqHeadingIdx];

                if (seqPathIdx == path.Length)
                {
                    //no more path left to match. Match failed
                    return false;
                }

                var pathChar = path[seqPathIdx];

                bool isMatch;
                switch (headingChar)
                {
                    case '?':
                        //https://editorconfig.org/#file-format-details indicates that '?' "Matches any single character",
                        //but in EditorConfig.Core '?' does not match path separators. We keep consistency with EditorConfig.Core
                        isMatch = !IsPathSeparator(pathChar);
                        break;
                    case '/':
                        if (seqHeadingIdx > 0 && heading[seqHeadingIdx - 1] == '/')
                        {
                            //skip repeated '/'. dir1/dir2 is equivalent to dir1////dir2
                            isMatch = true;
                            seqPathIdx--;
                        }
                        else
                            isMatch = IsPathSeparator(pathChar);

                        break;
                    case '[':
                    case '{':
                        throw new NotSupportedException(
                            "[] is not supported. Should have fallen back to EditorConfigMinimatcher");
                    default:
                        isMatch = IsLiteralMatch(heading, ref seqHeadingIdx, pathChar);
                        break;
                }

                if (!isMatch)
                    return false;
            }

            //Got to the end of the heading. Hit the base case
            return MatchHeading(heading, seqHeadingIdx, path, seqPathIdx);
        }

        static bool IsLiteralMatch(string heading, ref int headingIdx, char pathChar)
        {
            if (heading[headingIdx] == '\\')
            {
                if (headingIdx < heading.Length - 1)
                {
                    bool isMatch = IsLiteralMatch(heading[headingIdx + 1], pathChar);
                    headingIdx++;
                    return isMatch;
                }
                else
                    return IsLiteralMatch('\\', pathChar); //trailing '\' matches on '\'
            }
            else
                return IsLiteralMatch(heading[headingIdx], pathChar);
        }

        static bool IsPathSeparator(char pathChar)
        {
            return pathChar == '/' || pathChar == '\\';
        }

        static bool IsLiteralMatch(char char1, char char2)
        {
            return char.ToLowerInvariant(char1) ==
                char.ToLowerInvariant(char2);
        }

        static Match FindSectionHeading(string[] lines, int searchBottom)
        {
            int li = searchBottom;
            for (; li >= 0; li--)
            {
                var line = lines[li];
                int charIndex = SkipSpace(0, line);

                if (charIndex >= line.Length || line[charIndex] != '[')
                    continue;

                int endCharIndex = ReverseSkipSpace(line.Length - 1, line);
                if (endCharIndex < 0 || line[endCharIndex] != ']')
                    continue;
                if (charIndex + 1 > endCharIndex - 1) // empty braces []
                    return new Match(li, string.Empty);

                return new Match(li, line.Substring(charIndex + 1, endCharIndex - charIndex - 1));
            }

            return Match.nil;
        }

        struct Match
        {
            public readonly int matchIndex;
            public readonly string value;

            public static Match nil = new Match(-1, null);

            public bool IsMatch => matchIndex >= 0;

            public Match(int matchIndex, string value)
            {
                this.matchIndex = matchIndex;
                this.value = value;
            }
        }

        static Match FindMatchingEntryBottomUp(string[] lines, int lastIndex, string key)
        {
            int li = lastIndex;
            for (; li >= 0; li--)
            {
                var line = lines[li];
                int startIndex = SkipSpace(0, line);
                if (String.Compare(line, startIndex, key, 0, key.Length, StringComparison.OrdinalIgnoreCase) != 0)
                    continue;

                //find '=' position
                int eqIndex = startIndex + key.Length;
                eqIndex = SkipSpace(eqIndex, line);
                if (eqIndex >= line.Length || line[eqIndex] != '=')
                    continue;

                //Get value
                int valueStartIndex = SkipSpace(eqIndex + 1, line);
                int valueEndIndex = ReverseSkipSpace(line.Length - 1, line);

                string value;
                if (valueStartIndex > valueEndIndex || valueEndIndex >= line.Length)
                    value = string.Empty;
                else
                    value = line.Substring(valueStartIndex, valueEndIndex - valueStartIndex + 1);

                return new Match(li, value);
            }

            return Match.nil;
        }

        static int SkipSpace(int index, string st)
        {
            while (index < st.Length && (st[index] == ' ' || st[index] == '\t'))
                index++;

            return index;
        }

        static int ReverseSkipSpace(int endIndex, string st)
        {
            while (endIndex >= 0 && (st[endIndex] == ' ' || st[endIndex] == '\t'))
                endIndex--;

            return endIndex;
        }
    }
}
