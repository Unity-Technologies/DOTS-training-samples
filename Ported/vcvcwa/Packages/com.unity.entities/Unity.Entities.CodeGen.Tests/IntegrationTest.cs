using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Mono.Cecil;
using NUnit.Framework;

namespace Unity.Entities.CodeGen.Tests
{
    [TestFixture]
    public abstract class IntegrationTest : PostProcessorTestBase
    {
        // Make sure to not check this in with true or your tests will always pass!
        public static bool overwriteExpectationWithReality = false;

        protected abstract string ExpectedPath { get; }
        protected virtual string AdditionalIL { get { return string.Empty; } }

        protected void RunTest(TypeReference type)
        {
            var expectationFile = Path.GetFullPath($"{ExpectedPath}/{GetType().Name}.expectation.txt");

            var jobCSharp = Decompiler.DecompileIntoString(type);
            var jobCSharpLines = jobCSharp.Split('\n');

            var shouldOverWrite = overwriteExpectationWithReality || !File.Exists(expectationFile);

            if (shouldOverWrite)
            {
                File.WriteAllText(expectationFile, jobCSharp);
            }
            string expected = File.ReadAllText(expectationFile);
            var expectedLines = expected.Split('\n');

            var attributeRegex = new Regex(@"^[\t, ]*\[[\w]+\][\s]*$");
            var actualAttributes = new List<string>();
            var expectedAttributes = new List<string>();

            bool success = true;
            int minLines = Math.Min(expectedLines.Length, jobCSharpLines.Length);
            for (int i = 0; i < minLines; ++i)
            {
                string actualLine = jobCSharpLines[i];
                string expectedLine = expectedLines[i];

                if (attributeRegex.IsMatch(actualLine))
                {
                    actualAttributes.Add(actualLine);
                    expectedAttributes.Add(expectedLine);
                    continue;
                }

                if (expectedLine != actualLine)
                {
                    success = false;
                    break;
                }
            }

            actualAttributes.Sort();
            expectedAttributes.Sort();
            success &= expectedAttributes.SequenceEqual(actualAttributes);

            if (!success || overwriteExpectationWithReality)
            {
                var tempFolder = Path.GetTempPath();
                var path = $@"{tempFolder}decompiled.cs";
                File.WriteAllText(path, jobCSharp + Environment.NewLine + Environment.NewLine + AdditionalIL);
                Console.WriteLine("Actual Decompiled C#: ");
                Console.WriteLine((string)jobCSharp);
                if (!String.IsNullOrEmpty(AdditionalIL))
                {
                    Console.WriteLine("Addition IL: ");
                    Console.WriteLine(AdditionalIL);
                }
                UnityEngine.Debug.Log($"Wrote expected csharp to editor log and to {path}");
            }

            if (shouldOverWrite)
                return;

            Assert.IsTrue(success);
        }
    }
}
