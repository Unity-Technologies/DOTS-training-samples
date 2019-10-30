using System;
using System.Collections.Generic;

namespace Unity.Coding.Utils
{
    public static class Stdin
    {
        public static IEnumerable<string> SelectLines()
        {
            for (;;)
            {
                var line = Console.ReadLine();
                if (line == null)
                    yield break;

                yield return line;
            }
        }
    }
}
