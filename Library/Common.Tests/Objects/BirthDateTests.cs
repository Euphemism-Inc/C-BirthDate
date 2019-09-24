// (c) Euphemism Inc. All right reserved.

using Coconut.Library.Common.Extensions;
using Coconut.Library.Common.Format;
using Coconut.Library.Common.Parse;
using Coconut.Library.Common.Tests.Objects.TestResources;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Coconut.Library.Common.Tests.Objects
{
    /// <summary>
    /// Tests for <see cref="BirthDate"/>.
    /// </summary>
    [TestClass]
    public class BirthDateTests
    {
        /// <summary>
        /// Test the default initialisation of the <see cref="BirthDate"/> object;
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("Library")]
        [TestCategory("Common")]
        public void BirthDate_Init_Test()
        {
            // Arrange & Act
            BirthDate birthdate = new BirthDate();

            // Assert
            Assert.IsTrue(birthdate.IsEmpty);

            Assert.AreEqual(0, birthdate.Year);
            Assert.AreEqual(0, birthdate.Month);
            Assert.AreEqual(0, birthdate.Day);

            Assert.IsFalse(birthdate.YearIsReal);
            Assert.IsFalse(birthdate.MonthIsReal);
            Assert.IsFalse(birthdate.DayIsReal);
        }

        /// <summary>
        /// Check is stack copy if <see cref="BirthDate"/> are made correctly.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("Library")]
        [TestCategory("Common")]
        public void BirthDate_Copy_Test()
        {
            // Arrange
            BirthDate birthdate_1 = new BirthDate(1987, 7, 18);

            // Act
            BirthDate birthdate_2 = birthdate_1;

            // Assert
            Assert.AreEqual(birthdate_1, birthdate_2);

            Assert.AreEqual(birthdate_1.Year, birthdate_2.Year);
            Assert.AreEqual(birthdate_1.Month, birthdate_2.Month);
            Assert.AreEqual(birthdate_1.Day, birthdate_2.Day);

            Assert.AreEqual(birthdate_1.YearIsReal, birthdate_2.YearIsReal);
            Assert.AreEqual(birthdate_1.MonthIsReal, birthdate_2.MonthIsReal);
            Assert.AreEqual(birthdate_1.DayIsReal, birthdate_2.DayIsReal);
        }

        /// <summary>
        /// Check if the <see cref="BirthDate.YearIsReal"/>, <see cref="BirthDate.MonthIsReal"/> and <see cref="BirthDate.DayIsReal"/> properties are initialised correctly.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("Library")]
        [TestCategory("Common")]
        public void BirthDate_Reality_Test()
        {
            // Arrange

            // Act
            var birthDate_1 = new BirthDate(2000, 1, 1);
            var birthDate_2 = new BirthDate(2000, 1, 0);
            var birthDate_3 = new BirthDate(2000, 0, 0);
            var birthDate_4 = new BirthDate(0, 0, 0);

            var dateTime = new DateTime(2000, 1, 1);
            var birthDate_5 = new BirthDate(dateTime, BirthDateMaskEnum.NoMask);
            var birthDate_6 = new BirthDate(dateTime, BirthDateMaskEnum.Day);
            var birthDate_7 = new BirthDate(dateTime, BirthDateMaskEnum.Month | BirthDateMaskEnum.Day);
            var birthDate_8 = new BirthDate(dateTime, BirthDateMaskEnum.Year | BirthDateMaskEnum.Month | BirthDateMaskEnum.Day);

            var birthDate_9 = new BirthDate();

            // Assert
            Assert.IsTrue(birthDate_1.YearIsReal, "Year must be real.");
            Assert.IsTrue(birthDate_1.MonthIsReal, "Month must be real.");
            Assert.IsTrue(birthDate_1.DayIsReal, "Day must be real.");

            Assert.IsTrue(birthDate_2.YearIsReal, "Year must be real.");
            Assert.IsTrue(birthDate_2.MonthIsReal, "Month must be real.");
            Assert.IsFalse(birthDate_2.DayIsReal, "Day must not be real.");

            Assert.IsTrue(birthDate_3.YearIsReal, "Year must be real.");
            Assert.IsFalse(birthDate_3.MonthIsReal, "Month must not be real.");
            Assert.IsFalse(birthDate_3.DayIsReal, "Day must not be real.");

            Assert.IsFalse(birthDate_4.YearIsReal, "Year must not be real.");
            Assert.IsFalse(birthDate_4.MonthIsReal, "Month must not be real.");
            Assert.IsFalse(birthDate_4.DayIsReal, "Day must not be real.");

            Assert.IsTrue(birthDate_5.YearIsReal, "Year must be real.");
            Assert.IsTrue(birthDate_5.MonthIsReal, "Month must be real.");
            Assert.IsTrue(birthDate_5.DayIsReal, "Day must be real.");

            Assert.IsTrue(birthDate_6.YearIsReal, "Year must be real.");
            Assert.IsTrue(birthDate_6.MonthIsReal, "Month must be real.");
            Assert.IsFalse(birthDate_6.DayIsReal, "Day must not be real.");

            Assert.IsTrue(birthDate_7.YearIsReal, "Year must be real.");
            Assert.IsFalse(birthDate_7.MonthIsReal, "Month must not be real.");
            Assert.IsFalse(birthDate_7.DayIsReal, "Day must not be real.");

            Assert.IsFalse(birthDate_8.YearIsReal, "Year must not be real.");
            Assert.IsFalse(birthDate_8.MonthIsReal, "Month must not be real.");
            Assert.IsFalse(birthDate_8.DayIsReal, "Day must not be real.");

            Assert.IsTrue(birthDate_9.IsEmpty);
        }

        #region ToString

        /// <summary>
        /// Check if <see cref="BirthDate.ToString(IFormatProvider)"/> generates the expected string. Culture dependent: English.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("Library")]
        [TestCategory("Common")]
        public void BirthDate_ToString_English_Test()
        {
            // Arrange
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en-US");
            var birthDate = new BirthDate(2000, 0, 0);
            var formatProvider = new BirthDateFormatProvider(birthDate);

            // Act
            var formattedString = birthDate.ToString(formatProvider);

            // Assert
            Assert.AreEqual("0/0/2000", formattedString);
        }

        /// <summary>
        /// Check if <see cref="BirthDate.ToString(IFormatProvider)"/> generates the expected string. Culture dependent: Dutch.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("Library")]
        [TestCategory("Common")]
        public void BirthDate_ToString_Dutch_Test()
        {
            // Arrange
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("nl-NL");
            var birthDate = new BirthDate(2000, 0, 0);
            var formatProvider = new BirthDateFormatProvider(birthDate);

            // Act
            var formattedString = birthDate.ToString(formatProvider);

            // Assert
            Assert.AreEqual("0-0-2000", formattedString);
        }

        /// <summary>
        /// Check if <see cref="BirthDate.ToString(string, IFormatProvider)"/> generates the expected string. Culture dependent: Dutch.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("Library")]
        [TestCategory("Common")]
        public void BirthDate_ToString_Format_Test()
        {
            // Arrange
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("nl-NL");
            var birthDate = new BirthDate(2000, 12, 1);
            var formatProvider = new BirthDateFormatProvider(birthDate);

            // Act
            var formattedString = birthDate.ToString("yyyy/MM/dd", formatProvider);

            // Assert
            Assert.AreEqual("2000-12-01", formattedString);
        }

        /// <summary>
        /// Check if <see cref="BirthDate.ToString(IFormatProvider)"/> generates the expected string when the <see cref="BirthDate"/> is fully masked. Culture dependent: Dutch.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("Library")]
        [TestCategory("Common")]
        public void BirthDate_ToString_AllZero_Test()
        {
            // Arrange
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("nl-NL");
            var birthDate = new BirthDate(0, 0, 0);
            var formatProvider = new BirthDateFormatProvider(birthDate);

            // Act
            var formattedString = birthDate.ToString(formatProvider);

            // Assert
            Assert.AreEqual("0-0-0000", formattedString);
        }

        /// <summary>
        /// Check if <see cref="BirthDate.ToString(string, IFormatProvider)"/> generates the expected string when the format string should output a three latter month name. Culture dependent: Dutch.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("Library")]
        [TestCategory("Common")]
        public void BirthDate_ToString_Format_ShortMonth_Test()
        {
            // Arrange
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("nl-NL");
            var birthDate = new BirthDate(2000, 12, 1);
            var formatProvider = new BirthDateFormatProvider(birthDate);

            // Act
            var formattedString = birthDate.ToString("MMM", formatProvider);

            // Assert
            Assert.AreEqual("dec", formattedString);
        }

        /// <summary>
        /// Check if <see cref="BirthDate.ToString(string, IFormatProvider)"/> generates the expected string when the format string should output a month name. Culture dependent: Dutch.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("Library")]
        [TestCategory("Common")]
        public void BirthDate_ToString_Format_LongMonth_Test()
        {
            // Arrange
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("nl-NL");
            var birthDate = new BirthDate(2000, 12, 1);
            var formatProvider = new BirthDateFormatProvider(birthDate);

            // Act
            var formattedString = birthDate.ToString("MMMM", formatProvider);

            // Assert
            Assert.AreEqual("december", formattedString);
        }

        /// <summary>
        /// Check if <see cref="BirthDate.ToString(string, IFormatProvider)"/> generates the expected string when the format string should output a two latter day name. Culture dependent: Dutch.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("Library")]
        [TestCategory("Common")]
        public void BirthDate_ToString_Format_ShortDay_Test()
        {
            // Arrange
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("nl-NL");
            var birthDate = new BirthDate(2000, 12, 1);
            var formatProvider = new BirthDateFormatProvider(birthDate);

            // Act
            var formattedString = birthDate.ToString("ddd", formatProvider);

            // Assert
            Assert.AreEqual("vr", formattedString);
        }

        /// <summary>
        /// Check if <see cref="BirthDate.ToString(string, IFormatProvider)"/> generates the expected string when the format string should output a day name. Culture dependent: Dutch.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("Library")]
        [TestCategory("Common")]
        public void BirthDate_ToString_Format_LongDay_Test()
        {
            // Arrange
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("nl-NL");
            var birthDate = new BirthDate(2000, 12, 1);
            var formatProvider = new BirthDateFormatProvider(birthDate);

            // Act
            var formattedString = birthDate.ToString("dddd", formatProvider);

            // Assert
            Assert.AreEqual("vrijdag", formattedString);
        }

        /// <summary>
        /// Check if <see cref="BirthDate.ToString(string, IFormatProvider)"/> generates the expected string when there are quotes in the format string. The text between quotes should be a literal string. Culture dependent: Dutch.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("Library")]
        [TestCategory("Common")]
        public void BirthDate_ToString_Format_Quoted_Test()
        {
            // Arrange
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("nl-NL");
            var birthDate = new BirthDate(2000, 12, 1);
            var formatProvider = new BirthDateFormatProvider(birthDate);

            // Act
            var formattedString = birthDate.ToString("'yyyy/MM/dd' yyyy/MM/dd", formatProvider);

            // Assert
            Assert.AreEqual("yyyy/MM/dd 2000-12-01", formattedString);
        }

        /// <summary>
        /// Check if <see cref="BirthDate.ToString()"/> generates the expected string when uninitialised. Culture dependent: English.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("Library")]
        [TestCategory("Common")]
        public void BirthDate_Uninitialised_ToString_Test()
        {
            // Arrange
            // Just like with DateTime, must be supported (no exception).
            var birthDate = new BirthDate();

            // Act
            var str = birthDate.ToString();

            // Assert
            Assert.AreEqual(String.Empty, str); // Howoever, since no actual date is provided, return empty string.
        }

        #endregion

        #region Parse

        /// <summary>
        /// Check if <see cref="BirthDate.ParseExact(string)"/> generates the expected <see cref="BirthDate"/>. Culture dependent: English.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("Library")]
        [TestCategory("Common")]
        public void BirthDate_ParseExact_English_Test()
        {
            // Arrange
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en-US");
            var expected = new BirthDate(2000, 12, 1);
            string stringToParse = "12/1/2000";

            // Act
            BirthDate parsedBirthDate = BirthDate.ParseExact(stringToParse);

            // Assert
            Assert.AreEqual(expected, parsedBirthDate);
        }

        /// <summary>
        /// Check if <see cref="BirthDate.ParseExact(string)"/> throws a <see cref="FormatException"/> when the format string is not an exact match. Culture dependent: English.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("Library")]
        [TestCategory("Common")]
        [ExpectedException(typeof(FormatException))]
        public void BirthDate_ParseExact_English_Test_Fail()
        {
            // Arrange
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en-US");
            string stringToParse = "12/01/2000";

            // Act
            BirthDate.ParseExact(stringToParse);

            // Assert
            // We expect this to fail because the default format for English is 'M/d/yyyy'. This will format to '12/1/2000'.
        }

        /// <summary>
        /// Check if <see cref="BirthDate.ParseExact(string)"/> generates the expected <see cref="BirthDate"/>. Culture dependent: Dutch.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("Library")]
        [TestCategory("Common")]
        public void BirthDate_ParseExact_Dutch_Test()
        {
            // Arrange
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("nl-NL");
            var expected = new BirthDate(2000, 12, 1);
            string stringToParse = "1-12-2000";

            // Act
            BirthDate parsedBirthDate = BirthDate.ParseExact(stringToParse);

            // Assert
            Assert.AreEqual(expected, parsedBirthDate);
        }

        /// <summary>
        /// Check if <see cref="BirthDate.ParseExact(string, System.Collections.Generic.IList{string}, IFormatProvider)"/> generates the expected <see cref="BirthDate"/> when there are quotes in the format string. Culture dependent: English.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("Library")]
        [TestCategory("Common")]
        public void BirthDate_ParseExact_Quoted_Test()
        {
            // Arrange
            var cultureInfo = CultureInfo.GetCultureInfo("en-US");
            var formatProvider = new BirthDateParseProvider(cultureInfo);

            var expected = new BirthDate(2000, 12, 1);
            string stringToParse = "12/1/2000 M/d/yyyy";
            string formatString = "M/d/yyyy 'M/d/yyyy'";

            // Act
            BirthDate parsedBirthDate = BirthDate.ParseExact(stringToParse, new[] { formatString }, formatProvider);

            // Assert
            Assert.AreEqual(expected, parsedBirthDate);
        }

        /// <summary>
        /// Check if <see cref="BirthDate.ParseExact(string, System.Collections.Generic.IList{string}, IFormatProvider)"/> generates the expected <see cref="BirthDate"/> when there is an escaped character in the format string. Culture dependent: English.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("Library")]
        [TestCategory("Common")]
        public void BirthDate_ParseExact_Escaped_Test()
        {
            // Arrange
            var cultureInfo = CultureInfo.GetCultureInfo("en-US");
            var formatProvider = new BirthDateParseProvider(cultureInfo);

            var expected = new BirthDate(2000, 12, 1);
            string stringToParse = "12/1/2000 M";
            string formatString = @"M/d/yyyy \M";

            // Act
            BirthDate parsedBirthDate = BirthDate.ParseExact(stringToParse, new[] { formatString }, formatProvider);

            // Assert
            Assert.AreEqual(expected, parsedBirthDate);
        }

        /// <summary>
        /// Check if <see cref="BirthDate.ParseExact(string, System.Collections.Generic.IList{string}, IFormatProvider)"/> generates the expected <see cref="BirthDate"/> when there is an single formatted character in the format string. Culture dependent: English.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("Library")]
        [TestCategory("Common")]
        public void BirthDate_ParseExact_SingleChar_Test()
        {
            // Arrange
            var cultureInfo = CultureInfo.GetCultureInfo("en-US");
            var formatProvider = new BirthDateParseProvider(cultureInfo);

            var expected = new BirthDate(2000, 12, 1);
            string stringToParse = "12/1/2000 12";
            string formatString = @"M/d/yyyy %M";

            // Act
            BirthDate parsedBirthDate = BirthDate.ParseExact(stringToParse, new[] { formatString }, formatProvider);

            // Assert
            Assert.AreEqual(expected, parsedBirthDate);
        }

        /// <summary>
        /// Checks if the <see cref="BirthDate"/> parser fails when year is masked and unmasked.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        [TestCategory("UnitTest")]
        [TestCategory("Library")]
        [TestCategory("Common")]
        public void BirthDate_ParseExact_Year_MaskedUnmasked_Test()
        {
            // Arrange
            var cultureInfo = CultureInfo.GetCultureInfo("en-US");
            var formatProvider = new BirthDateParseProvider(cultureInfo);

            string stringToParse = "0/0/0000 010";
            string formatString = @"M/d/yyyy yyy";

            // Act
            BirthDate.ParseExact(stringToParse, new[] { formatString }, formatProvider);
        }

        /// <summary>
        /// Checks if the <see cref="BirthDate"/> parser fails when year is masked and unmasked.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        [TestCategory("UnitTest")]
        [TestCategory("Library")]
        [TestCategory("Common")]
        public void BirthDate_ParseExact_Year_UnmaskedMasked_Test()
        {
            // Arrange
            var cultureInfo = CultureInfo.GetCultureInfo("en-US");
            var formatProvider = new BirthDateParseProvider(cultureInfo);

            string stringToParse = "12/1/2010 0000";
            string formatString = @"M/d/yyyy yyyy";

            // Act
            BirthDate.ParseExact(stringToParse, new[] { formatString }, formatProvider);
        }

        /// <summary>
        /// Check if <see cref="BirthDate.ParseExact(string, System.Collections.Generic.IList{string}, IFormatProvider)"/> generates the expected
        /// <see cref="BirthDate"/> when there is a three letter month name in the format string. Culture dependent: English.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("Library")]
        [TestCategory("Common")]
        public void BirthDate_ParseExact_Month_3_Test()
        {
            // Arrange
            var cultureInfo = CultureInfo.GetCultureInfo("en-US");
            var formatProvider = new BirthDateParseProvider(cultureInfo);

            var expected = new BirthDate(2000, 12, 1);
            string stringToParse = "12/1/2000 dec";
            string formatString = @"M/d/yyyy MMM";

            // Act
            BirthDate parsedBirthDate = BirthDate.ParseExact(stringToParse, new[] { formatString }, formatProvider);

            // Assert
            Assert.AreEqual(expected, parsedBirthDate);
        }

        /// <summary>
        /// Check if <see cref="BirthDate.ParseExact(string, System.Collections.Generic.IList{string}, IFormatProvider)"/> generates the expected
        /// <see cref="BirthDate"/> when there is a month name in the format string. Culture dependent: English.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("Library")]
        [TestCategory("Common")]
        public void BirthDate_ParseExact_Month_4_Test()
        {
            // Arrange
            var cultureInfo = CultureInfo.GetCultureInfo("en-US");
            var formatProvider = new BirthDateParseProvider(cultureInfo);

            var expected = new BirthDate(2000, 12, 1);
            string stringToParse = "12/1/2000 december";
            string formatString = @"M/d/yyyy MMMM";

            // Act
            BirthDate parsedBirthDate = BirthDate.ParseExact(stringToParse, new[] { formatString }, formatProvider);

            // Assert
            Assert.AreEqual(expected, parsedBirthDate);
        }

        /// <summary>
        /// Checks if the <see cref="BirthDate"/> parser fails when month (length 1 or 2) is different.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        [TestCategory("UnitTest")]
        [TestCategory("Library")]
        [TestCategory("Common")]
        public void BirthDate_ParseExact_Month_1_Different_Test()
        {
            // Arrange
            var cultureInfo = CultureInfo.GetCultureInfo("en-US");
            var formatProvider = new BirthDateParseProvider(cultureInfo);

            string stringToParse = "oct 12/0/2000";
            string formatString = @"MMM M/d/yyyy";

            // Act
            BirthDate.ParseExact(stringToParse, new[] { formatString }, formatProvider);
        }

        /// <summary>
        /// Checks if the <see cref="BirthDate"/> parser fails when month (length 3) is different.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        [TestCategory("UnitTest")]
        [TestCategory("Library")]
        [TestCategory("Common")]
        public void BirthDate_ParseExact_Month_3_Different_Test()
        {
            // Arrange
            var cultureInfo = CultureInfo.GetCultureInfo("en-US");
            var formatProvider = new BirthDateParseProvider(cultureInfo);

            string stringToParse = "12/0/2000 oct";
            string formatString = @"M/d/yyyy MMM";

            // Act
            BirthDate.ParseExact(stringToParse, new[] { formatString }, formatProvider);
        }

        /// <summary>
        /// Checks if the <see cref="BirthDate"/> parser fails when month (length 4) is different.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        [TestCategory("UnitTest")]
        [TestCategory("Library")]
        [TestCategory("Common")]
        public void BirthDate_ParseExact_Month_4_Different_Test()
        {
            // Arrange
            var cultureInfo = CultureInfo.GetCultureInfo("en-US");
            var formatProvider = new BirthDateParseProvider(cultureInfo);

            string stringToParse = "12/0/2000 october";
            string formatString = @"M/d/yyyy MMMM";

            // Act
            BirthDate.ParseExact(stringToParse, new[] { formatString }, formatProvider);
        }

        /// <summary>
        /// Checks if the <see cref="BirthDate"/> parser fails when month is masked and unmasked.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        [TestCategory("UnitTest")]
        [TestCategory("Library")]
        [TestCategory("Common")]
        public void BirthDate_ParseExact_Month_MaskedUnmasked_Test()
        {
            // Arrange
            var cultureInfo = CultureInfo.GetCultureInfo("en-US");
            var formatProvider = new BirthDateParseProvider(cultureInfo);

            string stringToParse = "0/0/2000 dec";
            string formatString = @"M/d/yyyy MMM";

            // Act
            BirthDate.ParseExact(stringToParse, new[] { formatString }, formatProvider);
        }

        /// <summary>
        /// Checks if the <see cref="BirthDate"/> parser fails when month is masked and unmasked.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        [TestCategory("UnitTest")]
        [TestCategory("Library")]
        [TestCategory("Common")]
        public void BirthDate_ParseExact_Month_UnmaskedMasked_Test()
        {
            // Arrange
            var cultureInfo = CultureInfo.GetCultureInfo("en-US");
            var formatProvider = new BirthDateParseProvider(cultureInfo);

            string stringToParse = "12/1/2000 XXX";
            string formatString = @"M/d/yyyy MMM";

            // Act
            BirthDate.ParseExact(stringToParse, new[] { formatString }, formatProvider);
        }

        /// <summary>
        /// Check if <see cref="BirthDate.ParseExact(string, System.Collections.Generic.IList{string}, IFormatProvider)"/> generates the expected
        /// <see cref="BirthDate"/> when there is a three letter day name in the format string. Culture dependent: English.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("Library")]
        [TestCategory("Common")]
        public void BirthDate_ParseExact_Day_3_Test()
        {
            // Arrange
            var cultureInfo = CultureInfo.GetCultureInfo("en-US");
            var formatProvider = new BirthDateParseProvider(cultureInfo);

            var expected = new BirthDate(2000, 12, 1);
            string stringToParse = "12/1/2000 fri";
            string formatString = @"M/d/yyyy ddd";

            // Act
            BirthDate parsedBirthDate = BirthDate.ParseExact(stringToParse, new[] { formatString }, formatProvider);

            // Assert
            Assert.AreEqual(expected, parsedBirthDate);
        }

        /// <summary>
        /// Check if <see cref="BirthDate.ParseExact(string, System.Collections.Generic.IList{string}, IFormatProvider)"/> generates the expected
        /// <see cref="BirthDate"/> when there is a day name in the format string. Culture dependent: English.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("Library")]
        [TestCategory("Common")]
        public void BirthDate_ParseExact_Day_4_Test()
        {
            // Arrange
            var cultureInfo = CultureInfo.GetCultureInfo("en-US");
            var formatProvider = new BirthDateParseProvider(cultureInfo);

            var expected = new BirthDate(2000, 12, 1);
            string stringToParse = "12/1/2000 friday";
            string formatString = @"M/d/yyyy dddd";

            // Act
            BirthDate parsedBirthDate = BirthDate.ParseExact(stringToParse, new[] { formatString }, formatProvider);

            // Assert
            Assert.AreEqual(expected, parsedBirthDate);
        }

        /// <summary>
        /// Checks if the <see cref="BirthDate"/> parser fails when day is masked and unmasked.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        [TestCategory("UnitTest")]
        [TestCategory("Library")]
        [TestCategory("Common")]
        public void BirthDate_ParseExact_Day_MaskedUnmasked_Test()
        {
            // Arrange
            var cultureInfo = CultureInfo.GetCultureInfo("en-US");
            var formatProvider = new BirthDateParseProvider(cultureInfo);

            string stringToParse = "12/0/2000 wed";
            string formatString = @"M/d/yyyy ddd";

            // Act
            BirthDate.ParseExact(stringToParse, new[] { formatString }, formatProvider);
        }

        /// <summary>
        /// Checks if the <see cref="BirthDate"/> parser fails when day is masked and unmasked.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        [TestCategory("UnitTest")]
        [TestCategory("Library")]
        [TestCategory("Common")]
        public void BirthDate_ParseExact_Day_UnmaskedMasked_Test()
        {
            // Arrange
            var cultureInfo = CultureInfo.GetCultureInfo("en-US");
            var formatProvider = new BirthDateParseProvider(cultureInfo);

            string stringToParse = "12/1/2000 XXX";
            string formatString = @"M/d/yyyy ddd";

            // Act
            BirthDate.ParseExact(stringToParse, new[] { formatString }, formatProvider);
        }

        /// <summary>
        /// Checks if parsing fails when day name (three letters) in parsing string is not the same as actual day.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        [TestCategory("UnitTest")]
        [TestCategory("Library")]
        [TestCategory("Common")]
        public void BirthDate_ParseExact_Day_3_Dubble_NotEqual()
        {
            // Arrange
            var cultureInfo = CultureInfo.GetCultureInfo("en-US");
            var formatProvider = new BirthDateParseProvider(cultureInfo);

            string stringToParse = "12/1/2000 fri mon"; // was on a friday
            string formatString = @"M/d/yyyy ddd ddd";

            // Act
            BirthDate.ParseExact(stringToParse, new[] { formatString }, formatProvider);
        }

        /// <summary>
        /// Checks if parsing fails when day name (full name) in parsing string is not the same as actual day.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        [TestCategory("UnitTest")]
        [TestCategory("Library")]
        [TestCategory("Common")]
        public void BirthDate_ParseExact_Day_4_Dubble_NotEqual()
        {
            // Arrange
            var cultureInfo = CultureInfo.GetCultureInfo("en-US");
            var formatProvider = new BirthDateParseProvider(cultureInfo);

            string stringToParse = "12/1/2000 friday monday"; // was on a friday
            string formatString = @"M/d/yyyy dddd dddd";

            // Act
            BirthDate.ParseExact(stringToParse, new[] { formatString }, formatProvider);
        }

        /// <summary>
        /// Checks if parsing fails when day name (three letters and full name) in parsing string is not the same as actual day.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        [TestCategory("UnitTest")]
        [TestCategory("Library")]
        [TestCategory("Common")]
        public void BirthDate_ParseExact_Day_3Or4_Dubble_NotEqual()
        {
            // Arrange
            var cultureInfo = CultureInfo.GetCultureInfo("en-US");
            var formatProvider = new BirthDateParseProvider(cultureInfo);

            string stringToParse = "12/1/2000 fri monday"; // was on a friday
            string formatString = @"M/d/yyyy ddd dddd";

            // Act
            BirthDate.ParseExact(stringToParse, new[] { formatString }, formatProvider);
        }

        /// <summary>
        /// Checks if parsing fails when day name (three letters) in parsing string is not the same as actual day.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        [TestCategory("UnitTest")]
        [TestCategory("Library")]
        [TestCategory("Common")]
        public void BirthDate_ParseExact_Day_DoesNotMatchDate()
        {
            // Arrange
            var cultureInfo = CultureInfo.GetCultureInfo("en-US");
            var formatProvider = new BirthDateParseProvider(cultureInfo);

            string stringToParse = "12/1/2000 wed"; // was on a friday
            string formatString = @"M/d/yyyy ddd";

            // Act
            BirthDate.ParseExact(stringToParse, new[] { formatString }, formatProvider);
        }

        /// <summary>
        /// Check if <see cref="BirthDate.ParseExact(string, System.Collections.Generic.IList{string}, IFormatProvider)"/> generates the expected
        /// <see cref="BirthDate"/> when there is a long and short year in the format string. Culture dependent: English.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("Library")]
        [TestCategory("Common")]
        public void BirthDate_ParseExact_Year_1_Test()
        {
            // Arrange
            var cultureInfo = CultureInfo.GetCultureInfo("en-US");
            var formatProvider = new BirthDateParseProvider(cultureInfo);

            var expected = new BirthDate(2001, 12, 1);
            string stringToParse = "12/1/2001 1";
            string formatString = @"M/d/yyyy y";

            // Act
            BirthDate parsedBirthDate = BirthDate.ParseExact(stringToParse, new[] { formatString }, formatProvider);

            // Assert
            Assert.AreEqual(expected, parsedBirthDate);
        }

        /// <summary>
        /// Check if <see cref="BirthDate.ParseExact(string, System.Collections.Generic.IList{string}, IFormatProvider)"/> generates the expected
        /// <see cref="BirthDate"/> when there is a long and short year in the format string. Culture dependent: English.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("Library")]
        [TestCategory("Common")]
        public void BirthDate_ParseExact_Year_2_Test()
        {
            // Arrange
            var cultureInfo = CultureInfo.GetCultureInfo("en-US");
            var formatProvider = new BirthDateParseProvider(cultureInfo);

            var expected = new BirthDate(2000, 12, 1);
            string stringToParse = "12/1/2000 00";
            string formatString = @"M/d/yyyy yy";

            // Act
            BirthDate parsedBirthDate = BirthDate.ParseExact(stringToParse, new[] { formatString }, formatProvider);

            // Assert
            Assert.AreEqual(expected, parsedBirthDate);
        }

        /// <summary>
        /// Check if <see cref="BirthDate.ParseExact(string, System.Collections.Generic.IList{string}, IFormatProvider)"/> generates the expected
        /// <see cref="BirthDate"/> when there is a short year in the format string. Culture dependent: English.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("Library")]
        [TestCategory("Common")]
        public void BirthDate_ParseExact_Small_Year_Test()
        {
            // Arrange
            var cultureInfo = CultureInfo.GetCultureInfo("en-US");
            var formatProvider = new BirthDateParseProvider(cultureInfo);

            var expected = new BirthDate(2010, 0, 0);
            string stringToParse = "0/0/10";
            string formatString = @"M/d/yy";

            // Act
            BirthDate parsedBirthDate = BirthDate.ParseExact(stringToParse, new[] { formatString }, formatProvider);

            // Assert
            Assert.AreEqual(expected, parsedBirthDate);
        }

        #endregion

        #region IComparable, IComparable<BirthDate>

        /// <summary>
        /// Check if <see cref="BirthDate.CompareTo(BirthDate)"/> works as expected.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("Library")]
        [TestCategory("Common")]
        public void BirthDate_CompareTo_Test()
        {
            // Arrange
            BirthDate birthDate_0, birthDate_0c = birthDate_0 = new BirthDate(2000, 5, 5);
            BirthDate birthDate_1, birthDate_1c = birthDate_1 = new BirthDate(2000, 5, 0);
            BirthDate birthDate_2, birthDate_2c = birthDate_2 = new BirthDate(2000, 0, 0);
            BirthDate birthDate_3, birthDate_3c = birthDate_3 = new BirthDate(0, 0, 0);

            BirthDate birthDate_4 = new BirthDate(2000, 5, 6);
            BirthDate birthDate_5 = new BirthDate(2000, 6, 5);
            BirthDate birthDate_6 = new BirthDate(2001, 5, 5);

            BirthDate birthDate_7 = new BirthDate(2000, 5, 4);
            BirthDate birthDate_8 = new BirthDate(2000, 4, 5);
            BirthDate birthDate_9 = new BirthDate(1999, 5, 5);

            // Act & Assert
            // Equal
            Assert.AreEqual(0, birthDate_0.CompareTo(birthDate_0c));
            Assert.AreEqual(0, birthDate_1.CompareTo(birthDate_1c));
            Assert.AreEqual(0, birthDate_2.CompareTo(birthDate_2c));
            Assert.AreEqual(0, birthDate_3.CompareTo(birthDate_3c));

            Assert.AreEqual(0, birthDate_1.CompareTo(birthDate_0));
            Assert.AreEqual(0, birthDate_2.CompareTo(birthDate_1));
            Assert.AreEqual(0, birthDate_3.CompareTo(birthDate_2));

            Assert.AreEqual(0, birthDate_0.CompareTo(birthDate_1));
            Assert.AreEqual(0, birthDate_1.CompareTo(birthDate_2));
            Assert.AreEqual(0, birthDate_2.CompareTo(birthDate_3));

            // More Then
            Assert.AreEqual(1, birthDate_4.CompareTo(birthDate_0));
            Assert.AreEqual(1, birthDate_5.CompareTo(birthDate_0));
            Assert.AreEqual(1, birthDate_6.CompareTo(birthDate_0));

            // Less Then
            Assert.AreEqual(-1, birthDate_7.CompareTo(birthDate_0));
            Assert.AreEqual(-1, birthDate_8.CompareTo(birthDate_0));
            Assert.AreEqual(-1, birthDate_9.CompareTo(birthDate_0));
        }

        #endregion

        #region IXmlSerializable

        /// <summary>
        /// Check if <see cref="BirthDate.WriteXml(System.Xml.XmlWriter)"/> generates the expected xml.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("Library")]
        [TestCategory("Common")]
        public void BirthDate_Serialize_Test()
        {
            // Arrange
            string expected = GetType().Assembly.GetEmbeddedResourceContent(ResourceKeys.BirthDate_Serialized);
            using (var ms = new MemoryStream())
            {
                XmlSerializer xs = new XmlSerializer(typeof(BirthDate));
                var birthDate = new BirthDate(2000, 1, 1);

                // Act
                xs.Serialize(ms, birthDate);

                // Assert
                ms.Position = 0;
                using (var sr = new StreamReader(ms))
                {
                    var serializedText = sr.ReadToEnd();

                    Assert.AreEqual(expected.Replace("\r",""), serializedText.Replace("\r", ""));
                }
            }
        }

        /// <summary>
        /// Check if <see cref="BirthDate.ReadXml(System.Xml.XmlReader)"/> generates the expected BirthDate.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("Library")]
        [TestCategory("Common")]
        public void BirthDate_Deserialize_Test()
        {
            // Arrange
            string serialized = GetType().Assembly.GetEmbeddedResourceContent(ResourceKeys.BirthDate_Serialized);
            using (var ms = new MemoryStream(Encoding.ASCII.GetBytes(serialized)))
            {
                ms.Position = 0;
                var expected = new BirthDate(2000, 1, 1);
                var xs = new XmlSerializer(typeof(BirthDate));

                // Act
                BirthDate birthDate = (BirthDate)xs.Deserialize(ms);

                // Assert
                Assert.AreEqual(expected, birthDate);
                Assert.IsFalse(birthDate.IsEmpty);
            }
        }

        /// <summary>
        /// Check if <see cref="BirthDate.ReadXml(System.Xml.XmlReader)"/> generates the expected empty BirthDate.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("Library")]
        [TestCategory("Common")]
        public void BirthDate_Deserialize_Empty_Test()
        {
            // Arrange
            string serialized = GetType().Assembly.GetEmbeddedResourceContent(ResourceKeys.BirthDate_Serialized_Empty);
            using (var ms = new MemoryStream(Encoding.ASCII.GetBytes(serialized)))
            {
                ms.Position = 0;
                var expected = new BirthDate();
                var xs = new XmlSerializer(typeof(BirthDate));

                // Act
                BirthDate birthDate = (BirthDate)xs.Deserialize(ms);

                // Assert
                Assert.AreEqual(expected, birthDate);
                Assert.IsTrue(birthDate.IsEmpty);
            }
        }

        /// <summary>
        /// Tests if the provided xml schema is what we expect.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("Library")]
        [TestCategory("Common")]
        public void BirthDate_CompliesToProvidedSchema()
        {
            // Arrange
            string serialized = GetType().Assembly.GetEmbeddedResourceContent(ResourceKeys.BirthDate_Serialized);
            var schemaSet = new XmlSchemaSet();
            var expected = new BirthDate(2000, 1, 1);
            var birthDate = new BirthDate();

            BirthDate.AddXmlSchema(schemaSet);
            var settings = new XmlReaderSettings() {
                Schemas = schemaSet,
                ValidationType = ValidationType.Schema
            };
            settings.ValidationEventHandler += new ValidationEventHandler((s, e) => {
                Assert.Fail(e.Message);
            });

            // Act
            using (var reader = new StringReader(serialized))
            {
                using (var xmlReader = XmlReader.Create(reader, settings))
                {
                    birthDate.ReadXml(xmlReader);
                }
            }

            // Assert
            Assert.AreEqual(expected, birthDate);
        }

        /// <summary>
        /// Tests if the provided xml schema is what we expect.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(XmlSchemaValidationException))]
        [TestCategory("UnitTest")]
        [TestCategory("Library")]
        [TestCategory("Common")]
        public void BirthDate_DoesNotComplyToProvidedSchema_UnknownChildElement()
        {
            // Arrange
            string serialized = GetType().Assembly.GetEmbeddedResourceContent(ResourceKeys.BirthDate_Serialized_UnknownChildElement);
            var schemaSet = new XmlSchemaSet();
            var birthDate = new BirthDate();
            ValidationEventArgs eventArgs = null;

            BirthDate.AddXmlSchema(schemaSet);
            var settings = new XmlReaderSettings()
            {
                Schemas = schemaSet,
                ValidationType = ValidationType.Schema
            };
            settings.ValidationEventHandler += new ValidationEventHandler((s, e) => {
                eventArgs = e;
            });

            // Act
            try
            {
                using (var reader = new StringReader(serialized))
                {
                    using (var xmlReader = XmlReader.Create(reader, settings))
                    {
                        birthDate.ReadXml(xmlReader);
                    }
                }
            }

            // Assert
            finally
            {
                Assert.IsTrue(eventArgs != null);
                Assert.IsNotNull(eventArgs.Message);
                Assert.AreEqual(
                    "The element 'BirthDate' in namespace 'Coconut.Library.Common' has invalid child element 'IsEmpty2' in namespace 'Coconut.Library.Common'. List of possible elements expected: 'IsEmpty' in namespace 'Coconut.Library.Common'.",
                    eventArgs.Message
                );
            }
        }

        /// <summary>
        /// Tests if the provided xml schema is what we expect.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(XmlSchemaValidationException))]
        [TestCategory("UnitTest")]
        [TestCategory("Library")]
        [TestCategory("Common")]
        public void BirthDate_DoesNotComplyToProvidedSchema_MissingChildElement()
        {
            // Arrange
            string serialized = GetType().Assembly.GetEmbeddedResourceContent(ResourceKeys.BirthDate_Serialized_MissingChildElement);
            var schemaSet = new XmlSchemaSet();
            var birthDate = new BirthDate();
            ValidationEventArgs eventArgs = null;

            BirthDate.AddXmlSchema(schemaSet);
            var settings = new XmlReaderSettings()
            {
                Schemas = schemaSet,
                ValidationType = ValidationType.Schema
            };
            settings.ValidationEventHandler += new ValidationEventHandler((s, e) => {
                eventArgs = e;
            });

            // Act
            try
            {
                using (var reader = new StringReader(serialized))
                {
                    using (var xmlReader = XmlReader.Create(reader, settings))
                    {
                        birthDate.ReadXml(xmlReader);
                    }
                }
            }

            // Assert
            finally
            {
                Assert.IsTrue(eventArgs != null);
                Assert.IsNotNull(eventArgs.Message);
                Assert.AreEqual(
                    "The element 'BirthDate' in namespace 'Coconut.Library.Common' has incomplete content. List of possible elements expected: 'IsEmpty' in namespace 'Coconut.Library.Common'.",
                    eventArgs.Message
                );
            }
        }

        #endregion

        #region Operators

        /// <summary>
        /// Check if the operators on <see cref="BirthDate"/> function properly.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("Library")]
        [TestCategory("Common")]
        public void BirthDate_Operator_Test()
        {
            // Arrange
            var big = new BirthDate(2000, 12, 30);
            var bigsame = big;
            var small = new BirthDate(1000, 01, 01);
            var smallsame = small;

            // Act && Assert
            Assert.IsTrue(big == bigsame);
            Assert.IsTrue(big != small);

            Assert.IsTrue(big > small);
            Assert.IsTrue(big >= bigsame);

            Assert.IsTrue(small < big);
            Assert.IsTrue(small <= smallsame);
        }

        /// <summary>
        /// Check if the operators on <see cref="BirthDate"/> function properly. (Extended)
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("Library")]
        [TestCategory("Common")]
        public void BirthDate_Operator_ExtendedTest()
        {
            // Arrange
            BirthDate birthDate_0, birthDate_0c = birthDate_0 = new BirthDate(2000, 5, 5);
            BirthDate birthDate_1, birthDate_1c = birthDate_1 = new BirthDate(2000, 5, 0);
            BirthDate birthDate_2, birthDate_2c = birthDate_2 = new BirthDate(2000, 0, 0);
            BirthDate birthDate_3, birthDate_3c = birthDate_3 = new BirthDate(0, 0, 0);

            BirthDate birthDate_4 = new BirthDate(2000, 5, 6);
            BirthDate birthDate_5 = new BirthDate(2000, 6, 5);
            BirthDate birthDate_6 = new BirthDate(2001, 5, 5);

            BirthDate birthDate_7 = new BirthDate(2000, 5, 4);
            BirthDate birthDate_8 = new BirthDate(2000, 4, 5);
            BirthDate birthDate_9 = new BirthDate(1999, 5, 5);

            // Act && Assert
            Assert.IsTrue(birthDate_0 == birthDate_0c);
            Assert.IsTrue(birthDate_1 == birthDate_1c);
            Assert.IsTrue(birthDate_2 == birthDate_2c);
            Assert.IsTrue(birthDate_3 == birthDate_3c);

            Assert.IsTrue(birthDate_1 == birthDate_0);
            Assert.IsTrue(birthDate_2 == birthDate_1);
            Assert.IsTrue(birthDate_3 == birthDate_2);

            Assert.IsTrue(birthDate_0 == birthDate_1);
            Assert.IsTrue(birthDate_1 == birthDate_2);
            Assert.IsTrue(birthDate_2 == birthDate_3);

            Assert.IsTrue(birthDate_0 >= birthDate_0c);
            Assert.IsTrue(birthDate_1 >= birthDate_1c);
            Assert.IsTrue(birthDate_2 >= birthDate_2c);
            Assert.IsTrue(birthDate_3 >= birthDate_3c);

            Assert.IsTrue(birthDate_4 >= birthDate_0);
            Assert.IsTrue(birthDate_5 >= birthDate_0);
            Assert.IsTrue(birthDate_6 >= birthDate_0);

            Assert.IsTrue(birthDate_0 <= birthDate_0c);
            Assert.IsTrue(birthDate_1 <= birthDate_1c);
            Assert.IsTrue(birthDate_2 <= birthDate_2c);
            Assert.IsTrue(birthDate_3 <= birthDate_3c);

            Assert.IsTrue(birthDate_7 <= birthDate_0);
            Assert.IsTrue(birthDate_8 <= birthDate_0);
            Assert.IsTrue(birthDate_9 <= birthDate_0);

            Assert.IsTrue(birthDate_4 > birthDate_0);
            Assert.IsTrue(birthDate_5 > birthDate_0);
            Assert.IsTrue(birthDate_6 > birthDate_0);

            Assert.IsTrue(birthDate_7 < birthDate_0);
            Assert.IsTrue(birthDate_8 < birthDate_0);
            Assert.IsTrue(birthDate_9 < birthDate_0);
        }

        #endregion
    }
}
