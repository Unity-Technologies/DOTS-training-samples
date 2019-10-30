using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

[assembly: InternalsVisibleTo("UnityEditor.CacheServerTests")]

namespace UnityEditor.Build.CacheServer
{
    internal static class Util
    {
        private const string IpAddressKey = "CacheServerIPAddress";
        
        private static int ReverseByte(int b)
        {
            return ((b & 0x0F) << 4) | ((b >> 4) & 0x0F);
        }

        private static byte[] StringToByteArray(string input, bool asGuid)
        {
            var bytes = new byte[input.Length / 2];
            for (var i = 0; i < input.Length; i += 2)
            {
                var b = Convert.ToByte(input.Substring(i, 2), 16);
                bytes[i / 2] = asGuid ? (byte) ReverseByte(b) : b;
            }

            return bytes;
        }

        /// <summary>
        /// Convert a hex string to a byte array that represents an Asset Hash
        /// </summary>
        /// <param name="hashStr">32 character hex string</param>
        /// <returns>byte array</returns>
        public static byte[] StringToHash(string hashStr)
        {
            Debug.Assert(hashStr.Length == 32);
            return StringToByteArray(hashStr, false);
        }

        /// <summary>
        /// Convert a hex string to a byte array that represents an Asset GUID
        /// </summary>
        /// <param name="guidStr">32 character hex string</param>
        /// <returns>byte array</returns>
        public static byte[] StringToGuid(string guidStr)
        {
            Debug.Assert(guidStr.Length == 32);
            return StringToByteArray(guidStr, true);
        }
        
        /// <summary>
        /// Parse an ascii byte array at <paramref name="index"/>start as an int value
        /// </summary>
        /// <param name="bytes">byte array</param>
        /// <param name="index">offset</param>
        /// <returns></returns>
        public static int ReadUInt32(byte[] bytes, int index)
        {
            Debug.Assert(bytes.Length + index >= 8);
            return Int32.Parse(Encoding.ASCII.GetString(bytes, index, 8), NumberStyles.HexNumber);
        }

        /// <summary>
        /// Encode an integer as an ascii byte array
        /// </summary>
        /// <param name="input">integer</param>
        /// <param name="minLength">true ensure the byte array is as short as possible; false to pad to 8 bytes</param>
        /// <returns></returns>
        public static byte[] EncodeInt32(int input, bool minLength = false)
        {
            return Encoding.ASCII.GetBytes(input.ToString(minLength ? "X" : "X8"));
        }
        
        /// <summary>
        /// Parse a subset of an ascii byte array as a long value
        /// </summary>
        /// <param name="bytes">byte array</param>
        /// <param name="index">offset within <paramref name="bytes"/> to read from</param>
        /// <returns></returns>
        public static long ReadUInt64(byte[] bytes, int index)
        {
            Debug.Assert(bytes.Length + index >= 16);
            return Int64.Parse(Encoding.ASCII.GetString(bytes, index, 16), NumberStyles.HexNumber);
        }

        /// <summary>
        /// Encode a long value into an ascii byte array
        /// </summary>
        /// <param name="input">long value</param>
        /// <returns></returns>
        public static byte[] EncodeInt64(long input)
        {
            return Encoding.ASCII.GetBytes(input.ToString("X16"));
        }

        /// <summary>
        /// Compare two byte arrays for value equality
        /// </summary>
        /// <param name="ar1">first array</param>
        /// <param name="ar2">second array</param>
        /// <returns></returns>
        public static bool ByteArraysAreEqual(byte[] ar1, byte[] ar2)
        {
            return ar1.Length == ar2.Length && ByteArraysAreEqual(ar1, 0, ar2, 0, ar1.Length);
        }
        
        /// <summary>
        /// Compare two byte arrays for value equality at specific offsets and length
        /// </summary>
        /// <param name="ar1">first array</param>
        /// <param name="start1">offset within first array</param>
        /// <param name="ar2">second array</param>
        /// <param name="start2">offset within second array</param>
        /// <param name="count">number of bytes to compare</param>
        /// <returns></returns>
        public static bool ByteArraysAreEqual(byte[] ar1, int start1, byte[] ar2, int start2, int count)
        {
            Debug.Assert(start1 >= 0 && start2 >= 0 && count >= 0);
            if(start1 + count > ar1.Length)
                return false;

            if (start2 + count > ar2.Length)
                return false;
            
            for (var i = 0; i < count; i++)
                if (ar1[start1 + i] != ar2[start2 + i])
                    return false;

            return true;
        }

        /// <summary>
        /// Retrieve the configured cache server address for the Unity Editor
        /// </summary>
        public static string ConfigCacheServerAddress
        {
            get { return EditorPrefs.GetString(IpAddressKey); }
        }

        /// <summary>
        /// Parse an address string in the format of 'address:port' to a string address and integer port number
        /// </summary>
        /// <param name="address">combined address string</param>
        /// <param name="host">address part</param>
        /// <param name="port">port part</param>
        public static void ParseCacheServerIpAddress(string address, out string host, out int port)
        {
            host = null;
            port = 8126;
            
            var parts = address.Split(':');
            
            if(parts.Length > 0)
                host = parts[0];

            if (parts.Length > 1)
                port = int.Parse(parts[1]);
        }
    }
}