// (c) Euphemism Inc. All right reserved.

namespace Coconut.Library.Common.Parse
{
    /// <summary>
    /// The result of the <see cref="BirthDateParse.DoStrictParse(string, string)"/> function. Can be converted to a <see cref="BirthDate"/>.
    /// </summary>
    internal sealed class BirthDateParseResult
    {
        /// <summary>
        /// Gets or sets a value indicating whether the process of parsing the <see cref="System.String"/> to
        /// <see cref="BirthDate"/> via <see cref="BirthDateParse.DoStrictParse(string, string)"/> was successfull.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// The year found while parsing.
        /// </summary>
        public short? Year { get; set; }

        /// <summary>
        /// The found year is real and thus non-zero.
        /// </summary>
        public bool? YearIsReal { get; set; }

        /// <summary>
        /// The length of the year format part.
        /// </summary>
        public int YearFormatPartLength { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the year is found in the string to parse.
        /// </summary>
        public bool YearIsFound { get; set; }


        /// <summary>
        /// The month found while parsing.
        /// </summary>
        public short? Month { get; set; }

        /// <summary>
        /// The found month is real and thus non-zero.
        /// </summary>
        public bool? MonthIsReal { get; set; }

        /// <summary>
        /// The length of the month format part.
        /// </summary>
        public int MonthFormatPartLength { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the month is found in the string to parse.
        /// </summary>
        public bool MonthIsFound { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the found month was found as text. (jan, january, feb, ...)
        /// </summary>
        public bool? MonthIsText { get; set; }


        /// <summary>
        /// The day found while parsing.
        /// </summary>
        public short? Day { get; set; }

        /// <summary>
        /// The found day is real and thus non-zero.
        /// </summary>
        public bool? DayIsReal { get; set; }

        /// <summary>
        /// The length of the day format part.
        /// </summary>
        public int DayFormatPartLength { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the day is found in the string to parse.
        /// </summary>
        public bool DayIsFound { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the found day was found as text. (mon, monday, wed, ...)
        /// </summary>
        public bool? DayIsText { get; set; }

        /// <summary>
        /// Gets or sets the day of the week index.
        /// </summary>
        public int? DayOfWeekIndex { get; set; }
    }
}
