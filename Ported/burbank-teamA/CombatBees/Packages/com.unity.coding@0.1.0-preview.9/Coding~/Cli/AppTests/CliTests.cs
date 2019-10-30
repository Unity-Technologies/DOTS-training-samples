using System.Linq;
using NUnit.Framework;
using Shouldly;
using Unity.Coding.Cli;
using Unity.Coding.Tests;
using Unity.Coding.Utils;

namespace CliApp
{
    class CliTests : TestFileSystemFixture
    {
        static StringLogger Execute(params string[] args)
        {
            var packageDir = TestContext
                .CurrentContext.TestDirectory.ToNPath()
                .ParentContaining(@"Packages\com.unity.coding", true)
                .DirectoryMustExist()
                .ToString();

            var appArgs = args.ToList();
            appArgs.Add("--package-root");
            appArgs.Add(packageDir);

            var logger = new StringLogger();
            App.Execute(appArgs, logger);
            return logger;
        }

        [Test]
        public void Format_WithoutFilesSpecified_ReturnsError() =>
            Execute("format")
                .ErrorsAsString
                .ShouldContain("Usage:");

        [Test]
        public void Format_WithNonexistentFile_ReturnsError() =>
            Execute("format", "does_not_exist.cs")
                .ErrorsAsString
                .ShouldContain("Could not find file");

        [Test]
        public void Format_WithFolder_ReturnsError() =>
            Execute("format", ".")
                .ErrorsAsString
                .ShouldContain("Found directory");

        const string k_UnformattedSource = "\t    class C {\n  };\n";
        const string k_FormattedSource = "class C\n{\n};\n";

        [Test]
        public void Format_WithoutPackageRootSpecified_DoesNotThrow()
        {
            var logger = new StringLogger();
            App.Execute(new[] { "format", BaseDir.Combine("file").WriteAllText("") }, logger);

            logger.ErrorsAsString.ShouldBeEmpty();
            logger.InfosAsString.ShouldBeEmpty();
        }

        [Test]
        public void Format_WithNoEditorConfig_DoesNothing()
        {
            var path = BaseDir.Combine("file.txt").WriteAllText(k_UnformattedSource);

            var result = Execute("format", path);

            result.ErrorsAsString.ShouldBeEmpty();
            result.InfosAsString.ShouldBeEmpty();
            path.ReadAllText().ShouldBe(k_UnformattedSource);
        }

        [Test]
        public void Format_WithBadPackagePath_ReturnsError()
        {
            // minimum required to trigger need for coding package guts
            WriteRootEditorConfig("[*]", "formatters=uncrustify");

            var logger = new StringLogger();
            App.Execute(new[]
                {
                    "format", "--package-root", "does_not_exist",
                    BaseDir.Combine("file").WriteAllText("")
                }, logger);

            logger.ErrorsAsString.ShouldContain("Invalid package root");
        }

        [Test]
        public void Format_WithEditorConfig_FormatsFile()
        {
            WriteRootEditorConfig("[*]", "formatters=uncrustify,generic");
            var path = BaseDir.Combine("file.cs").WriteAllText(k_UnformattedSource);

            var result = Execute("format", path);

            result.ErrorsAsString.ShouldBeEmpty();
            result.InfosAsString.ShouldBeEmpty();
            path.ReadAllText().ShouldBe(k_FormattedSource);
        }

        [Test]
        public void Format_WithMultipleFilesIncludingOneBadPath_FormatsValidAndErrorsForBad()
        {
            WriteRootEditorConfig("[*]", "formatters=uncrustify,generic");

            var paths = new[]
            {
                BaseDir.Combine("file.cs").WriteAllText(k_UnformattedSource),
                BaseDir.Combine("file2.cs"),
                BaseDir.Combine("file3.cs").WriteAllText(k_UnformattedSource),
            };

            var result = Execute("format", paths[0], paths[1], paths[2]);

            result.Errors.Count.ShouldBe(1);
            result.ErrorsAsString.ShouldMatch(@"Could not find file.*file2\.cs");
            result.InfosAsString.ShouldBeEmpty();

            paths[0].ReadAllText().ShouldBe(k_FormattedSource);
            paths[2].ReadAllText().ShouldBe(k_FormattedSource);
        }
    }
}
