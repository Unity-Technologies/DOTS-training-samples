using System;
using JetBrains.Annotations;

namespace Unity.Coding.Utils
{
    public static class StringUtility
    {
        /// <summary>
        /// Scans `text` for EOL sequences (\n, \r\n) and returns the most common seen. Old style mac sequences (plain \r) are not supported and instead treated as unix (plain \n).
        /// </summary>
        /// <param name="text">The text to scan for EOL's.</param>
        /// <param name="stopAfterWindow">Once any of the candidate EOL types is "winning" by this amount, stop and return it. Set to 0 to force a scan of all of `text`.</param>
        /// <returns>Either "\n" (unix), or "\r\n" (dos/windows), whichever is most common. If no newlines found, returns the environment default.</returns>
        [NotNull]
        public static string DetectEolType([NotNull] string text, int stopAfterWindow = 50)
        {
            int crlfCount = 0, lfCount = 0;
            for (var i = 0; i < text.Length; ++i)
            {
                if (text[i] == '\r')
                {
                    if (i < text.Length - 1 && text[i + 1] == '\n')
                    {
                        ++crlfCount;
                        ++i;
                    }
                    else
                    {
                        // old mac style, treat as unix
                        ++lfCount;
                    }
                }
                else if (text[i] == '\n')
                {
                    ++lfCount;
                }

                if (stopAfterWindow > 0 && Math.Abs(crlfCount - lfCount) >= stopAfterWindow)
                    break;
            }

            if (crlfCount > lfCount)
                return "\r\n";
            if (lfCount > crlfCount)
                return "\n";

            // it's either a tie, or we didn't find any. in that case, go with whatever the os wants.
            return Environment.NewLine;
        }
    }
}
