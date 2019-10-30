using System;
using System.Security.Cryptography;

/* Copyright (C) 1991-2, RSA Data Security, Inc. Created 1991. All
   rights reserved.

   License to copy and use this software is granted provided that it
   is identified as the "RSA Data Security, Inc. MD4 Message-Digest
   Algorithm" in all material mentioning or referencing this software
   or this function.

   License is also granted to make and use derivative works provided
   that such works are identified as "derived from the RSA Data
   Security, Inc. MD4 Message-Digest Algorithm" in all material
   mentioning or referencing the derived work.

   RSA Data Security, Inc. makes no representations concerning either
   the merchantability of this software or the suitability of this
   software for any particular purpose. It is provided "as is"
   without express or implied warranty of any kind.

   These notices must be retained in any copies of any part of this
   documentation and/or software.
 */

/* Converted to C# by Ryan Caltabiano for Unity Technologies */

namespace UnityEditor.Build.Pipeline.Utilities
{
    public sealed class MD4 : HashAlgorithm
    {
        uint[] m_Buffer;
        uint[] m_Block;
        uint m_Bytes;

        static readonly byte[] kPadding = {
            0x80, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
        };

        MD4()
        {
            Initialize();
        }

        public new static MD4 Create()
        {
            return new MD4();
        }

        public override void Initialize()
        {
            m_Buffer = new uint[]
            {
                0x67452301,
                0xefcdab89,
                0x98badcfe,
                0x10325476
            };

            m_Block = new uint[16];
            m_Bytes = 0;
        }

        protected override void HashCore(byte[] array, int ibStart, int cbSize)
        {
            if (array == null)
                throw new ArgumentNullException("array");

            for (var i = 0; i < cbSize; i++)
            {
                var b = array[ibStart + i];
                var c = m_Bytes & 63;
                var k = c >> 2;
                var s = (c & 3) << 3;
                m_Block[k] = (m_Block[k] & ~((uint)255 << (int)s)) | ((uint)b << (int)s);

                if (c == 63)
                    ProcessBlock();

                m_Bytes++;
            }
        }

        protected override byte[] HashFinal()
        {
            var bytes = BitConverter.GetBytes(m_Bytes << 3);

            var length = ((m_Bytes + 8) & 0x7fffffc0) + 56 - m_Bytes;
            HashCore(kPadding, 0, (int)length);
            HashCore(bytes, 0, 4);
            HashCore(kPadding, kPadding.Length - 4, 4);

            var output = new byte[16];
            output[0] = (byte)(m_Buffer[0] & 0xff);
            output[1] = (byte)((m_Buffer[0] >> 8) & 0xff);
            output[2] = (byte)((m_Buffer[0] >> 16) & 0xff);
            output[3] = (byte)((m_Buffer[0] >> 24) & 0xff);
            output[4] = (byte)(m_Buffer[1] & 0xff);
            output[5] = (byte)((m_Buffer[1] >> 8) & 0xff);
            output[6] = (byte)((m_Buffer[1] >> 16) & 0xff);
            output[7] = (byte)((m_Buffer[1] >> 24) & 0xff);
            output[8] = (byte)(m_Buffer[2] & 0xff);
            output[9] = (byte)((m_Buffer[2] >> 8) & 0xff);
            output[10] = (byte)((m_Buffer[2] >> 16) & 0xff);
            output[11] = (byte)((m_Buffer[2] >> 24) & 0xff);
            output[12] = (byte)(m_Buffer[3] & 0xff);
            output[13] = (byte)((m_Buffer[3] >> 8) & 0xff);
            output[14] = (byte)((m_Buffer[3] >> 16) & 0xff);
            output[15] = (byte)((m_Buffer[3] >> 24) & 0xff);
            return output;
        }

        void ProcessBlock()
        {
            var buffer = new uint[4];
            Array.Copy(m_Buffer, buffer, 4);

            /* Round 1 */
            buffer[0] = RotateLeft(buffer[0] + F(buffer[1], buffer[2], buffer[3]) + m_Block[0], 3);
            buffer[3] = RotateLeft(buffer[3] + F(buffer[0], buffer[1], buffer[2]) + m_Block[1], 7);
            buffer[2] = RotateLeft(buffer[2] + F(buffer[3], buffer[0], buffer[1]) + m_Block[2], 11);
            buffer[1] = RotateLeft(buffer[1] + F(buffer[2], buffer[3], buffer[0]) + m_Block[3], 19);
            buffer[0] = RotateLeft(buffer[0] + F(buffer[1], buffer[2], buffer[3]) + m_Block[4], 3);
            buffer[3] = RotateLeft(buffer[3] + F(buffer[0], buffer[1], buffer[2]) + m_Block[5], 7);
            buffer[2] = RotateLeft(buffer[2] + F(buffer[3], buffer[0], buffer[1]) + m_Block[6], 11);
            buffer[1] = RotateLeft(buffer[1] + F(buffer[2], buffer[3], buffer[0]) + m_Block[7], 19);
            buffer[0] = RotateLeft(buffer[0] + F(buffer[1], buffer[2], buffer[3]) + m_Block[8], 3);
            buffer[3] = RotateLeft(buffer[3] + F(buffer[0], buffer[1], buffer[2]) + m_Block[9], 7);
            buffer[2] = RotateLeft(buffer[2] + F(buffer[3], buffer[0], buffer[1]) + m_Block[10], 11);
            buffer[1] = RotateLeft(buffer[1] + F(buffer[2], buffer[3], buffer[0]) + m_Block[11], 19);
            buffer[0] = RotateLeft(buffer[0] + F(buffer[1], buffer[2], buffer[3]) + m_Block[12], 3);
            buffer[3] = RotateLeft(buffer[3] + F(buffer[0], buffer[1], buffer[2]) + m_Block[13], 7);
            buffer[2] = RotateLeft(buffer[2] + F(buffer[3], buffer[0], buffer[1]) + m_Block[14], 11);
            buffer[1] = RotateLeft(buffer[1] + F(buffer[2], buffer[3], buffer[0]) + m_Block[15], 19);

            /* Round 2 */
            buffer[0] = RotateLeft(buffer[0] + G(buffer[1], buffer[2], buffer[3]) + m_Block[0] + 0x5A827999, 3);
            buffer[3] = RotateLeft(buffer[3] + G(buffer[0], buffer[1], buffer[2]) + m_Block[4] + 0x5A827999, 5);
            buffer[2] = RotateLeft(buffer[2] + G(buffer[3], buffer[0], buffer[1]) + m_Block[8] + 0x5A827999, 9);
            buffer[1] = RotateLeft(buffer[1] + G(buffer[2], buffer[3], buffer[0]) + m_Block[12] + 0x5A827999, 13);
            buffer[0] = RotateLeft(buffer[0] + G(buffer[1], buffer[2], buffer[3]) + m_Block[1] + 0x5A827999, 3);
            buffer[3] = RotateLeft(buffer[3] + G(buffer[0], buffer[1], buffer[2]) + m_Block[5] + 0x5A827999, 5);
            buffer[2] = RotateLeft(buffer[2] + G(buffer[3], buffer[0], buffer[1]) + m_Block[9] + 0x5A827999, 9);
            buffer[1] = RotateLeft(buffer[1] + G(buffer[2], buffer[3], buffer[0]) + m_Block[13] + 0x5A827999, 13);
            buffer[0] = RotateLeft(buffer[0] + G(buffer[1], buffer[2], buffer[3]) + m_Block[2] + 0x5A827999, 3);
            buffer[3] = RotateLeft(buffer[3] + G(buffer[0], buffer[1], buffer[2]) + m_Block[6] + 0x5A827999, 5);
            buffer[2] = RotateLeft(buffer[2] + G(buffer[3], buffer[0], buffer[1]) + m_Block[10] + 0x5A827999, 9);
            buffer[1] = RotateLeft(buffer[1] + G(buffer[2], buffer[3], buffer[0]) + m_Block[14] + 0x5A827999, 13);
            buffer[0] = RotateLeft(buffer[0] + G(buffer[1], buffer[2], buffer[3]) + m_Block[3] + 0x5A827999, 3);
            buffer[3] = RotateLeft(buffer[3] + G(buffer[0], buffer[1], buffer[2]) + m_Block[7] + 0x5A827999, 5);
            buffer[2] = RotateLeft(buffer[2] + G(buffer[3], buffer[0], buffer[1]) + m_Block[11] + 0x5A827999, 9);
            buffer[1] = RotateLeft(buffer[1] + G(buffer[2], buffer[3], buffer[0]) + m_Block[15] + 0x5A827999, 13);

            /* Round 3 */
            buffer[0] = RotateLeft(buffer[0] + H(buffer[1], buffer[2], buffer[3]) + m_Block[0] + 0x6ED9EBA1, 3);
            buffer[3] = RotateLeft(buffer[3] + H(buffer[0], buffer[1], buffer[2]) + m_Block[8] + 0x6ED9EBA1, 9);
            buffer[2] = RotateLeft(buffer[2] + H(buffer[3], buffer[0], buffer[1]) + m_Block[4] + 0x6ED9EBA1, 11);
            buffer[1] = RotateLeft(buffer[1] + H(buffer[2], buffer[3], buffer[0]) + m_Block[12] + 0x6ED9EBA1, 15);
            buffer[0] = RotateLeft(buffer[0] + H(buffer[1], buffer[2], buffer[3]) + m_Block[2] + 0x6ED9EBA1, 3);
            buffer[3] = RotateLeft(buffer[3] + H(buffer[0], buffer[1], buffer[2]) + m_Block[10] + 0x6ED9EBA1, 9);
            buffer[2] = RotateLeft(buffer[2] + H(buffer[3], buffer[0], buffer[1]) + m_Block[6] + 0x6ED9EBA1, 11);
            buffer[1] = RotateLeft(buffer[1] + H(buffer[2], buffer[3], buffer[0]) + m_Block[14] + 0x6ED9EBA1, 15);
            buffer[0] = RotateLeft(buffer[0] + H(buffer[1], buffer[2], buffer[3]) + m_Block[1] + 0x6ED9EBA1, 3);
            buffer[3] = RotateLeft(buffer[3] + H(buffer[0], buffer[1], buffer[2]) + m_Block[9] + 0x6ED9EBA1, 9);
            buffer[2] = RotateLeft(buffer[2] + H(buffer[3], buffer[0], buffer[1]) + m_Block[5] + 0x6ED9EBA1, 11);
            buffer[1] = RotateLeft(buffer[1] + H(buffer[2], buffer[3], buffer[0]) + m_Block[13] + 0x6ED9EBA1, 15);
            buffer[0] = RotateLeft(buffer[0] + H(buffer[1], buffer[2], buffer[3]) + m_Block[3] + 0x6ED9EBA1, 3);
            buffer[3] = RotateLeft(buffer[3] + H(buffer[0], buffer[1], buffer[2]) + m_Block[11] + 0x6ED9EBA1, 9);
            buffer[2] = RotateLeft(buffer[2] + H(buffer[3], buffer[0], buffer[1]) + m_Block[7] + 0x6ED9EBA1, 11);
            buffer[1] = RotateLeft(buffer[1] + H(buffer[2], buffer[3], buffer[0]) + m_Block[15] + 0x6ED9EBA1, 15);

            unchecked
            {
                m_Buffer[0] += buffer[0];
                m_Buffer[1] += buffer[1];
                m_Buffer[2] += buffer[2];
                m_Buffer[3] += buffer[3];
            }
        }

        static uint F(uint x, uint y, uint z)
        {
            // XY v not(X) Z
            return (x & y) | (~x & z);
        }

        static uint G(uint x, uint y, uint z)
        {
            // XY v XZ v YZ
            return (x & y) | (x & z) | (y & z);
        }

        static uint H(uint x, uint y, uint z)
        {
            // X XOR Y XOR Z
            return x ^ y ^ z;
        }

        static uint RotateLeft(uint x, uint n)
        {
            return (x << (int)n) | (x >> (32 - (int)n));
        }
    }
}