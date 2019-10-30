using System.IO;
using NUnit.Framework;
using UnityEditor;

namespace Unity.Build.Common.Tests
{
    [TestFixture]
    class TemporaryFileTrackerTests
    {
        string m_TestDirectory;

        [SetUp]
        public void Setup()
        {
            m_TestDirectory = "Assets/" + Path.GetRandomFileName();
            Directory.CreateDirectory(m_TestDirectory);
        }

        [TearDown]
        public void Teardown()
        {
            FileUtil.DeleteFileOrDirectory(m_TestDirectory);
            FileUtil.DeleteFileOrDirectory(m_TestDirectory + ".meta");
        }

        [Test]
        public void Should_Create_And_Delete_Directory()
        {
            using (var trackerA = new TemporaryFileTracker())
            {
                trackerA.CreateDirectory(m_TestDirectory + "/Parent/Child");
                using (var trackerB = new TemporaryFileTracker())
                {
                    File.Create(trackerB.TrackFile(m_TestDirectory + "/Parent/Child/File.txt")).Dispose();
                    File.Create(trackerB.TrackFile(m_TestDirectory + "/Parent/File.txt")).Dispose();

                    DirectoryAssert.Exists(m_TestDirectory + "/Parent/Child");
                    FileAssert.Exists(m_TestDirectory + "/Parent/Child/File.txt");
                    FileAssert.Exists(m_TestDirectory + "/Parent/File.txt");
                }

                FileAssert.DoesNotExist(m_TestDirectory + "/Parent/Child/File.txt");
                FileAssert.DoesNotExist(m_TestDirectory + "/Parent/File.txt");
                DirectoryAssert.Exists(m_TestDirectory + "/Parent/Child");
            }

            DirectoryAssert.DoesNotExist(m_TestDirectory + "/Parent/Child");
            DirectoryAssert.DoesNotExist(m_TestDirectory + "/Parent");
            DirectoryAssert.Exists(m_TestDirectory);
        }

        [Test]
        public void Should_Delete_Existing_File()
        {
            var existingFile = m_TestDirectory + "/Parent/Existing.txt";
            var newFile = m_TestDirectory + "/Parent/File.txt";

            Directory.CreateDirectory(m_TestDirectory + "/Parent");
            File.Create(existingFile).Dispose();

            FileAssert.Exists(existingFile);

            using (var tracker = new TemporaryFileTracker())
            {
                tracker.TrackFile(existingFile, ensureDoesntExist: false);
                File.Create(tracker.TrackFile(newFile)).Dispose();

                FileAssert.Exists(existingFile);
                FileAssert.Exists(newFile);
            }

            FileAssert.DoesNotExist(existingFile);
            FileAssert.DoesNotExist(newFile);
        }

        [Test]
        public void Should_Delete_File_When_Starting_To_Track()
        {
            var existingFile = m_TestDirectory + "/Parent/Existing.txt";
            Directory.CreateDirectory(m_TestDirectory + "/Parent");
            File.WriteAllText(existingFile, "hello world");

            using (var tracker = new TemporaryFileTracker())
            {
                tracker.TrackFile(existingFile);
                FileAssert.DoesNotExist(existingFile);
            }
        }
    }
}