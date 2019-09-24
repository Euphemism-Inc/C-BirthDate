// (c) Euphemism Inc. All right reserved.

using System;
using System.Globalization;
using System.Threading;

namespace Coconut.Library.Common.Format
{
    /// <summary>
    /// BirthDate Format Provider
    /// </summary>
    /// <seealso cref="IFormatProvider" />
    public sealed class BirthDateFormatProvider : IFormatProvider
    {
        private readonly BirthDate _birthDate;
        private readonly CultureInfo _cultureInfo;

        /// <summary>
        /// Initializes a new instance of the <see cref="BirthDateFormatProvider"/> class.
        /// </summary>
        /// <param name="birthDate">The birth date.</param>
        /// <param name="cultureInfo"></param>
        public BirthDateFormatProvider(
            BirthDate birthDate,
            CultureInfo cultureInfo
        ) : this(birthDate)
        {
            _cultureInfo = cultureInfo ?? throw new ArgumentNullException(nameof(cultureInfo));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BirthDateFormatProvider"/> class.
        /// </summary>
        /// <param name="birthDate">The birth date.</param>
        public BirthDateFormatProvider(
            BirthDate birthDate
        )
        {
            _birthDate = birthDate;
            _cultureInfo = Thread.CurrentThread.CurrentUICulture;
        }

        /// <summary>
        /// Returns an object that provides formatting services for the specified type.
        /// </summary>
        /// <param name="formatType">An object that specifies the type of format object to return.</param>
        /// <returns>
        /// An instance of the object specified by <paramref name="formatType" />, if the <see cref="T:System.IFormatProvider" /> implementation can supply that type of object; otherwise, null.
        /// </returns>
        public object GetFormat(Type formatType)
        {
            if (formatType == typeof(BirthDateFormatInfo))
            {
                return new BirthDateFormatInfo(_birthDate, _cultureInfo);
            }
            else
            {
                return null;
            }
        }
    }
}
