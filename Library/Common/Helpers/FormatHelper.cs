// (c) Euphemism Inc. All right reserved.

using System;
using System.Globalization;
using System.Text;

namespace Coconut.Library.Common.Helpers
{
    internal static class FormatHelper
    {
        private static readonly string format_InvalidString = "Invalid format string.";
        private static readonly string format_BadQuote = "Quote does not end: {0}.";


        /// <summary>
        /// Format digits.
        /// </summary>
        /// <param name="result"></param>
        /// <param name="value">The number to format.</param>
        /// <param name="len"></param>
        /// <param name="doMask"></param>
        /// <param name="maskChar"></param>
        public static void FormatDigits(StringBuilder result, int value, int len, bool doMask, char maskChar)
        {
            if (!doMask)
            {
                RightPadDigits(result, value, len);
            }
            else
            {
                result.Append("".PadLeft(len, maskChar));
            }
        }

        /// <summary>
        /// Gets the length of the escaped character sequence.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="pos">The position.</param>
        /// <returns></returns>
        /// <exception cref="System.FormatException"></exception>
        public static int GetEscapedCharLength(string format, int pos)
        {
            int nextChar = ParseNextChar(format, pos);
            if (nextChar >= 0)
            {
                return 2;
            }
            else
            {
                // This means that '\' is at the end of the formatting string.
                throw new FormatException(format_InvalidString);
            }
        }

        /// <summary>
        /// Parse escaped character. Can be used to insert character into the format string.
        /// </summary>
        /// <example>For exmple, "\d" will insert the character 'd' into the string.</example>
        /// <param name="result"></param>
        /// <param name="format"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static int FormatEscapedChar(StringBuilder result, string format, int pos)
        {
            int nextChar = ParseNextChar(format, pos);
            if (nextChar >= 0)
            {
                result.Append(((char)nextChar));
                return 2;
            }
            else
            {
                // This means that '\' is at the end of the formatting string.
                throw new FormatException(format_InvalidString);
            }
        }


        /// <summary>
        /// Returns the length of the next sequence and gets the next character.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="pos">The format position.</param>
        /// <param name="nextChar">The next character.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        internal static int GetNextCharLength(string format, int pos, out char nextChar)
        {
            nextChar = (char)ParseNextChar(format, pos);
            if (nextChar >= 0 && nextChar != '%')
            {
                return 2;
            }
            else
            {
                // This means that '%' is at the end of the format string or
                // "%%" appears in the format string.
                throw new FormatException(format_InvalidString);
            }
        }

        /// <summary>
        /// Format next character.
        /// </summary>
        /// <param name="result"></param>
        /// <param name="format"></param>
        /// <param name="pos"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public static int FormatNextChar(StringBuilder result, string format, int pos, Func<string, string> callback)
        {
            // Optional format character.
            // For example, format string "%d" will print day of month 
            // without leading zero.  Most of the cases, "%" can be ignored.
            int nextChar = ParseNextChar(format, pos);
            // nextChar will be -1 if we already reach the end of the format string.
            // Besides, we will not allow "%%" appear in the pattern.
            if (nextChar >= 0 && nextChar != '%')
            {
                var newFormatString = ((char)nextChar).ToString();
                result.Append(callback(newFormatString));
                return 2;
            }
            else
            {
                // This means that '%' is at the end of the format string or
                // "%%" appears in the format string.
                throw new FormatException(format_InvalidString);
            }
        }

        /// <summary>
        /// Gets the length of the quoted text.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="pos">The position.</param>
        /// <returns></returns>
        public static int GetQuotedTextLength(string format, int pos)
        {
            var quotedText = new StringBuilder();
            return ParseQuoteString(quotedText, format, pos);
        }

        /// <summary>
        /// Format quoted text.
        /// </summary>
        /// <param name="result"></param>
        /// <param name="format"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static int FormatQuoted(StringBuilder result, string format, int pos)
        {
            return ParseQuoteString(result, format, pos);
        }


        /// <summary>
        /// Parse repeated pattern.
        /// </summary>
        /// <param name="format"></param>
        /// <param name="pos"></param>
        /// <param name="patternChar"></param>
        /// <returns></returns>
        public static int ParseLengthOfRepeatPattern(string format, int pos, char patternChar)
        {
            int len = format.Length;
            int index = pos + 1;
            while ((index < len) && (format[index] == patternChar))
            {
                index++;
            }
            return index - pos;
        }

        /// <summary>
        /// Get the next character at the index of 'pos' in the 'format' string.
        /// Return value of -1 means 'pos' is already at the end of the 'format' string.
        /// Otherwise, return value is the int value of the next character.
        /// </summary>
        /// <param name="format"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static int ParseNextChar(string format, int pos)
        {
            if (pos >= format.Length - 1)
            {
                return (-1);
            }
            return format[pos + 1];
        }


        /// <summary>
        /// Parse quoted text.
        /// </summary>
        /// <param name="result"></param>
        /// <param name="format"></param>
        /// <param name="pos"></param>
        /// <remarks>
        /// The pos should point to a quote character. This method will
        /// get the string enclosed by the quote character.
        /// </remarks>
        /// <returns></returns>
        private static int ParseQuoteString(StringBuilder result, string format, int pos)
        {
            // NOTE : pos will be the index of the quote character in the 'format' string.
            int formatLen = format.Length;
            int beginPos = pos;
            char quoteChar = format[pos++]; // Get the character used to quote the following string.

            bool foundQuote = false;
            while (pos < formatLen)
            {
                char ch = format[pos++];
                if (ch == quoteChar)
                {
                    foundQuote = true;
                    break;
                }
                else if (ch == '\\')
                {
                    // The following are used to support escaped character.
                    // Escaped character is also supported in the quoted string.
                    // Therefore, someone can use a format like "'minute:' mm\"" to display:
                    //  minute: 45"
                    // because the second double quote is escaped.
                    if (pos < formatLen)
                    {
                        result.Append(format[pos++]);
                    }
                    else
                    {
                        // This means that '\' is at the end of the formatting string.
                        throw new FormatException(format_InvalidString);
                    }
                }
                else
                {
                    result.Append(ch);
                }
            }

            if (!foundQuote)
            {
                // Here we can't find the matching quote.
                throw new FormatException(String.Format(CultureInfo.CurrentCulture, format_BadQuote, quoteChar));
            }

            // Return the character count including the begin/end quote characters and enclosed string.
            return pos - beginPos;
        }

        /// <summary>
        /// Right pad digits.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="value">The value.</param>
        /// <param name="len">The length.</param>
        /// <exception cref="ArgumentOutOfRangeException">value</exception>
        private static void RightPadDigits(StringBuilder result, int value, int len)
        {
            if (value < 0) throw new ArgumentOutOfRangeException(nameof(value));

            if (value >= (1 * Math.Pow(10, len)))
            {
                len = 0;

                var tmp = value;
                while (tmp != 0)
                {
                    len++;
                    tmp /= 10;
                }
            }

            char[] chars = new char[len];

            for (int i = len - 1; i >= 0; i--)
            {
                int r = value % 10;
                value = (value - r) / 10;

                chars[i] = (char)('0' + r);
            }

            result.Append(chars);
        }
    }
}
