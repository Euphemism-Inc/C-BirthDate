// (c) Euphemism Inc. All right reserved.

using System;
using System.Globalization;

namespace Coconut.Library.Common.Format
{
    /// <summary>
    /// The info needed to format a <see cref="BirthDate"/>.
    /// </summary>
    public sealed class BirthDateFormatInfo
    {
        internal const char DefaultNumberMaskChar = '0';
        internal const char DefaultTextMaskChar = 'X';

        /// <summary>
        /// Gets or sets the culture information.
        /// </summary>
        public CultureInfo CultureInfo { get; set; }

        /// <summary>
        /// Gets or sets the default format.
        /// </summary>
        public string DefaultFormat { get; set; }

        /// <summary>
        /// The mask character for numbers.
        /// </summary>
        public char NumberMaskChar { get; set; } = DefaultNumberMaskChar;

        /// <summary>
        /// The mask character for strings.
        /// </summary>
        public char TextMaskChar { get; set; } = DefaultTextMaskChar;

        /// <summary>
        /// Gets a value indicating whether to mask the year.
        /// </summary>
        public bool MaskYear { get; set; }

        /// <summary>
        /// Gets a value indicating whether to mask month.
        /// </summary>
        public bool MaskMonth { get; set; }

        /// <summary>
        /// Gets a value indicating whether to mask day.
        /// </summary>
        public bool MaskDay { get; set; }

        /// <summary>
        /// Prevents a default instance of the <see cref="BirthDateFormatInfo"/> class from being created.
        /// </summary>
        internal BirthDateFormatInfo() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BirthDateFormatInfo"/> class.
        /// </summary>
        /// <param name="birthDate"></param>
        /// <param name="cultureInfo"></param>
        public BirthDateFormatInfo(BirthDate birthDate, CultureInfo cultureInfo)
        {
            CultureInfo = cultureInfo ?? throw new ArgumentNullException(nameof(cultureInfo));
            DefaultFormat = CultureInfo.DateTimeFormat.ShortDatePattern;

            MaskYear = !birthDate.YearIsReal;
            MaskMonth = !birthDate.MonthIsReal;
            MaskDay = !birthDate.DayIsReal;
        }

        /// <summary>
        /// Try get instance of the <see cref="BirthDateFormatInfo"/> class using the given <see cref="IFormatProvider"/>.
        /// </summary>
        /// <param name="formatProvider"></param>
        /// <param name="birthDateFormatInfo"></param>
        /// <returns></returns>
        internal static bool TryGetInstance(IFormatProvider formatProvider, out BirthDateFormatInfo birthDateFormatInfo)
        {
            birthDateFormatInfo = formatProvider.GetFormat(typeof(BirthDateFormatInfo)) as BirthDateFormatInfo;
            return birthDateFormatInfo != null;
        }
    }
}
