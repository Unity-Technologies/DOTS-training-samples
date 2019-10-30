using System;
using System.IO;
using System.Threading;
using NUnit.Framework;
using UnityEditor.Build.CacheServer;
using UnityEngine;
using Random = System.Random;

namespace UnityEditor.CacheServerTests
{
    [TestFixture]
    public class Tests
    {
        private const string KTestHost = "127.0.0.1";
        private const string KInvalidTestHost = "192.0.2.1";
        private Random rand;

        private static int TestPort
        {
            get { return LocalCacheServer.Port; }
        }

        private FileId GenerateFileId()
        {
            if(rand == null)
                rand = new Random();
            var guid = new byte[16];
            var hash = new byte[16];
            rand.NextBytes(guid);
            rand.NextBytes(hash);
            return FileId.From(guid, hash);
        }

        [OneTimeSetUp]
        public void BeforeAll()
        {
            var cachePath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            LocalCacheServer.Setup(1024 * 1024, cachePath);
            rand = new Random();
        }

        [OneTimeTearDown]
        public void AfterAll()
        {
            LocalCacheServer.Clear();
        }
        
        [Test]
        public void FileDownloadItem()
        {
            var fileId = GenerateFileId();
            var readStream = new ByteArrayStream(128 * 1024);
        
            var client = new Client(KTestHost, TestPort);
            client.Connect();

            client.BeginTransaction(fileId);
            client.Upload(FileType.Asset, readStream);
            client.EndTransaction();
            Thread.Sleep(50); // give the server a little time to finish the transaction

            var targetFile = Path.GetTempFileName();
            var downloadItem = new FileDownloadItem(fileId, FileType.Asset, targetFile);

            var mre = new ManualResetEvent(false);
            Exception err = null;
            client.DownloadFinished += (sender, args) =>
            {
                try
                {
                    Assert.AreEqual(DownloadResult.Success, args.Result);
                    Assert.AreEqual(args.DownloadItem.Id, fileId);
                    Assert.IsTrue(File.Exists(targetFile));
                
                    var fileBytes = File.ReadAllBytes(targetFile);
                    Assert.IsTrue(Util.ByteArraysAreEqual(readStream.BackingBuffer, fileBytes));
                }
                catch (Exception e)
                {
                    err = e;
                }
                finally
                {
                    if(File.Exists(targetFile))
                        File.Delete(targetFile);
                
                    mre.Set();
                }
            };

            client.QueueDownload(downloadItem);
            Assert.IsTrue(mre.WaitOne(2000));
        
            if (err != null)
                throw err;
        }
    
        [Test]
        public void Connect()
        {
            var client = new Client(KTestHost, TestPort);
            client.Connect();
            client.Close();
        }
    
        [Test]
        public void ConnectTimeout()
        {
            var client = new Client(KInvalidTestHost, TestPort);
            TimeoutException err = null;
            try
            {
                client.Connect(0);
            }
            catch (TimeoutException e)
            {
                err = e;
            }
            finally
            {
                client.Close();
                Debug.Assert(err != null);
            }
        }

        [Test]
        public void TransactionIsolation()
        {
            var fileId = GenerateFileId();
            var readStream = new ByteArrayStream(16 * 1024);
        
            var client = new Client(KTestHost, TestPort);
            client.Connect();

            Assert.Throws<TransactionIsolationException>(() => client.Upload(FileType.Asset, readStream));
            Assert.Throws<TransactionIsolationException>(() => client.EndTransaction());

            // Back-to-back begin transactions are allowed
            client.BeginTransaction(fileId);
            Assert.DoesNotThrow(() => client.BeginTransaction(fileId));
        }
        
        [Test]
        public void UploadDownloadOne()
        {
            var fileId = GenerateFileId();
            var readStream = new ByteArrayStream(16 * 1024);
        
            var client = new Client(KTestHost, TestPort);
            client.Connect();
        
            client.BeginTransaction(fileId);
            client.Upload(FileType.Asset, readStream);
            client.EndTransaction();
            Thread.Sleep(50); // give the server a little time to finish the transaction

            var downloadItem = new TestDownloadItem(fileId, FileType.Asset);
        
            client.QueueDownload(downloadItem);

            Exception err = null;
            var mre = new ManualResetEvent(false);
            client.DownloadFinished += (sender, args) =>
            {
                try
                {
                    Assert.AreEqual(0, args.DownloadQueueLength);
                    Assert.AreEqual(DownloadResult.Success, args.Result);
                    Assert.AreEqual(fileId, args.DownloadItem.Id);
                }
                catch (Exception e)
                {
                    err = e;
                }
                finally
                {
                    mre.Set();
                }
            };

            Assert.IsTrue(mre.WaitOne(2000));
        
            if (err != null)
                throw err;
        
            Assert.IsTrue(Util.ByteArraysAreEqual(readStream.BackingBuffer, downloadItem.Bytes));
        }

        [Test]
        public void DownloadMany()
        {
            const int fileCount = 5;
        
            var fileIds = new FileId[fileCount];
            var fileStreams = new ByteArrayStream[fileCount];
        
            var client = new Client(KTestHost, TestPort);
            client.Connect();
        
            // Upload files
            var rand = new Random();
            for (var i = 0; i < fileCount; i++)
            {
                fileIds[i] = GenerateFileId();
                fileStreams[i] = new ByteArrayStream(rand.Next(64 * 1024, 128 * 1024));

                client.BeginTransaction(fileIds[i]);
                client.Upload(FileType.Asset, fileStreams[i]);
                client.EndTransaction();
            }
        
            Thread.Sleep(50);
        
            // Download
            var receivedCount = 0;
            Exception err = null;
            var mre = new ManualResetEvent(false);
            client.DownloadFinished += (sender, args) =>
            {
                try
                {
                    Assert.AreEqual(args.Result, DownloadResult.Success);
                    Assert.AreEqual(args.DownloadItem.Id, fileIds[receivedCount]);

                    var downloadItem = (TestDownloadItem) args.DownloadItem;
                    Assert.IsTrue(Util.ByteArraysAreEqual(fileStreams[receivedCount].BackingBuffer, downloadItem.Bytes));
               
                    receivedCount++;
                    Assert.AreEqual(fileCount - receivedCount, args.DownloadQueueLength);
                }
                catch (Exception e)
                {
                    err = e;
                }
                finally
                {
                    if(err != null || receivedCount == fileCount)
                        mre.Set();
                }
            };

            for (var i = 0; i < fileCount; i++)
                client.QueueDownload(new TestDownloadItem(fileIds[i], FileType.Asset));

            Assert.AreEqual(fileCount, client.DownloadQueueLength);

            Assert.IsTrue(mre.WaitOne(2000));
        
            if (err != null)
                throw err;

            Assert.AreEqual(fileCount, receivedCount);
        }
    
        [Test]
        public void DonwloadFileNotFound()
        {
            var client = new Client(KTestHost, TestPort);
            client.Connect();
        
            var fileId = FileId.From(new byte[16], new byte[16]);
        
            var mre = new ManualResetEvent(false);
            var downloadItem = new TestDownloadItem(fileId, FileType.Asset);
        
            client.QueueDownload(downloadItem);
        
            Exception err = null;
        
            client.DownloadFinished += (sender, args) =>
            {
                try
                {
                    Assert.AreEqual(args.Result, DownloadResult.FileNotFound);
                    Assert.AreEqual(args.DownloadItem.Id, fileId);
                }
                catch (Exception e)
                {
                    err = e;
                }
                finally
                {
                    mre.Set();
                }
            };

            mre.WaitOne(500);
        
            if (err != null)
                throw err;
        }

        [Test]
        public void ResetDownloadFinishedEventHandler()
        {
            var fileId = GenerateFileId();
            var readStream = new ByteArrayStream(16 * 1024);
        
            var client = new Client(KTestHost, TestPort);
            client.Connect();
        
            client.BeginTransaction(fileId);
            client.Upload(FileType.Asset, readStream);
            client.EndTransaction();
            Thread.Sleep(50);

            var downloadItem = new TestDownloadItem(fileId, FileType.Asset);

            // Add two listeners that will assert if called
            client.DownloadFinished += (sender, args) => { Debug.Assert(false); };
            client.DownloadFinished += (sender, args) => { Debug.Assert(false); };
        
            // Clear the listeners so they will not be called
            client.ResetDownloadFinishedEventHandler();
        
            client.QueueDownload(downloadItem);
        
            var mre = new ManualResetEvent(false);
            ThreadPool.QueueUserWorkItem(state =>
            {
                while (client.DownloadQueueLength > 0)
                    Thread.Sleep(0);
            
                mre.Set();
            });
        
            Assert.IsTrue(mre.WaitOne(2000));
        }
    }
}
