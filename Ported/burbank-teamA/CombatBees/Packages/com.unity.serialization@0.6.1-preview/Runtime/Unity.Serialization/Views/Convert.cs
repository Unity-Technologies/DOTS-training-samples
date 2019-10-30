using System;
using System.Runtime.InteropServices;

namespace Unity.Serialization
{
    static class Convert
    {
        public enum ParseError
        {
            None,
            Syntax,
            Overflow,
            Underflow,
        }

        [StructLayout(LayoutKind.Explicit)]
        struct UintFloatUnion
        {
            [FieldOffset(0)]
            public uint uintValue;
            [FieldOffset(0)]
            public float floatValue;
        }

        const ulong k_ULongMaxDiv10 = ulong.MaxValue / 10;

        public static unsafe ParseError StrToInt64(char* c, int len, out long output)
        {
            var value = 0L;
            var sign = 1;
            var i = 0;

            if (c[i] == '+' || c[i] == '-' )
            {
                if (c[i] == '-')
                {
                    sign = -1;
                }

                i++;
            }

            for (; i < len; i++)
            {
                if (!IsDigit(c[i]))
                {
                    break;
                }

                if ((ulong) value > k_ULongMaxDiv10)
                {
                    output = 0;
                    return ParseError.Overflow;
                }

                value *= 10;
                value += c[i] - '0';
            }

            if (sign == -1)
            {
                value = -value;

                if (value > 0) {
                    output = 0;
                    return ParseError.Overflow;
                }
            }
            else
            {
                if (value < 0) {
                    output = 0;
                    return ParseError.Overflow;
                }
            }
            output = value;
            return ParseError.None;
        }

        public static unsafe ParseError StrToUInt64(char* c, int len, out ulong output)
        {
            var value = 0UL;
            var i = 0;

            if (c[i] == '+' || c[i] == '-' )
            {
                if (c[i] == '-')
                {
                    output = 0;
                    return ParseError.Underflow;
                }

                i++;
            }

            for (; i < len; i++)
            {
                if (!IsDigit(c[i]))
                {
                    break;
                }

                if (value > k_ULongMaxDiv10)
                {
                    output = 0;
                    return ParseError.Overflow;
                }

                value *= 10;
                var temp = value + (ulong) (c[i] - '0');

                if (temp < value)
                {
                    output = 0;
                    return ParseError.Overflow;
                }

                value = temp;
            }

            output = value;
            return ParseError.None;
        }

        /// <summary>
        /// Ripped from Unity.Entities.Words.
        ///
        /// This assembly can't depend on `Unity.Entities` yet. Waiting for baselib.
        /// </summary>
        public static unsafe ParseError StrToFloat32(char* c, int len, out float output)
        {
            if (MatchesNaN(c, len))
            {
                output = float.NaN;
                return ParseError.None;
            }
            
            output = 0;
            var value = 0f;
            var sign = 1;
            var i = 0;

            if (c[i] == '+' || c[i] == '-' )
            {
                if (c[i] == '-')
                {
                    sign = -1;
                }

                i++;
            }

            if (MatchesInfinity(c + i, len - i))
            {
                output = sign == -1 ? float.NegativeInfinity : float.PositiveInfinity;
                return ParseError.None;
            }

            ulong decimalMantissa = 0;
            var significantDigits = 0;
            var digitsAfterDot = 0;
            var mantissaDigits = 0;

            while (i < len && IsDigit(c[i]))
            {
                ++mantissaDigits;

                if (significantDigits < 9)
                {
                    var temp = decimalMantissa * 10 + (ulong) (c[i] - '0');
                    if (temp > decimalMantissa)
                    {
                        ++significantDigits;
                    }
                    decimalMantissa = temp;
                }
                else
                {
                    --digitsAfterDot;
                }

                ++i;
            }

            if (i < len && c[i] == '.')
            {
                ++i;
                while (i < len && IsDigit(c[i]))
                {
                    mantissaDigits++;

                    if (significantDigits < 9)
                    {
                        var temp = decimalMantissa * 10 + (ulong) (c[i] - '0');
                        if (temp > decimalMantissa)
                        {
                            ++significantDigits;
                        }
                        decimalMantissa = temp;
                        digitsAfterDot++;
                    }
                    ++i;
                }
            }

            if (mantissaDigits == 0)
                return ParseError.Syntax;

            var decimalExponent = 0;
            var decimalExponentSign = 1;

            if (i < len && (c[i]|32) == 'e')
            {
                ++i;
                if (i < len)
                {
                    if (c[i] == '+')
                        ++i;
                    else if (c[i] == '-')
                    {
                        decimalExponentSign = -1;
                        ++i;
                    }
                }

                var exponentDigits = 0;
                while (i < len && IsDigit(c[i]))
                {
                    ++exponentDigits;
                    decimalExponent = decimalExponent * 10 + (c[i] - '0');
                    if (decimalExponent > 38)
                        if(decimalExponentSign == 1)
                            return ParseError.Overflow;
                        else
                            return ParseError.Underflow;
                    ++i;
                }
                if (exponentDigits == 0)
                    return ParseError.Syntax;
            }
            decimalExponent = decimalExponent * decimalExponentSign - digitsAfterDot;
            var error = Base10ToBase2(ref value, decimalMantissa, decimalExponent);
            if (error != ParseError.None)
                return error;
            output = value * sign;
            return ParseError.None;
        }

        static ParseError Base10ToBase2(ref float output, ulong mantissa10, int exponent10)
        {
            if (mantissa10 == 0)
            {
                output = 0.0f;
                return ParseError.None;
            }
            if (exponent10 == 0)
            {
                output = mantissa10;
                return ParseError.None;
            }
            var exponent2 = exponent10;
            var mantissa2 = mantissa10;
            while (exponent10 > 0)
            {
                while ((mantissa2 & 0xe000000000000000U) != 0)
                {
                    mantissa2 >>= 1;
                    ++exponent2;
                }
                mantissa2 *= 5;
                --exponent10;
            }
            while(exponent10 < 0)
            {
                while ((mantissa2 & 0x8000000000000000U) == 0)
                {
                    mantissa2 <<= 1;
                    --exponent2;
                }
                mantissa2 /= 5;
                ++exponent10;
            }
            // TODO: implement math.ldexpf (which presumably handles denormals (i don't))
            UintFloatUnion ufu = new UintFloatUnion();
            ufu.floatValue = mantissa2;
            var e = (int)((ufu.uintValue >> 23) & 0xFFU) - 127;
            e += exponent2;
            if (e > 128)
                return ParseError.Overflow;
            if (e < -127)
                return ParseError.Underflow;
            ufu.uintValue = (ufu.uintValue & ~(0xFFU<<23)) | ((uint)(e + 127) << 23);
            output = ufu.floatValue;
            return ParseError.None;
        }

        public static unsafe bool IsSigned(char* c, int len)
        {
            if (len == 0)
            {
                return false;
            }

            return c[0] == '-';
        }

        public static unsafe bool IsIntegral(char* c, int len)
        {
            if (len == 0) return false;

            var i = 0;

            if (c[i] == '-' || c[i] == '+')
            {
                i++;
            }

            for (; i < len; i++)
            {
                if (!IsDigit(c[i]))  return false;
            }

            return true;
        }

        public static unsafe bool IsDecimal(char* c, int len)
        {
            if (len == 0) return false;

            var i = 0;

            if (c[i] == '-'|| c[i] == '+')
            {
                i++;
            }

            var isDecimal = false;

            for (; i < len; i++)
            {
                if (c[i] == '.' || (c[i]|32) == 'e')
                {
                    isDecimal = true;
                    continue;
                }

                if (!IsDigit(c[i]))
                {
                    return false;
                }
            }

            return isDecimal;
        }

        static bool IsDigit(char c)
        {
            return c >= '0' && c <= '9';
        }
        
        internal static unsafe bool MatchesNaN(char* c, int len)
        {
            var expected = stackalloc char[3] {'n', 'a', 'n'};
            return Matches(c, len, expected, 3);
        }
        
        internal static unsafe bool MatchesInfinity(char* c, int len)
        {
            var expected = stackalloc char[8] {'i', 'n', 'f', 'i', 'n', 'i', 't', 'y'};
            return Matches(c, len, expected, 8);
        }
        
        internal static unsafe bool MatchesTrue(char* c, int len)
        {
            var expected = stackalloc char[4] {'t', 'r', 'u', 'e'};
            return Matches(c, len, expected, 4);
        }

        internal static unsafe bool MatchesFalse(char* c, int len)
        {
            var expected = stackalloc char[5] {'f', 'a', 'l', 's', 'e'};
            return Matches(c, len, expected, 5);
        }
        
        internal static unsafe bool Matches(char* inputAnyCase, int inputLen, char* expectedLowerCase, int expectedLen)
        {
            if (inputLen != expectedLen)
            {
                return false;
            }

            for (var i = 0; i < inputLen; i++)
            {
                if ((inputAnyCase[i] | 32) != expectedLowerCase[i])
                {
                    return false;
                }
            }

            return true;
        }
    }
}
