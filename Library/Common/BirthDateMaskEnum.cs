// (c) Euphemism Inc. All right reserved.

using System;

namespace Coconut.Library.Common
{
    /// <summary>
    /// Mask on a <see cref="BirthDate"/>.
    /// </summary>
    [Flags]
    public enum BirthDateMaskEnum
    {
        /// <summary>
        /// No mask
        /// </summary>
        NoMask = 0,

        /// <summary>
        /// Mask on the day property.
        /// </summary>
        Day = 1,

        /// <summary>
        /// Mask on the month property.
        /// </summary>
        Month = 2,

        /// <summary>
        /// Mask on the year property.
        /// </summary>
        Year = 4
    }
}
