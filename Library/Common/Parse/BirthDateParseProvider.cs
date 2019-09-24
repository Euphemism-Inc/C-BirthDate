// (c) Euphemism Inc. All right reserved.

using System;
using System.Globalization;
using System.Threading;

namespace Coconut.Library.Common.Parse
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="IFormatProvider" />
    public sealed class BirthDateParseProvider : IFormatProvider
    {
        private CultureInfo _cultureInfo;

        /// <summary>
        /// Initializes a new instance of the <see cref="BirthDateParseProvider"/> class.
        /// </summary>
        public BirthDateParseProvider()
            : this(Thread.CurrentThread.CurrentUICulture)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BirthDateParseProvider"/> class.
        /// </summary>
        /// <param name="cultureInfo">The culture information.</param>
        public BirthDateParseProvider(CultureInfo cultureInfo)
        {
            _cultureInfo = cultureInfo;
        }

        /// <summary>
        /// Returns an object that provides formatting services for the specified type.
        /// </summary>
        /// <param name="formatType">An object that specifies the type of format object to return.</param>
        /// <returns>
        /// An instance of the object specified by <paramref name="formatType" />, if the <see cref="T:System.IFormatProvider" /> implementation can supply that type of object; otherwise, null.
        /// </returns>
        /// <exception cref="NotImplementedException"></exception>
        public object GetFormat(Type formatType)
        {
            if (formatType == typeof(BirthDateParseInfo))
            {
                return new BirthDateParseInfo(_cultureInfo);
            }
            else
            {
                return null;
            }
        }
    }
}
