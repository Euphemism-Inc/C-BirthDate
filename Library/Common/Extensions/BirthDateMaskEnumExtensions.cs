// (c) Euphemism Inc. All right reserved.

namespace Coconut.Library.Common.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="BirthDateMaskEnum"/>.
    /// </summary>
    public static class BirthDateMaskEnumExtensions
    {
        /// <summary>
        /// Determines whether the mask is valid.
        /// </summary>
        /// <param name="mask">The mask.</param>
        /// <returns>
        ///   <c>true</c> if the mask is valid; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsValidMask(this BirthDateMaskEnum mask)
        {
            bool hasYear = (mask & BirthDateMaskEnum.Year) != 0;
            bool hasMonth = (mask & BirthDateMaskEnum.Month) != 0;
            bool hasDay = (mask & BirthDateMaskEnum.Day) != 0;

            return !(hasYear && !hasMonth) &&
                !(hasYear && !hasDay) &&
                !(hasMonth && !hasDay);
        }
    }
}
