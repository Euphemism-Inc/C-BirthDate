// (c) Euphemism Inc. All right reserved.

using Coconut.Library.Common.Parse;
using System;

namespace Coconut.Library.Common.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="BirthDateParseResult"/>.
    /// </summary>
    internal static class BirthDateParseResultExtensions
    {
        /// <summary>
        /// Convert <see cref="BirthDateParseResult"/> to <see cref="BirthDate"/>.
        /// </summary>
        /// <param name="parseResult">The parse result.</param>
        /// <returns></returns>
        /// <exception cref="FormatException"></exception>
        public static BirthDate ToBirthDate(this BirthDateParseResult parseResult)
        {
            if (!parseResult.Success)
            {
                throw new InvalidOperationException();
            }

            return new BirthDate(
                parseResult.ToDateTime(),
                parseResult.ToBirthDateMaskEnum()
            );
        }

        /// <summary>
        /// Check if the day found matches the date.
        /// </summary>
        /// <param name="parseResult">The parse result.</param>
        /// <returns></returns>
        public static bool DayMatchesDate(this BirthDateParseResult parseResult)
        {
            if (!parseResult.Success)
            {
                throw new InvalidOperationException();
            }

            return !parseResult.DayOfWeekIndex.HasValue || (parseResult.ToDateTime().DayOfWeek == (DayOfWeek)parseResult.DayOfWeekIndex.Value);
        }

        /// <summary>
        /// Convert <see cref="BirthDateParseResult"/> to <see cref="BirthDateMaskEnum"/>.
        /// </summary>
        /// <param name="parseResult">The parse result.</param>
        /// <returns></returns>
        /// <exception cref="FormatException"></exception>
        private static BirthDateMaskEnum ToBirthDateMaskEnum(this BirthDateParseResult parseResult)
        {
            return BirthDateMaskEnum.NoMask |
                (parseResult.YearIsReal.Value ? 0 : BirthDateMaskEnum.Year) |
                (parseResult.MonthIsReal.Value ? 0 : BirthDateMaskEnum.Month) |
                (parseResult.DayIsReal.Value ? 0 : BirthDateMaskEnum.Day);
        }

        /// <summary>
        /// Convert <see cref="BirthDateParseResult"/> to <see cref="DateTime"/>.
        /// </summary>
        /// <param name="parseResult">The parse result.</param>
        /// <returns></returns>
        private static DateTime ToDateTime(this BirthDateParseResult parseResult)
        {
            return new DateTime(
                parseResult.YearIsReal.Value ? parseResult.Year.Value : 1,
                parseResult.MonthIsReal.Value ? parseResult.Month.Value : 1,
                parseResult.DayIsReal.Value ? parseResult.Day.Value : 1
            );
        }
    }
}
