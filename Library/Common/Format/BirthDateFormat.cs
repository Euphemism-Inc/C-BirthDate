// (c) Euphemism Inc. All right reserved.

using Coconut.Library.Common.Helpers;
using System;
using System.Globalization;
using System.Text;

namespace Coconut.Library.Common.Format
{
    internal sealed class BirthDateFormat
    {
        private readonly string _format;
        private readonly BirthDateFormatInfo _birthDateFormatInfo;
        private readonly DateTimeFormatInfo _dateTimeFormatInfo;
        private readonly Calendar _calendar;

        /// <summary>
        /// Initializes a new instance of the <see cref="BirthDateFormat"/> class.
        /// </summary>
        /// <param name="format"></param>
        /// <param name="birthDateFormatInfo"></param>
        public BirthDateFormat(string format, BirthDateFormatInfo birthDateFormatInfo)
        {
            _birthDateFormatInfo = birthDateFormatInfo ?? throw new ArgumentNullException(nameof(birthDateFormatInfo));
            _format = format ?? birthDateFormatInfo.DefaultFormat;

            var cultureInfo = birthDateFormatInfo.CultureInfo;
            _dateTimeFormatInfo = DateTimeFormatInfo.GetInstance(cultureInfo);
            _calendar = cultureInfo.Calendar;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="birthDate"></param>
        /// <returns></returns>
        public string Format(BirthDate birthDate)
        {
            if (birthDate.IsEmpty)
            {
                return String.Empty;
            }
            else
            {
                return CustomFormat(_format, birthDate);
            }
        }

        /// <summary>
        /// Format the <paramref name="birthDate"/> according to the <paramref name="format"/> and <see cref="BirthDateFormatInfo"/>
        /// provided in the constructor of this class.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="birthDate">The birth date.</param>
        /// <returns></returns>
        private string CustomFormat(string format, BirthDate birthDate)
        {
            StringBuilder result = new StringBuilder();
            var dateTime = birthDate.GetDateTimeWithoutMaskInternal();

            int pos = 0;
            int tokenLen;

            while (pos < format.Length)
            {
                char ch = format[pos];
                switch (ch)
                {
                    case 'd':
                        tokenLen = FormatDay(result, format, pos, birthDate.Day, dateTime);
                        break;
                    case 'M':
                        tokenLen = FormatMonth(result, format, pos, birthDate.Month);
                        break;
                    case 'y':
                        tokenLen = FormatYear(result, format, pos, birthDate.Year);
                        break;
                    case '/':
                        tokenLen = FormatDateSeparator(result);
                        break;
                    case '\'':
                    case '\"':
                        tokenLen = FormatHelper.FormatQuoted(result, format, pos);
                        break;
                    case '%':
                        tokenLen = FormatHelper.FormatNextChar(result, format, pos, (fs) => CustomFormat(fs, birthDate));
                        break;
                    case '\\':
                        tokenLen = FormatHelper.FormatEscapedChar(result, format, pos);
                        break;
                    default:
                        result.Append(ch);
                        tokenLen = 1;
                        break;
                }
                pos += tokenLen;
            }
            return result.ToString();
        }

        /// <summary>
        /// Append the <see cref="DateTimeFormatInfo.DateSeparator"/> to the <paramref name="result"/> and
        /// return the length of the token.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns></returns>
        private int FormatDateSeparator(StringBuilder result)
        {
            int tokenLen;
            result.Append(_dateTimeFormatInfo.DateSeparator);
            tokenLen = 1;
            return tokenLen;
        }

        /// <summary>
        /// Format <see cref="BirthDate.Year"/> and append the format-result to the <paramref name="result"/>
        /// and return the length of the token.
        /// </summary>
        /// <remarks>
        /// The formatted year will be:
        ///  - If <see cref="BirthDateFormatInfo.MaskYear"/> is true, masked with
        ///    <see cref="BirthDateFormatInfo.NumberMaskChar"/>, same length as token.
        ///  - Token length = 1: (Year mod 100) without leading zero.
        ///  - Token length = 2: (Year mod 100) with leading zero.
        ///  - Token length > 2: Whole year, without leading zero.
        /// </remarks>
        /// <param name="result">The result.</param>
        /// <param name="format">The format.</param>
        /// <param name="pos">The position.</param>
        /// <param name="year">The year.</param>
        /// <returns></returns>
        private int FormatYear(StringBuilder result, string format, int pos, int year)
        {
            int tokenLen = FormatHelper.ParseLengthOfRepeatPattern(format, pos, 'y');
            if (tokenLen <= 2)
            {
                // tokenLen == 1: Always print (year % 100). No leading zero.
                // tokenLen == 2: Always print (year % 100) with leading zero.
                FormatHelper.FormatDigits(result, year % 100, tokenLen, _birthDateFormatInfo.MaskYear, _birthDateFormatInfo.NumberMaskChar);
            }
            else if (!_birthDateFormatInfo.MaskYear)
            {
                // yyy/yyyy/yyyyy/... : Print year value. No leading zero.
                string fmtPattern = "D" + tokenLen;
                result.Append(year.ToString(fmtPattern, CultureInfo.InvariantCulture));
            }
            else
            {
                result.Append("".PadLeft(tokenLen, _birthDateFormatInfo.NumberMaskChar));
            }

            return tokenLen;
        }

        /// <summary>
        /// Format <see cref="BirthDate.Month"/> and append the format-result to the <paramref name="result"/>
        /// and return the length of the token.
        /// </summary>
        /// <remarks>
        /// The formatted month will be:
        ///  - If <see cref="BirthDateFormatInfo.MaskMonth"/> is true, masked with the same length as token, with
        ///    <see cref="BirthDateFormatInfo.NumberMaskChar"/> (Token Length &lt;= 2) or
        ///    <see cref="BirthDateFormatInfo.TextMaskChar"/> (Token Length &gt; 2).
        ///  - Token length = 1: (Month mod 100) without leading zero.
        ///  - Token length = 2: (Month mod 100) with leading zero.
        ///  - Token length = 3: Month name abbreviated as 3 characters.
        ///  - Token length > 3: Month name (not abbreviated).
        /// </remarks>
        /// <param name="result">The result.</param>
        /// <param name="format">The format.</param>
        /// <param name="pos">The position.</param>
        /// <param name="month">The month.</param>
        /// <returns></returns>
        private int FormatMonth(StringBuilder result, string format, int pos, int month)
        {
            int tokenLen = FormatHelper.ParseLengthOfRepeatPattern(format, pos, 'M');
            if (tokenLen <= 2)
            {
                // tokenLen == 1 : Month as digits with no leading zero.
                // tokenLen == 2 : Month as digits with leading zero for single-digit months.
                FormatHelper.FormatDigits(result, month, tokenLen, _birthDateFormatInfo.MaskMonth, _birthDateFormatInfo.NumberMaskChar);
            }
            else if (tokenLen == 3 && !_birthDateFormatInfo.MaskMonth)
            {
                // tokenLen == 3 : Month as a three-letter abbreviation.
                result.Append(_dateTimeFormatInfo.GetAbbreviatedMonthName(month));
            }
            else if (!_birthDateFormatInfo.MaskMonth)
            {
                // tokenLen >= 4 : Month as its full name.
                result.Append(_dateTimeFormatInfo.GetMonthName(month));
            }
            else
            {
                result.Append("".PadLeft(tokenLen, _birthDateFormatInfo.TextMaskChar));
            }

            return tokenLen;
        }

        /// <summary>
        /// Format <see cref="BirthDate.Day"/> and append the format-result to the <paramref name="result"/>
        /// and return the length of the token.
        /// </summary>
        /// <remarks>
        /// The formatted day will be:
        ///  - If <see cref="BirthDateFormatInfo.MaskDay"/> is true, masked with the same length as token, with
        ///    <see cref="BirthDateFormatInfo.NumberMaskChar"/> (Token Length &lt;= 2) or
        ///    <see cref="BirthDateFormatInfo.TextMaskChar"/> (Token Length &gt; 2).
        ///  - Token length = 1: (Day mod 100) without leading zero.
        ///  - Token length = 2: (Day mod 100) with leading zero.
        ///  - Token length = 3: Day name abbreviated as 3 characters.
        ///  - Token length > 3: Day name (not abbreviated).
        /// </remarks>
        /// <param name="result">The result.</param>
        /// <param name="format">The format.</param>
        /// <param name="pos">The position.</param>
        /// <param name="day">The day.</param>
        /// <param name="dateTime">The date time.</param>
        /// <returns></returns>
        private int FormatDay(StringBuilder result, string format, int pos, int day, DateTime dateTime)
        {
            int tokenLen = FormatHelper.ParseLengthOfRepeatPattern(format, pos, 'd');
            if (tokenLen <= 2)
            {
                // tokenLen == 1 : Day of month as digits with no leading zero.
                // tokenLen == 2 : Day of month as digits with leading zero for single-digit months.
                FormatHelper.FormatDigits(result, day, tokenLen, _birthDateFormatInfo.MaskDay, _birthDateFormatInfo.NumberMaskChar);
            }
            else if (tokenLen == 3 && !_birthDateFormatInfo.MaskDay)
            {
                // tokenLen == 3 : Day of week as a three-leter abbreviation.
                int dayOfWeek = (int)_calendar.GetDayOfWeek(dateTime);
                result.Append(_dateTimeFormatInfo.GetAbbreviatedDayName((DayOfWeek)dayOfWeek));
            }
            else if (!_birthDateFormatInfo.MaskDay)
            {
                // tokenLen >= 4 : Day of week as its full name.
                int dayOfWeek = (int)_calendar.GetDayOfWeek(dateTime);
                result.Append(_dateTimeFormatInfo.GetDayName((DayOfWeek)dayOfWeek));
            }
            else
            {
                result.Append("".PadLeft(tokenLen, _birthDateFormatInfo.TextMaskChar));
            }

            return tokenLen;
        }
    }
}
