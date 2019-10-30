using System;
using System.Collections;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using UnityEngine;

namespace UnityEditor.Build.CacheServer
{
    /// <summary>
    /// The type of a particular file.
    /// </summary>
    public enum FileType
    {
        Asset = 'a',
        Info = 'i',
        Resource = 'r'
    }

    /// <summary>
    /// The result returned by a download operation.
    /// </summary>
    public enum DownloadResult
    {
        Failure = 0,
        FileNotFound = 1,
        Success = 2
    }
    
    /// <summary>
    /// A GUID/Hash pair that uniquely identifies a particular file. For each FileId, the Cache Server can store a separate
    /// binary stream for each FileType.
    /// </summary>
    public struct FileId : IEqualityComparer
    {
        /// <summary>
        /// The guid byte array.
        /// </summary>
        public readonly byte[] guid;

        /// <summary>
        /// The hash code byte array.
        /// </summary>
        public readonly byte[] hash;
        
        /// <summary>
        /// A structure used to identify a file by guid and hash code.
        /// </summary>
        /// <param name="guid">File GUID.</param>
        /// <param name="hash">File hash code.</param>
        private FileId(byte[] guid, byte[] hash)
        {
            this.guid = guid;
            this.hash = hash;
        }

        /// <summary>
        /// Create a FileId given a string guid and string hash code representation.
        /// </summary>
        /// <param name="guidStr">GUID string representation.</param>
        /// <param name="hashStr">Hash code string representation.</param>
        /// <returns></returns>
        public static FileId From(string guidStr, string hashStr)
        {
            if (guidStr.Length != 32)
                throw new ArgumentException("Length != 32", "guidStr");

            if (hashStr.Length != 32)
                throw new ArgumentException("Length != 32", "hashStr");
            
            return new FileId(Util.StringToGuid(guidStr), Util.StringToHash(hashStr));
        }

        /// <summary>
        /// Create a FileId given a byte array guid and byte array hash code.
        /// </summary>
        /// <param name="guid">GUID byte array.</param>
        /// <param name="hash">Hash code byte array.</param>
        /// <returns></returns>
        public static FileId From(byte[] guid, byte[] hash)
        {
            if (guid.Length != 16)
                throw new ArgumentException("Length != 32", "guid");

            if (hash.Length != 16)
                throw new ArgumentException("Length != 32", "hash");
            
            return new FileId(guid, hash);
        }

        /// <summary>
        /// Check equality of two objects given their guid and hash code.
        /// </summary>
        /// <param name="x">lhs object.</param>
        /// <param name="y">rhs object.</param>
        /// <returns></returns>
        public new bool Equals(object x, object y)
        {
            var hash1 = (byte[]) x;
            var hash2 = (byte[]) y;

            if (hash1.Length != hash2.Length)
                return false;

            for (var i = 0; i < hash1.Length; i++)
                if (hash1[i] != hash2[i])
                    return false;

            return true;
        }

        /// <summary>
        /// Get the hash code for a specific object.
        /// </summary>
        /// <param name="obj">The object you want the hash code for.</param>
        /// <returns></returns>
        public int GetHashCode(object obj)
        {
            var hc = 17;
            hc = hc * 23 + guid.GetHashCode();
            hc = hc * 23 + hash.GetHashCode();
            return hc;
        }
    }

    /// <summary>
    /// Exception thrown when an upload operation is not properly isolated within a begin/end transaction
    /// </summary>
    public class TransactionIsolationException : Exception
    {
        public TransactionIsolationException(string msg) : base(msg) { }
    }

    /// <summary>
    /// EventArgs passed to the DownloadFinished event handler
    /// </summary>
    public class DownloadFinishedEventArgs : EventArgs
    {
        /// <summary>
        /// EventArgs download result code.
        /// </summary>
        public DownloadResult Result { get; set; }

        /// <summary>
        /// The downloaded item.
        /// </summary>
        public IDownloadItem DownloadItem { get; set; }

        /// <summary>
        /// The size of the downloaded item.
        /// </summary>
        public long Size { get; set; }

        /// <summary>
        /// The length of the download queue.
        /// </summary>
        public long DownloadQueueLength { get; set; }
    }
    
    /// <summary>
    /// A client API for uploading and downloading files from a Cache Server
    /// </summary>
    public class Client
    {
        private enum StreamReadState
        {
            Response,
            Size,
            Id
        }
        
        private const int ProtocolVersion = 254;
        private const string CmdTrxBegin = "ts";
        private const string CmdTrxEnd = "te";
        private const string CmdGet = "g";
        private const string CmdPut = "p";
        private const string CmdQuit = "q";
        
        private const int ResponseLen = 2;
        private const int SizeLen = 16;
        private const int GuidLen = 16;
        private const int HashLen = 16;
        private const int IdLen = GuidLen + HashLen;
        private const int ReadBufferLen = 64 * 1024;

        private readonly Queue<IDownloadItem> m_downloadQueue;
        private readonly TcpClient m_tcpClient;
        private readonly string m_host;
        private readonly int m_port;
        private NetworkStream m_stream;
        private readonly byte[] m_streamReadBuffer;
        private int m_streamBytesRead;
        private int m_streamBytesNeeded;
        private StreamReadState m_streamReadState = StreamReadState.Response;
        private DownloadFinishedEventArgs m_nextFileCompleteEventArgs;
        private Stream m_nextWriteStream;
        private bool m_inTrx;
        
        /// <summary>
        /// Returns the number of items in the download queue
        /// </summary>
        public int DownloadQueueLength
        {
            get { return m_downloadQueue.Count; }
        }
        
        /// <summary>
        /// Event fired when a queued download request finishes.
        /// </summary>
        public event EventHandler<DownloadFinishedEventArgs> DownloadFinished;

        /// <summary>
        /// Remove all listeners from the DownloadFinished event
        /// </summary>
        public void ResetDownloadFinishedEventHandler()
        {
            DownloadFinished = null;
        }
        
        /// <summary>
        /// Create a new Cache Server client
        /// </summary>
        /// <param name="host">The host name or IP of the Cache Server.</param>
        /// <param name="port">The port number of the Cache Server. Default port is 8126.</param>
        public Client(string host, int port = 8126)
        {
            m_streamReadBuffer = new byte[ReadBufferLen];
            m_downloadQueue = new Queue<IDownloadItem>();
            m_tcpClient = new TcpClient();
            m_host = host;
            m_port = port;
        }
        
        /// <summary>
        /// Connects to the Cache Server and sends a protocol version handshake.
        /// </summary>
        /// <exception cref="Exception"></exception>
        public void Connect()
        {
            var client = m_tcpClient;
            client.Connect(m_host, m_port);
            m_stream = client.GetStream();
            SendVersion();
        }
        
        /// <summary>
        /// Connects to the Cache Server and sends a protocol version handshake. A TimeoutException is thrown if the connection cannot
        /// be established within <paramref name="timeoutMs"/> milliseconds.
        /// </summary>
        /// <param name="timeoutMs"></param>
        /// <exception cref="TimeoutException"></exception>
        /// <exception cref="Exception"></exception>
        public void Connect(int timeoutMs)
        {
            var client = m_tcpClient;
            var op = client.BeginConnect(m_host, m_port, null, null);
            
            var connected = op.AsyncWaitHandle.WaitOne(TimeSpan.FromMilliseconds(timeoutMs));
            
            if(!connected)
                throw new TimeoutException();
            
            m_stream = client.GetStream();
            SendVersion();
        }

        /// <summary>
        /// Begin an upload transaction for an asset. Transactions in process can be interupted by calling BeginTransaction
        /// again before calling EndTransaction.
        /// </summary>
        /// <param name="fileId"></param>
        public void BeginTransaction(FileId fileId)
        {
            m_inTrx = true;
            m_stream.Write(Encoding.ASCII.GetBytes(CmdTrxBegin), 0, 2);
            m_stream.Write(fileId.guid, 0, GuidLen);
            m_stream.Write(fileId.hash, 0, HashLen);
        }

        /// <summary>
        /// Upload from the given stream for the given FileType. Will throw an exception if not preceeded by BeginTransaction.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="readStream"></param>
        /// <exception cref="TransactionIsolationException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public void Upload(FileType type, Stream readStream)
        {
            if(!m_inTrx)
                throw new TransactionIsolationException("Upload without BeginTransaction");
            
            if(!readStream.CanRead || !readStream.CanSeek)
                throw new ArgumentException();

            m_stream.Write(Encoding.ASCII.GetBytes(CmdPut + (char) type), 0, 2);
            m_stream.Write(Util.EncodeInt64(readStream.Length), 0, SizeLen);

            var buf = new byte[ReadBufferLen];
            while (readStream.Position < readStream.Length - 1)
            {
                var len = readStream.Read(buf, 0, ReadBufferLen);
                m_stream.Write(buf, 0, len);
            }
        }
        
        /// <summary>
        /// Commit the uploaded files to the Cache Server. Will throw an exception if not preceeded by BeginTransaction.
        /// </summary>
        /// <exception cref="TransactionIsolationException"></exception>
        public void EndTransaction()
        {
            if(!m_inTrx)
                throw new TransactionIsolationException("EndTransaction without BeginTransaction");
            
            m_inTrx = false;
            m_stream.Write(Encoding.ASCII.GetBytes(CmdTrxEnd), 0, 2);
        }

        /// <summary>
        /// Send a download request to the Cache Server. Listen to the DownloadComplete event to read the results.
        /// </summary>
        /// <param name="downloadItem">The IDownloadItem that specifies which file to download</param>
        public void QueueDownload(IDownloadItem downloadItem)
        {
            m_stream.Write(Encoding.ASCII.GetBytes(CmdGet + (char) downloadItem.Type), 0, 2);
            m_stream.Write(downloadItem.Id.guid, 0, GuidLen);
            m_stream.Write(downloadItem.Id.hash, 0, HashLen);
            m_downloadQueue.Enqueue(downloadItem);
            
            if(m_downloadQueue.Count == 1)
                ReadNextDownloadResult();
        }
  
        /// <summary>
        /// Close the connection to the Cache Server. Sends the 'quit' command and closes the network stream.
        /// </summary>
        public void Close()
        {
            if(m_stream != null)
                m_stream.Write(Encoding.ASCII.GetBytes(CmdQuit), 0, 1);
            
            if(m_tcpClient != null)
                m_tcpClient.Close();
        }

        private void SendVersion()
        {
            var encodedVersion = Util.EncodeInt32(ProtocolVersion, true);
            m_stream.Write(encodedVersion, 0, encodedVersion.Length);

            var versionBuf = new byte[8];
            var pos = 0;
            while (pos < versionBuf.Length - 1)
            {
                pos += m_stream.Read(versionBuf, 0, versionBuf.Length);
            }

            if(Util.ReadUInt32(versionBuf, 0) != ProtocolVersion)
                throw new Exception("Server version mismatch");
        }
        
        private void OnDownloadFinished(DownloadFinishedEventArgs e)
        {
            m_downloadQueue.Dequeue();
            e.DownloadQueueLength = m_downloadQueue.Count;

            if (DownloadFinished != null)
                DownloadFinished(this, e);
            
            if(m_downloadQueue.Count > 0)
                ReadNextDownloadResult();
        }

        private void ReadNextDownloadResult()
        {
            m_streamReadState = StreamReadState.Response;
            m_streamBytesNeeded = ResponseLen;
            m_streamBytesRead = 0;
            m_nextFileCompleteEventArgs = new DownloadFinishedEventArgs { Result = DownloadResult.Failure };
            BeginReadHeader();
        }

        private void BeginReadHeader()
        {
            m_stream.BeginRead(m_streamReadBuffer,
                0,
                m_streamBytesNeeded - m_streamBytesRead,
                EndReadHeader,
                m_stream);
        }

        private void EndReadHeader(IAsyncResult r)
        {
            var bytesRead = m_stream.EndRead(r);
            if (bytesRead <= 0) return;
            
            m_streamBytesRead += bytesRead;
            
            if (m_streamBytesRead < m_streamBytesNeeded)
            {
                BeginReadHeader();
                return;
            }

            switch (m_streamReadState)
            {
                case StreamReadState.Response:
                    if (Convert.ToChar(m_streamReadBuffer[0]) == '+')
                    {
                        m_streamReadState = StreamReadState.Size;
                        m_streamBytesNeeded = SizeLen;
                    }
                    else
                    {
                        m_nextFileCompleteEventArgs.Result = DownloadResult.FileNotFound;
                        m_streamReadState = StreamReadState.Id;
                        m_streamBytesNeeded = IdLen;
                    }
                        
                    break;
                        
                case StreamReadState.Size:
                    m_nextFileCompleteEventArgs.Size = Util.ReadUInt64(m_streamReadBuffer, 0);
                    m_streamReadState = StreamReadState.Id;
                    m_streamBytesNeeded = IdLen;
                    break;
                        
                case StreamReadState.Id:
                    var next = m_downloadQueue.Peek();
                    m_nextFileCompleteEventArgs.DownloadItem = next;
                    
                    var match =
                        Util.ByteArraysAreEqual(next.Id.guid, 0, m_streamReadBuffer, 0, GuidLen) &&
                        Util.ByteArraysAreEqual(next.Id.hash, 0, m_streamReadBuffer, GuidLen, HashLen);

                    if (!match)
                    {
                        Close();
                        throw new InvalidDataException();
                    }

                    if (m_nextFileCompleteEventArgs.Result == DownloadResult.FileNotFound)
                    {
                        OnDownloadFinished(m_nextFileCompleteEventArgs);
                    }
                    else
                    {
                        var size = m_nextFileCompleteEventArgs.Size;
                        m_nextWriteStream = next.GetWriteStream(size);
                        m_streamBytesNeeded = (int) size;
                        m_streamBytesRead = 0;
                        BeginReadData();
                    }

                    return;
                
                default:
                    throw new ArgumentOutOfRangeException();
            }

            m_streamBytesRead = 0;
            BeginReadHeader();
        }

        private void BeginReadData()
        {
            var len = Math.Min(ReadBufferLen, m_streamBytesNeeded - m_streamBytesRead);
            m_stream.BeginRead(m_streamReadBuffer, 0, len, EndReadData, null);
        }

        private void EndReadData(IAsyncResult readResult)
        {
            var bytesRead = m_stream.EndRead(readResult);
            Debug.Assert(bytesRead > 0);
            m_streamBytesRead += bytesRead;

            var writeResult = m_nextWriteStream.BeginWrite(m_streamReadBuffer, 0, bytesRead, null, null);
            m_nextWriteStream.EndWrite(writeResult);

            if (m_streamBytesRead < m_streamBytesNeeded)
            {
                BeginReadData();
            }
            else
            {
                m_nextFileCompleteEventArgs.DownloadItem.Finish();
                m_nextFileCompleteEventArgs.Result = DownloadResult.Success;
                OnDownloadFinished(m_nextFileCompleteEventArgs);
            }
        }
    }
}