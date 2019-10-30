using System.IO;
using NUnit.Framework;
using UnityEditor.Build.Pipeline.Utilities;

namespace UnityEditor.Build.Pipeline.Tests
{
    [TestFixture]
    class FileCompressorTests
    {
        const string k_SourceDirectory = "Compressor";

        static readonly string[] k_SourceFiles =  {
            "/File1.json",
            "/Subdir/File2.json",
            "\\File3.json",
            "\\Subdir\\File4.json"
        };

        static string NormalizePath(string path)
        {
            return path.Replace("\\", "/");
        }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            foreach (string file in k_SourceFiles)
            {
                var filePath = NormalizePath(k_SourceDirectory + file);
                var dir = Path.GetDirectoryName(filePath);
                Directory.CreateDirectory(dir);
                File.WriteAllText(filePath, filePath);
            }
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            Directory.Delete(k_SourceDirectory, true);
        }

        [Test]
        public void CompressAndDecompressCanHandleSubdirectories()
        {
            var targetDirectory = k_SourceDirectory + "2";
            var success = FileCompressor.Compress(k_SourceDirectory, "artifacts.sbpGz");
            Assert.IsTrue(success);

            success = FileCompressor.Decompress("artifacts.sbpGz", targetDirectory);
            Assert.IsTrue(success);

            for (int i = 0; i < k_SourceFiles.Length; i++)
            {
                var sourcePath = NormalizePath(k_SourceDirectory + k_SourceFiles[i]);
                var targetPath = NormalizePath(targetDirectory + k_SourceFiles[i]);
                FileAssert.Exists(targetPath);
                FileAssert.AreEqual(sourcePath, targetPath);
            }

            File.Delete("artifacts.sbpGz");
            Directory.Delete(targetDirectory, true);
        }

        [Test]
        public void TrailingSlashDoesNotChangeResults()
        {
            var success = FileCompressor.Compress(k_SourceDirectory, "artifacts1.sbpGz");
            Assert.IsTrue(success);
            
            success = FileCompressor.Compress(k_SourceDirectory + "/", "artifacts2.sbpGz");
            Assert.IsTrue(success);

            FileAssert.AreEqual("artifacts1.sbpGz", "artifacts2.sbpGz");

            File.Delete("artifacts1.sbpGz");
            File.Delete("artifacts2.sbpGz");
        }
    }
}