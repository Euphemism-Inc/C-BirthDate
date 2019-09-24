// (c) Euphemism Inc. All right reserved.

using Coconut.Library.Common.Extensions;
using Coconut.Library.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Coconut.Library.Common.Parse
{
    /// <summary>
    /// 
    /// </summary>
    internal sealed class BirthDateParse
    {
        private static readonly string formatException = "Could not format string ('{0}') to BirthDate.";

        private readonly BirthDateParseInfo _birthDateParseInfo;
        private readonly DateTimeFormatInfo _dateTimeFormatInfo;
        private readonly IList<string> _formats;
        private readonly Calendar _calendar;

        private readonly IList<string> _threeLetterMonthNames;
        private readonly IList<string> _fullMonthNames;
        private readonly IList<string> _threeLetterDayNames;
        private readonly IList<string> _fullDayNames;

        /// <summary>
        /// Initializes a new instance of the <see cref="BirthDateParse"/> class.
        /// </summary>
        /// <param name="birthDateParseInfo">The birth date parse information.</param>
        /// <param name="formats">The formats.</param>
        public BirthDateParse(BirthDateParseInfo birthDateParseInfo, IList<string> formats = null)
        {
            _birthDateParseInfo = birthDateParseInfo;
            _formats = formats ?? new List<string> { birthDateParseInfo.DefaultFormat };

            var cultureInfo = birthDateParseInfo.CultureInfo;
            _dateTimeFormatInfo = DateTimeFormatInfo.GetInstance(cultureInfo);
            _calendar = cultureInfo.Calendar;

            _threeLetterMonthNames = MakeCleanList(_dateTimeFormatInfo.AbbreviatedMonthNames, _birthDateParseInfo.CultureInfo);
            _fullMonthNames = MakeCleanList(_dateTimeFormatInfo.MonthNames, _birthDateParseInfo.CultureInfo);
            _threeLetterDayNames = MakeCleanList(_dateTimeFormatInfo.AbbreviatedDayNames, _birthDateParseInfo.CultureInfo);
            _fullDayNames = MakeCleanList(_dateTimeFormatInfo.DayNames, _birthDateParseInfo.CultureInfo);
        }

        /// <summary>
        /// Parses the exact.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <returns></returns>
        /// <exception cref="FormatException"></exception>
        public BirthDate ParseExact(string str)
        {
            foreach (string format in _formats)
            {
                var result = DoStrictParse(str, format);
                if (result.Success)
                {
                    return result.ToBirthDate();
                }
            }

            throw new FormatException(String.Format(formatException, str));
        }

        // - - - - -

        /// <summary>
        /// Do parse the string strictly to a <see cref="BirthDateParseResult"/> using a format string..
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="format">The format.</param>
        /// <remarks>
        /// Can only pase a string with a format with the year, month and day specified as numbers or month by name.
        /// </remarks>
        /// <returns></returns>
        private BirthDateParseResult DoStrictParse(string str, string format)
        {
            var result = new BirthDateParseResult()
            {
                Success = true
            };

            int formatPos = 0;
            int tokenLenFormatString;

            int parsePos = 0;
            int tokenLenParseString;

            while (formatPos < format.Length)
            {
                DoStrictParse(result, format, formatPos, str, parsePos, out tokenLenFormatString, out tokenLenParseString);
                formatPos += tokenLenFormatString;
                parsePos += tokenLenParseString;

                if (!result.Success) break;
            }

            result.Success &=
                result.YearIsFound &&
                result.MonthIsFound &&
                result.DayIsFound &&
                parsePos == str.Length &&
                result.DayMatchesDate();

            return result;
        }

        /// <summary>
        /// Do parse the <paramref name="str"/> strictly to a <see cref="BirthDateParseResult"/> using a <paramref name="format"/>..
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="format">The format.</param>
        /// <param name="formatPos">The format position.</param>
        /// <param name="str">The string.</param>
        /// <param name="parsePos">The parse position.</param>
        /// <param name="tokenLenFormatString">The token length format string.</param>
        /// <param name="tokenLenParseString">The token length parse string.</param>
        private void DoStrictParse(BirthDateParseResult result, string format, int formatPos, string str, int parsePos, out int tokenLenFormatString, out int tokenLenParseString)
        {
            switch (format[formatPos])
            {
                case 'd':
                    tokenLenFormatString = GetFormatPartDay(result, format, formatPos);
                    tokenLenParseString = ParseDay(result, str, parsePos);
                    break;
                case 'M':
                    tokenLenFormatString = GetFormatPartMonth(result, format, formatPos);
                    tokenLenParseString = ParseMonth(result, str, parsePos);
                    break;
                case 'y':
                    tokenLenFormatString = GetFormatPartYear(result, format, formatPos);
                    tokenLenParseString = ParseYear(result, str, parsePos);
                    break;
                case '/':
                    tokenLenFormatString = 1;
                    tokenLenParseString = ParsePartDateSeparator(result, str, parsePos);
                    break;
                case '\'':
                case '\"':
                    tokenLenFormatString = FormatHelper.GetQuotedTextLength(format, formatPos);
                    tokenLenParseString = tokenLenFormatString - 2;
                    break;
                case '%':
                    tokenLenFormatString = FormatHelper.GetNextCharLength(format, formatPos, out char nextChar);
                    tokenLenParseString = ParseNextChar(result, str, parsePos, nextChar);
                    break;
                case '\\':
                    tokenLenFormatString = FormatHelper.GetEscapedCharLength(format, formatPos);
                    tokenLenParseString = 1;
                    break;
                default:
                    tokenLenFormatString = 1;
                    tokenLenParseString = 1;
                    result.Success = format[formatPos] == str[parsePos];
                    break;
            }
        }

        /// <summary>
        /// Gets the length of the day format part and put this into <see cref="BirthDateParseResult.DayFormatPartLength"/>
        /// and return it. Also set <see cref="BirthDateParseResult.DayIsText"/>.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="format">The format.</param>
        /// <param name="pos">The position.</param>
        /// <returns></returns>
        /// <seealso cref="Format.BirthDateFormat.FormatDay(System.Text.StringBuilder, string, int, int, DateTime)"/>
        private int GetFormatPartDay(BirthDateParseResult result, string format, int pos)
        {
            int tokenLen = FormatHelper.ParseLengthOfRepeatPattern(format, pos, 'd');

            result.DayFormatPartLength = tokenLen;
            result.DayIsText = tokenLen >= 3;

            return tokenLen;
        }

        /// <summary>
        /// Fills in all the information about the day at the current position and put this information
        /// into <paramref name="result"/>. This includes the actual parsed day, wheter the day was masked,
        /// if the year was day and if the parsing was successfull so far.
        /// Returns the length of the parsed part.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="str">The string.</param>
        /// <param name="pos">The position.</param>
        /// <returns></returns>
        /// <seealso cref="Format.BirthDateFormat.FormatDay(System.Text.StringBuilder, string, int, int, DateTime)"/>
        private int ParseDay(BirthDateParseResult result, string str, int pos)
        {
            bool oldDayIsFound = result.DayIsFound;
            result.DayIsFound = true;

            var fullTextMask = new String(_birthDateParseInfo.TextMaskChar, result.DayFormatPartLength);
            var fullNumberMask = new String(_birthDateParseInfo.NumberMaskChar, result.DayFormatPartLength);
            if (FindString(str, pos, new[] { fullTextMask, fullNumberMask }, false, out int len1, out int obsoleteIndex)
                && (!result.DayIsReal.HasValue || !result.DayIsReal.Value)) // Masked
            {
                result.Day = 0;
                result.DayIsReal = false;
                return len1;
            }
            else if (result.DayFormatPartLength <= 2
                && (!result.DayIsReal.HasValue || result.DayIsReal.Value)) // Day as number. With or without leading zero.
            {
                var fl = result.DayFormatPartLength;
                Func<int, int, bool> isOk = (v, l) => { return v > 0 && v <= 31 && IsTwoDigitRightLength(v, l, fl); };
                if (
                    TryParseInt(str, pos, 2, out int len, out int value, isOk) // Actual number
                    && (result.DayFormatPartLength == 2 ? len == 2 : true)
                )
                {
                    result.Day = (short)value;
                    result.DayIsReal = true;
                    return len;
                }
            }
            // Parsing days as names of the week is undoable. We'll get ~4 matches in a month. So, instead we will
            // find them, but not alter the result if the result is not masked. (result.DayIsFound = oldDayIsFound)
            else if (result.DayFormatPartLength == 3
                && (!result.DayIsReal.HasValue || result.DayIsReal.Value)) // Day as three characters
            {
                if (FindString(str, pos, _threeLetterDayNames, true, out int len, out int index)
                    && (!result.DayOfWeekIndex.HasValue || result.DayOfWeekIndex.Value == index)) // Not masked
                {
                    // We can't fill in 'result.Day' with just day of the week names, neither can we check if the day is correct.
                    result.DayOfWeekIndex = index;
                    result.DayIsFound = oldDayIsFound;
                    result.DayIsText = true;
                    result.DayIsReal = true;
                    return len;
                }
            }
            else if (!result.DayIsReal.HasValue || result.DayIsReal.Value) // Full day names
            {
                if (FindString(str, pos, _fullDayNames, true, out int len, out int index)
                    && (!result.DayOfWeekIndex.HasValue || result.DayOfWeekIndex.Value == index)) // Not masked
                {
                    // We can't fill in 'result.Day' with just day of the week names, neither can we check if the day is correct.
                    result.DayOfWeekIndex = index;
                    result.DayIsFound = oldDayIsFound;
                    result.DayIsText = true;
                    result.DayIsReal = true;
                    return len;
                }
            }

            result.DayIsFound = oldDayIsFound;
            result.Success = false;
            return -1;
        }

        /// <summary>
        /// Gets the length of the month format part and put this into <see cref="BirthDateParseResult.MonthFormatPartLength"/>
        /// and return it. Also set <see cref="BirthDateParseResult.MonthIsText"/>.
        /// </summary>
        /// <seealso cref="Format.BirthDateFormat.FormatMonth(System.Text.StringBuilder, string, int, int)"/>
        private int GetFormatPartMonth(BirthDateParseResult result, string format, int pos)
        {
            int tokenLen = FormatHelper.ParseLengthOfRepeatPattern(format, pos, 'M');

            result.MonthFormatPartLength = tokenLen;
            result.MonthIsText = tokenLen >= 3;

            return tokenLen;
        }

        /// <summary>
        /// Fills in all the information about the month at the current position and put this information
        /// into <paramref name="result"/>. This includes the actual parsed month, wheter the month was masked,
        /// if the month was real and if the parsing was successfull so far.
        /// Returns the length of the parsed part.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="str">The string.</param>
        /// <param name="pos">The position.</param>
        /// <returns></returns>
        /// <seealso cref="Format.BirthDateFormat.FormatMonth(System.Text.StringBuilder, string, int, int)"/>
        private int ParseMonth(BirthDateParseResult result, string str, int pos)
        {
            bool oldMonthIsFound = result.MonthIsFound;
            result.MonthIsFound = true;

            var fullTextMask = new String(_birthDateParseInfo.TextMaskChar, result.MonthFormatPartLength);
            var fullNumberMask = new String(_birthDateParseInfo.NumberMaskChar, result.MonthFormatPartLength);
            if (FindString(str, pos, new[] { fullTextMask, fullNumberMask }, false, out int len1, out int obsoleteIndex)
                && (!result.MonthIsReal.HasValue || !result.MonthIsReal.Value)) // Masked
            {
                result.Month = 0;
                result.MonthIsReal = false;
                return len1;
            }
            else if (result.MonthFormatPartLength <= 2
                && (!result.MonthIsReal.HasValue || result.MonthIsReal.Value)) // Month as number. With or without leading zero.
            {
                var fl = result.MonthFormatPartLength;
                Func<int, int, bool> isOk = (v, l) => { return v > 0 && v <= 12 && IsTwoDigitRightLength(v, l, fl); };
                if (
                    TryParseInt(str, pos, 2, out int len, out int value, isOk) // Actual number
                    && (result.MonthFormatPartLength == 2 ? len == 2 : true)
                    && (!oldMonthIsFound || (oldMonthIsFound && result.Month == (short)value))
                )
                {
                    result.Month = (short)value;
                    result.MonthIsReal = true;
                    return len;
                }
            }
            else if (result.MonthFormatPartLength == 3
                && (!result.MonthIsReal.HasValue || result.MonthIsReal.Value)) // Month as three characters
            {
                if (FindString(str, pos, _threeLetterMonthNames, true, out int len, out int index)
                    && (!oldMonthIsFound || (oldMonthIsFound && result.Month == (short)index + 1))) // Not masked
                {
                    result.Month = (short)(index + 1);
                    result.MonthIsText = true;
                    result.MonthIsReal = true;
                    return len;
                }
            }
            else if (!result.MonthIsReal.HasValue || result.MonthIsReal.Value) // Full month names
            {
                if (FindString(str, pos, _fullMonthNames, true, out int len, out int index)
                    && (!oldMonthIsFound || (oldMonthIsFound && result.Month == (short)index + 1))) // Not masked
                {
                    result.Month = (short)(index + 1);
                    result.MonthIsText = true;
                    result.MonthIsReal = true;
                    return len;
                }
            }

            result.MonthIsFound = oldMonthIsFound;
            result.Success = false;
            return -1;
        }

        /// <summary>
        /// Gets the length of the year format part and put this into <see cref="BirthDateParseResult.YearFormatPartLength"/>
        /// and return it.
        /// </summary>
        /// <seealso cref="Format.BirthDateFormat.FormatYear(System.Text.StringBuilder, string, int, int)"/>
        private int GetFormatPartYear(BirthDateParseResult result, string format, int pos)
        {
            int tokenLen = FormatHelper.ParseLengthOfRepeatPattern(format, pos, 'y');

            result.YearFormatPartLength = tokenLen;

            return tokenLen;
        }

        /// <summary>
        /// Fills in all the information about the year at the current position and put this information
        /// into <paramref name="result"/>. This includes the actual parsed year, wheter the year was masked,
        /// if the year was real and if the parsing was successfull so far.
        /// Returns the length of the parsed part.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="str">The string.</param>
        /// <param name="pos">The position.</param>
        /// <returns></returns>
        /// <seealso cref="Format.BirthDateFormat.FormatYear(System.Text.StringBuilder, string, int, int)"/>
        private int ParseYear(BirthDateParseResult result, string str, int pos)
        {
            bool oldYearIsFound = result.YearIsFound;
            result.YearIsFound = true;

            var fullMask = new String(_birthDateParseInfo.NumberMaskChar, result.YearFormatPartLength);
            if (FindString(str, pos, fullMask, true, out int len1)
                && (!result.YearIsReal.HasValue || !result.YearIsReal.Value)) // Masked
            {
                result.Year = 0;
                result.YearIsReal = false;
                return len1;
            }
            else if (result.YearFormatPartLength <= 2
                && (!result.YearIsReal.HasValue || result.YearIsReal.Value)) // Year. With or without leading zero.
            {
                var fl = result.YearFormatPartLength;
                Func<int, int, bool> isOk = (v, l) => { return v >= 0 && v <= 99 && IsTwoDigitRightLength(v, l, fl); };
                if (
                    TryParseInt(str, pos, 2, out int len, out int value, isOk) // Actual number
                    && (result.YearFormatPartLength == 2 ? len == 2 : true)
                )
                {
                    var fullYear = (short)MakeYear(value);
                    var yearIsSame = !result.Year.HasValue || YearIsSame(result.Year.Value, fullYear);
                    if (result.Year == null)
                    {
                        result.Year = fullYear;
                        result.YearIsReal = true;
                        return len;
                    }
                    else if (yearIsSame)
                    {
                        return len;
                    }
                }
            }
            else if (!result.YearIsReal.HasValue || result.YearIsReal.Value) // Year, Without leading zero. (max digits is 4 since 9999 is max year.)
            {
                Func<int, int, bool> isOk = (v, l) => { return v > 0 && v <= 9999 && IsFourDigitRightLength(v, l); };
                if (
                    TryParseInt(str, pos, 4, out int len, out int value, isOk) // Actual number
                )
                {
                    var yearIsSame = !result.Year.HasValue || YearIsSame(result.Year.Value, (short)value);
                    if (
                        result.Year == null || // not filled in
                        (
                            (value - (value % 100)) / 100) == 0 && // Always set when upper digits are zero
                            yearIsSame // ... and lower are same.
                        )
                    {
                        result.Year = (short)value;
                        result.YearIsReal = true;
                        return len;
                    }
                    else if (yearIsSame)
                    {
                        return len;
                    }
                }
            }

            result.YearIsFound = oldYearIsFound;
            result.Success = false;
            return -1;
        }

        /// <summary>
        /// Gets the length of the date separator and return it. Set <see cref="BirthDateParseResult.Success"/> to
        /// <c>false</c> if not found.
        /// </summary>
        private int ParsePartDateSeparator(BirthDateParseResult result, string str, int pos)
        {
            var birthDateSeparator = _dateTimeFormatInfo.DateSeparator;
            if (FindString(str, pos, birthDateSeparator, false, out int len))
            {
                return len;
            }

            result.Success = false;
            return -1;
        }

        /// <summary>
        /// Parse the next character right after the current character.
        /// </summary>
        private int ParseNextChar(BirthDateParseResult result, string str, int pos, char nextChar)
        {
            string format = new String(nextChar, 1);
            DoStrictParse(result, format, 0, str, pos, out int obsoleteFormatTokenLength, out int tokenLenParseString);
            return tokenLenParseString;
        }

        // - - - -

        /// <summary>
        /// Tries to parse the integer at the current position.
        /// </summary>
        /// <param name="str">The string to parse from.</param>
        /// <param name="pos">The position in the parsable string.</param>
        /// <param name="maxLen">The maximum length of the integer.</param>
        /// <param name="len">The expected length of the integer.</param>
        /// <param name="value">The value.</param>
        /// <param name="isOk">The check to see if the integer is parsed right. Aka, a month can have a max of 31 days and no less then 28.</param>
        /// <returns></returns>
        private bool TryParseInt(string str, int pos, int maxLen, out int len, out int value, Func<int, int, bool> isOk)
        {
            int endPos = pos + maxLen;
            if (endPos > str.Length)
            {
                endPos = str.Length;
            }
            len = endPos - pos;
            value = 0;

            for (; endPos > pos && len > 0; endPos--)
            {
                var numberStr = new String(str.Skip(pos).Take(len).ToArray());
                if (int.TryParse(numberStr, NumberStyles.None, _birthDateParseInfo.CultureInfo, out value)
                    && isOk(value, len)
                )
                {
                    return true;
                }

                len--;
            }

            return false;
        }

        /// <summary>
        /// Finds one of the <paramref name="stringsToFind"/> in the <paramref name="str"/> at <paramref name="pos"/>.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="pos">The position.</param>
        /// <param name="stringsToFind">The strings to find.</param>
        /// <param name="toLower"></param>
        /// <param name="len">The length.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        private bool FindString(string str, int pos, IList<string> stringsToFind, bool toLower, out int len, out int index)
        {
            len = 0;
            index = -1;

            foreach (string stringToFind in stringsToFind)
            {
                index++;
                if (FindString(str, pos, stringToFind, toLower, out len)) return true;
            }

            return false;
        }

        /// <summary>
        /// Finds the <paramref name="stringToFind"/> in the <paramref name="str"/> at <paramref name="pos"/>.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="pos">The position.</param>
        /// <param name="stringToFind">The string to find.</param>
        /// <param name="toLower"></param>
        /// <param name="len">The length.</param>
        /// <returns></returns>
        private bool FindString(string str, int pos, string stringToFind, bool toLower, out int len)
        {
            len = stringToFind.Length;

            if (pos + len > str.Length)
            {
                return false;
            }

            var foundString = new String(
                str
                .Skip(pos)
                .Take(len)
                .Select(x => toLower ? Char.ToLowerInvariant(x) : x)
                .ToArray()
            );

            return stringToFind.SequenceEqual(foundString);
        }

        /// <summary>
        /// Determines whether the <paramref name="value"/> is of the right length as a <see cref="System.String"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="length">The length.</param>
        /// <param name="formatLength">Length of the format.</param>
        /// <returns></returns>
        private static bool IsTwoDigitRightLength(int value, int length, int formatLength)
        {
            return formatLength == 1 ?
                (length == 2 && value >= 10) || (length == 1 && value < 10) :
                length == 2;
        }

        /// <summary>
        /// Determines whether the <paramref name="value"/> is of the right length as a <see cref="System.String"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="length">The length.</param>
        /// <returns></returns>
        private static bool IsFourDigitRightLength(int value, int length)
        {
            return (length == 4 && value <= 9999 && value >= 1000) ||
                (length == 3 && value <= 999 && value >= 100) ||
                (length == 2 && value <= 99 && value >= 10) ||
                (length == 1 && value <= 9 && value >= 1);
        }

        /// <summary>
        /// Checks if the two years are the same. If one is 00 and the other is 2000,
        /// they are the same. However, if one is 1999 and the other is 00, then not.
        /// </summary>
        /// <param name="currentParsedYear">The current parsed year.</param>
        /// <param name="newYear">The new year.</param>
        /// <returns></returns>
        private bool YearIsSame(short currentParsedYear, short newYear)
        {
            var currentLower = currentParsedYear % 100;
            var currentHigher = (currentParsedYear - currentLower) / 100;
            var currentHasHigher = currentHigher != 0;

            var newLower = newYear % 100;
            var newHigher = (newYear - newLower) / 100;
            var newHasHigher = newHigher != 0;

            return currentHasHigher && newHasHigher ?
                currentParsedYear == newYear :
                currentLower == newLower;
        }

        /// <summary>
        /// Makes a full year out of the current value. If 00, the actual number is probably 2000, not 1900. Therefore, return that.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        private int MakeYear(int value)
        {
            var currYear = DateTime.Now.Year;
            var cutOffSmYear = currYear % 100;
            var cutOffbgYear = currYear - cutOffSmYear;
            var year = value <= cutOffSmYear ? value + cutOffbgYear : value + cutOffbgYear - 100;
            return year;
        }

        /// <summary>
        /// Make the input list of names small and skip empty entries.
        /// </summary>
        /// <param name="names"></param>
        /// <param name="cultureInfo"></param>
        /// <returns></returns>
        private static IList<string> MakeCleanList(IList<string> names, CultureInfo cultureInfo)
        {
            return names.Where(x => !String.IsNullOrEmpty(x))
                        .Select(x => x.ToLower(cultureInfo))
                        .ToArray();
        }
    }
}
