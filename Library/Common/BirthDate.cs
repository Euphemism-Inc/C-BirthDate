// (c) Euphemism Inc. All right reserved.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Coconut.Library.Common.Extensions;
using Coconut.Library.Common.Format;
using Coconut.Library.Common.Parse;
using Coconut.Library.Common.Resources;

namespace Coconut.Library.Common
{
    /// <summary>
    /// This class represents a BirthDay. Sometimes a person does not know exactly when (s)he is born, on what date. Therefore a DateTime can be too precise.
    /// </summary>
    [Serializable]
    [XmlSchemaProvider(nameof(BirthDate.AddXmlSchema))]
    [XmlRoot(elementName:nameof(BirthDate), DataType=nameof(BirthDate), Namespace=xmlSchemaNamespace)]
    public struct BirthDate : IComparable, IComparable<BirthDate>, IXmlSerializable
    {
        private const string xmlSchemaNamespace = nameof(Coconut) + "." + nameof(Library) + "." + nameof(Common);

        private static readonly string notInitializedException = "Object not initialized.";
        private static readonly Lazy<string> xmlSchemaString = new Lazy<string>(XmlSchemaString, isThreadSafe:true);
        private static readonly Lazy<XmlSchema> xmlSchema = new Lazy<XmlSchema>(XmlSchema, isThreadSafe:true);

        private bool _isEmptyInverse; // inverse backing field for IsEmpty
        private DateTime _date;

        /// <summary>
        /// Gets a value indicating whether year is an actual year.
        /// </summary>
        public bool YearIsReal { get; private set; }

        /// <summary>
        /// Gets a value indicating whether month is an actual month.
        /// </summary>
        public bool MonthIsReal { get; private set; }

        /// <summary>
        /// Gets a value indicating whether day is an actual day.
        /// </summary>
        public bool DayIsReal { get; private set; }

        /// <summary>
        /// The year in which the person is born.
        /// </summary>
        public int Year { get { return YearIsReal ? _date.Year : 0; } }

        /// <summary>
        /// The month in which the person is born.
        /// </summary>
        public int Month { get { return MonthIsReal ? _date.Month : 0; } }

        /// <summary>
        /// The day on which the person is born.
        /// </summary>
        public int Day { get { return DayIsReal ? _date.Day : 0; } }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is empty.
        /// </summary>
        public bool IsEmpty
        {
            get { return !_isEmptyInverse; }
            private set { _isEmptyInverse = !value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BirthDate"/> class.
        /// </summary>
        /// <param name="year">The year.</param>
        /// <param name="month">The month.</param>
        /// <param name="day">The day.</param>
        public BirthDate(short year, short month, short day) : this()
        {
            if (year < 0) throw new ArgumentException(nameof(year));
            if (month < 0 || (month > 0 && year == 0)) throw new ArgumentException(nameof(month));
            if (day < 0 || (day > 0 && month == 0)) throw new ArgumentException(nameof(day));

            YearIsReal = year > 0;
            MonthIsReal = month > 0 && YearIsReal;
            DayIsReal = day > 0 && MonthIsReal;
            IsEmpty = false;

            _date = new DateTime(
                YearIsReal ? year : 1,
                MonthIsReal ? month : 1,
                DayIsReal ? day : 1
            );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BirthDate"/> class.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <param name="mask">The mask on the <see cref="BirthDate"/>.</param>
        public BirthDate(
            DateTime date,
            BirthDateMaskEnum? mask = null
        ) : this()
        {
            if (!mask.HasValue) mask = BirthDateMaskEnum.NoMask;
            if (!mask.Value.IsValidMask()) throw new ArgumentException(nameof(mask));

            _date = date;
            DayIsReal = (mask & BirthDateMaskEnum.Day) == 0;
            MonthIsReal = (mask & BirthDateMaskEnum.Month) == 0;
            YearIsReal = (mask & BirthDateMaskEnum.Year) == 0;
            IsEmpty = false;
        }

        /// <summary>
        /// Gets date time without mask.
        /// </summary>
        public DateTime GetDateTimeWithoutMask()
        {
            if (IsEmpty) throw new InvalidOperationException(notInitializedException);
            return GetDateTimeWithoutMaskInternal();
        }

        /// <summary>
        /// Gets date time without mask.
        /// </summary>
        internal DateTime GetDateTimeWithoutMaskInternal()
        {
            return new DateTime(
                YearIsReal ? Year : 1,
                MonthIsReal ? Month : 1,
                DayIsReal ? Day : 1
            );
        }

        /// <summary>
        /// Gets the birth date mask.
        /// </summary>
        public BirthDateMaskEnum GetBirthDateMask()
        {
            if (IsEmpty) throw new InvalidOperationException(notInitializedException);
            return GetBirthDateMaskInternal();
        }

        /// <summary>
        /// Gets the birth date mask.
        /// </summary>
        internal BirthDateMaskEnum GetBirthDateMaskInternal()
        {
            return BirthDateMaskEnum.NoMask |
                (DayIsReal ? 0 : BirthDateMaskEnum.Day) |
                (MonthIsReal ? 0 : BirthDateMaskEnum.Month) |
                (YearIsReal ? 0 : BirthDateMaskEnum.Year);
        }

        #region ToString

        /// <summary>
        /// Returns a <see cref="String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return ToString(null, new BirthDateFormatProvider(this));
        }

        /// <summary>
        /// Returns a <see cref="String" /> that represents this instance.
        /// </summary>
        /// <param name="format"></param>
        /// <returns>
        /// A <see cref="String" /> that represents this instance.
        /// </returns>
        public string ToString(string format)
        {
            return ToString(format, new BirthDateFormatProvider(this));
        }

        /// <summary>
        /// Returns a <see cref="String" /> that represents this instance.
        /// </summary>
        /// <param name="format"></param>
        /// <param name="cultureInfo">The culture info.</param>
        /// <returns>
        /// A <see cref="String" /> that represents this instance.
        /// </returns>
        public string ToString(string format, CultureInfo cultureInfo)
        {
            return ToString(format, new BirthDateFormatProvider(this, cultureInfo));
        }

        /// <summary>
        /// Returns a <see cref="String" /> that represents this instance.
        /// </summary>
        /// <param name="formatProvider">The format provider.</param>
        /// <returns>
        /// A <see cref="String" /> that represents this instance.
        /// </returns>
        public string ToString(IFormatProvider formatProvider)
        {
            return ToString(null, formatProvider);
        }

        /// <summary>
        /// Returns a <see cref="String" /> that represents this instance.
        /// </summary>
        /// <param name="format"></param>
        /// <param name="formatProvider">The format provider.</param>
        /// <returns>
        /// A <see cref="String" /> that represents this instance.
        /// </returns>
        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (formatProvider == null) throw new ArgumentNullException(nameof(formatProvider));

            if (BirthDateFormatInfo.TryGetInstance(formatProvider, out BirthDateFormatInfo birthDateFormatInfo))
            {
                var birthDateFormatter = new BirthDateFormat(format, birthDateFormatInfo);
                return birthDateFormatter.Format(this);
            }
            else
            {
                return _date.ToString(format, formatProvider);
            }
        }

        #endregion

        #region Parse

        /// <summary>
        /// Parses the stirng.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <returns></returns>
        public static BirthDate ParseExact(string str)
        {
            return ParseExact(str, new BirthDateParseProvider());
        }

        /// <summary>
        /// Parses the string using the specified <see cref="CultureInfo"/>.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="cultureInfo">The culture information.</param>
        /// <returns></returns>
        public static BirthDate ParseExact(string str, CultureInfo cultureInfo)
        {
            var formatProvider = new BirthDateParseProvider(cultureInfo);
            return ParseExact(str, formatProvider);
        }

        /// <summary>
        /// Parses the string exactly according to prespecified default format.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// <returns></returns>
        public static BirthDate ParseExact(string str, IFormatProvider formatProvider)
        {
            return ParseExact(str, null, formatProvider);
        }

        /// <summary>
        /// Parses the string exactly according to prespecified formats.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="formats">The formats.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// <returns></returns>
        public static BirthDate ParseExact(string str, IList<string> formats, IFormatProvider formatProvider)
        {
            if (BirthDateParseInfo.TryGetInstance(formatProvider, out BirthDateParseInfo birthDateParseInfo))
            {
                var birthDateParser = new BirthDateParse(birthDateParseInfo, formats);
                return birthDateParser.ParseExact(str);
            }
            else if (formats == null)
            {
                var dateTime = DateTime.Parse(str, formatProvider);
                return new BirthDate(dateTime, BirthDateMaskEnum.NoMask);
            }
            else
            {
                var dateTime = DateTime.ParseExact(str, formats.ToArray(), formatProvider, DateTimeStyles.None);
                return new BirthDate(dateTime, BirthDateMaskEnum.NoMask);
            }
        }

        #endregion

        #region IComparable Members

        /// <summary>
        /// Compares the current instance with another object of the same type and returns an integer that indicates whether
        /// the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
        /// </summary>
        /// <param name="other">An object to compare with this instance.</param>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared.
        /// The return value has these meanings:
        /// Value Meaning
        ///   Less than zero
        ///     This instance precedes <paramref name="other" /> in the sort order.
        ///   Zero
        ///     This instance occurs in the same position in the sort order as <paramref name="other" />.
        ///   Greater than zero
        ///     This instance follows <paramref name="other" /> in the sort order.
        /// </returns>
        public int CompareTo(BirthDate other)
        {
            int comparison = IsEmpty && other.IsEmpty ? 0 : ~0;
            if (comparison != 0)
            {
                comparison = (YearIsReal && other.YearIsReal) ? Year.CompareTo(other.Year) : 0;
                if (comparison == 0)
                {
                    comparison = (MonthIsReal && other.MonthIsReal) ? Month.CompareTo(other.Month) : 0;
                    if (comparison == 0)
                    {
                        comparison = (DayIsReal && other.DayIsReal) ? Day.CompareTo(other.Day) : 0;
                    }
                }
            }

            return comparison;
        }

        /// <summary>
        /// Compares the current instance with another object of the same type and returns an integer that indicates whether
        /// the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
        /// </summary>
        /// <param name="obj">An object to compare with this instance.</param>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value has these meanings:
        /// Value Meaning
        ///   Less than zero
        ///     This instance precedes <paramref name="obj" /> in the sort order.
        ///   Zero
        ///     This instance occurs in the same position in the sort order as <paramref name="obj" />.
        ///   Greater than zero
        ///     This instance follows <paramref name="obj" /> in the sort order.
        /// </returns>
        public int CompareTo(object obj)
        {
            if (!(obj is BirthDate other)) return 1;
            return CompareTo(other);
        }

        #endregion

        #region IXmlSerializable Members

        /// <summary>
        /// This method is reserved and should not be used. When implementing the IXmlSerializable interface, you should return null
        /// (Nothing in Visual Basic) from this method, and instead, if specifying a custom schema is required, apply the
        /// <see cref="T:System.Xml.Serialization.XmlSchemaProviderAttribute" /> to the class.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Xml.Schema.XmlSchema" /> that describes the XML representation of the object that is produced by
        /// the <see cref="M:System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter)" /> method and consumed by the
        /// <see cref="M:System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader)" /> method.
        /// </returns>
        public XmlSchema GetSchema()
        {
            return null;
        }

        /// <summary>
        /// Generates an object from its XML representation.
        /// </summary>
        /// <param name="reader">The <see cref="XmlReader" /> stream from which the object is deserialized.</param>
        /// <exception cref="XmlSchemaValidationException">Thrown when the XML in the reader does not comply with the expected <see cref="BirthDate"/> XSD.</exception>
        public void ReadXml(XmlReader reader)
        {
            if (reader.IsEmptyElement)
            {
                return;
            }

            var settings = new XmlReaderSettings()
            {
                Schemas = new XmlSchemaSet(),
                ValidationType = ValidationType.Schema
            };
            AddXmlSchema(settings.Schemas);

            var validatingReader = XmlReader.Create(reader, settings);
            validatingReader.ReadStartElement(nameof(BirthDate));
            validatingReader.Read();

            validatingReader.ReadStartElement(nameof(DateTime));
            _date = validatingReader.ReadContentAsDateTime();
            validatingReader.ReadEndElement();

            validatingReader.ReadStartElement(nameof(YearIsReal));
            YearIsReal = validatingReader.ReadContentAsBoolean();
            validatingReader.ReadEndElement();

            validatingReader.ReadStartElement(nameof(MonthIsReal));
            MonthIsReal = validatingReader.ReadContentAsBoolean();
            validatingReader.ReadEndElement();

            validatingReader.ReadStartElement(nameof(DayIsReal));
            DayIsReal = validatingReader.ReadContentAsBoolean();
            validatingReader.ReadEndElement();

            validatingReader.ReadStartElement(nameof(IsEmpty));
            IsEmpty = validatingReader.ReadContentAsBoolean();
            validatingReader.ReadEndElement();

            validatingReader.ReadEndElement();
        }

        /// <summary>
        /// Converts an object into its XML representation.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Xml.XmlWriter" /> stream to which the object is serialized.</param>
        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement(nameof(DateTime));
            writer.WriteValue(_date);
            writer.WriteEndElement();

            writer.WriteStartElement(nameof(YearIsReal));
            writer.WriteValue(YearIsReal);
            writer.WriteEndElement();

            writer.WriteStartElement(nameof(MonthIsReal));
            writer.WriteValue(MonthIsReal);
            writer.WriteEndElement();

            writer.WriteStartElement(nameof(DayIsReal));
            writer.WriteValue(DayIsReal);
            writer.WriteEndElement();

            writer.WriteStartElement(nameof(IsEmpty));
            writer.WriteValue(IsEmpty);
            writer.WriteEndElement();
        }

        /// <summary>
        /// Xml schema for <see cref="BirthDate"/>.
        /// </summary>
        /// <param name="schemaSet"></param>
        /// <returns></returns>
        public static XmlQualifiedName AddXmlSchema(XmlSchemaSet schemaSet)
        {
            schemaSet.Add(xmlSchema.Value);

            return new XmlQualifiedName(nameof(BirthDate), xmlSchemaNamespace);
        }

        /// <summary>
        /// The XML schema string.
        /// </summary>
        /// <returns></returns>
        private static string XmlSchemaString()
        {
            var rawXmlSchemaString = typeof(BirthDate).Assembly.GetEmbeddedResourceContent(ResourceKeys.BirthDate_XmlScheme);

            return String.Format(rawXmlSchemaString, xmlSchemaNamespace, nameof(BirthDate));
        }

        /// <summary>
        /// The xml schema string.
        /// </summary>
        /// <returns></returns>
        private static XmlSchema XmlSchema()
        {
            return System.Xml.Schema.XmlSchema.Read(new StringReader(xmlSchemaString.Value), validationEventHandler: null);
        }

        #endregion

        #region Object Members

        /// <summary>
        /// Determines whether the specified <see cref="BirthDate" />, is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="BirthDate" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="BirthDate" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(BirthDate other)
        {
            return CompareTo(other) == 0;
        }

        /// <summary>
        /// Determines whether the specified <see cref="Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            return CompareTo(obj) == 0;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            var hashCode = -990088782;
            hashCode = hashCode * -1521134295 + base.GetHashCode();
            hashCode = hashCode * -1521134295 + _date.GetHashCode();
            hashCode = hashCode * -1521134295 + YearIsReal.GetHashCode();
            hashCode = hashCode * -1521134295 + MonthIsReal.GetHashCode();
            hashCode = hashCode * -1521134295 + DayIsReal.GetHashCode();
            hashCode = hashCode * -1521134295 + IsEmpty.GetHashCode();
            return hashCode;
        }

        #endregion

        #region Operators

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="birthDate">The BirthDate.</param>
        /// <param name="other">The other BirthDate.</param>
        public static bool operator ==(BirthDate birthDate, BirthDate other)
        {
            return birthDate.CompareTo(other) == 0;
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="birthDate">The BirthDate.</param>
        /// <param name="other">The other BirthDate.</param>
        public static bool operator !=(BirthDate birthDate, BirthDate other)
        {
            return birthDate.CompareTo(other) != 0;
        }

        /// <summary>
        /// Implements the operator &gt;.
        /// </summary>
        /// <param name="birthDate">The birth date.</param>
        /// <param name="other">The other.</param>
        public static bool operator >(BirthDate birthDate, BirthDate other)
        {
            return birthDate.CompareTo(other) > 0;
        }

        /// <summary>
        /// Implements the operator &gt;=.
        /// </summary>
        /// <param name="birthDate">The birth date.</param>
        /// <param name="other">The other.</param>
        public static bool operator >=(BirthDate birthDate, BirthDate other)
        {
            return birthDate.CompareTo(other) >= 0;
        }

        /// <summary>
        /// Implements the operator &lt;.
        /// </summary>
        /// <param name="birthDate">The birth date.</param>
        /// <param name="other">The other.</param>
        public static bool operator <(BirthDate birthDate, BirthDate other)
        {
            return birthDate.CompareTo(other) < 0;
        }

        /// <summary>
        /// Implements the operator &lt;=.
        /// </summary>
        /// <param name="birthDate">The birth date.</param>
        /// <param name="other">The other.</param>
        public static bool operator <=(BirthDate birthDate, BirthDate other)
        {
            return birthDate.CompareTo(other) <= 0;
        }

        #endregion
    }
}
