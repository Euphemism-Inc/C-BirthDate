// (c) Euphemism Inc. All right reserved.

using Coconut.Library.Common.Format;
using System;
using System.Globalization;

namespace Coconut.Library.Common.Parse
{
    /// <summary>
    /// The info needed to parse a <see cref="BirthDate"/>.
    /// </summary>
    public sealed class BirthDateParseInfo
    {

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
        public char NumberMaskChar { get; set; } = BirthDateFormatInfo.DefaultNumberMaskChar;

        /// <summary>
        /// The mask character for text.
        /// </summary>
        public char TextMaskChar { get; set; } = BirthDateFormatInfo.DefaultTextMaskChar;

        /// <summary>
        /// Prevents a default instance of the <see cref="BirthDateParseInfo"/> class from being created.
        /// </summary>
        internal BirthDateParseInfo() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BirthDateParseInfo"/> class.
        /// </summary>
        /// <param name="cultureInfo"></param>
        public BirthDateParseInfo(CultureInfo cultureInfo)
        {
            CultureInfo = cultureInfo ?? throw new ArgumentNullException(nameof(cultureInfo));
            DefaultFormat = CultureInfo.DateTimeFormat.ShortDatePattern;
        }

        /// <summary>
        /// Try get instance of the <see cref="BirthDateParseInfo"/> class using the given <see cref="IFormatProvider"/>.
        /// </summary>
        /// <param name="formatProvider"></param>
        /// <param name="birthDateParseInfo"></param>
        /// <returns></returns>
        internal static bool TryGetInstance(IFormatProvider formatProvider, out BirthDateParseInfo birthDateParseInfo)
        {
            birthDateParseInfo = formatProvider.GetFormat(typeof(BirthDateParseInfo)) as BirthDateParseInfo;
            return birthDateParseInfo != null;
        }
    }
}
