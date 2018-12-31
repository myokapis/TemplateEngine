/* ****************************************************************************
Copyright 2018 Gene Graves

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

   http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
**************************************************************************** */

using System;
using System.Globalization;

namespace TemplateEngine.Formats
{

    /// <summary>
    /// Abstract class for building format attributes that can be applied to properties to
    /// define the way the property data should be represented as text.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public abstract class FormatAttribute : Attribute
    {
        /// <summary>
        /// Abstract method to convert a value into an appropriately formatted string
        /// </summary>
        /// <param name="data">Value to be formatted</param>
        /// <returns>Data formatted as a string</returns>
        public abstract string FormatData(object data);

        /// <summary>
        /// Shorthand reference to the current culture
        /// </summary>
        public CultureInfo Culture => CultureInfo.CurrentCulture;
    }

    /// <summary>
    /// Attribute for formatting property data as a currency string.
    /// </summary>
    public class FormatCurrencyAttribute : FormatAttribute
    {

        /// <summary>
        /// Currency formatter constructor that accepts format parameters
        /// </summary>
        /// <param name="decimalPlaces">Number of decimal places to include in the result</param>
        /// <param name="negativePattern">Pattern to be used for representing a negative value</param>
        /// <param name="groupSeparator">Character to be used to delimit thousand groups</param>
        /// <param name="decimalSeparator">Character to be used as a decimal separator</param>
        public FormatCurrencyAttribute(int? decimalPlaces = null, int? negativePattern = null,
            string groupSeparator = null, string decimalSeparator = null)
        {
            this.FormatInfo = (NumberFormatInfo)this.Culture.NumberFormat.Clone();
            this.FormatString = "C";

            if (decimalPlaces.HasValue)
            {
                // override number of decimal places shown
                if (decimalPlaces.Value < 0) throw new ArgumentException("Number of decimal places must be greater than zero");
                this.FormatInfo.CurrencyDecimalDigits = decimalPlaces.Value;
            }

            // set format attributes
            if (negativePattern.HasValue) this.FormatInfo.CurrencyNegativePattern = negativePattern.Value;
            if (groupSeparator != null) this.FormatInfo.CurrencyGroupSeparator = groupSeparator;
            if (decimalSeparator != null) this.FormatInfo.CurrencyDecimalSeparator = decimalSeparator;
        }

        /// <summary>
        /// Currency format constructor that accepts a format string
        /// </summary>
        /// <param name="formatString">Currency format string</param>
        public FormatCurrencyAttribute(string formatString)
        {
            this.FormatString = formatString;
        }

        /// <summary>
        /// Currency format constructor that accepts a number formatter
        /// </summary>
        /// <param name="formatInfo">A <cref="NumberFormatInfo" /> to be used for formatting</param>
        public FormatCurrencyAttribute(NumberFormatInfo formatInfo)
        {
            this.FormatInfo = formatInfo;
            this.FormatString = "C";
        }

        private NumberFormatInfo FormatInfo { get; }

        private string FormatString { get; }

        /// <summary>
        /// Formats an object using the elements provided to this formatter's constructor
        /// </summary>
        /// <param name="data">Data to be formatted</param>
        /// <returns>Object data formatted as a currency string</returns>
        public override string FormatData(object data)
        {
            if (data == null) return "";

            if (double.TryParse(data.ToString(), out var dbl))
            {
                return dbl.ToString(this.FormatString, this.FormatInfo);
            }

            return "";
        }

    }

    /// <summary>
    /// Attribute for formatting property data as a date string.
    /// </summary>
    public class FormatDateAttribute : FormatAttribute
    {

        /// <summary>
        /// Date format constructor that accepts a format string
        /// </summary>
        /// <param name="formatString">Date format string</param>
        public FormatDateAttribute(string formatString)
        {
            this.FormatString = formatString ?? "G";
        }

        /// <summary>
        /// Date format constructor that accepts a date formatter
        /// </summary>
        /// <param name="formatInfo">A <cref="DateTimeFormatInfo" /> to be used for formatting</param>
        public FormatDateAttribute(DateTimeFormatInfo formatInfo)
        {
            this.FormatInfo = formatInfo;
            this.FormatString = null;
        }

        private DateTimeFormatInfo FormatInfo { get; }

        private string FormatString { get; }

        /// <summary>
        /// Formats an object using the elements provided to this formatter's constructor
        /// </summary>
        /// <param name="data">Data to be formatted</param>
        /// <returns>Object data formatted as a date string</returns>
        public override string FormatData(object data)
        {
            if (data == null) return "";

            if(DateTime.TryParse(data.ToString(), out var date))
            {
                return date.ToString(this.FormatString, this.FormatInfo);
            }

            return "";
        }

    }

    /// <summary>
    /// Attribute for formatting property data as an integer string.
    /// </summary>
    public class FormatIntegerAttribute : FormatAttribute
    {

        /// <summary>
        /// Integer formatter constructor that accepts format parameters
        /// </summary>
        /// <param name="negativePattern">Pattern to be used for representing a negative value</param>
        /// <param name="groupSeparator">Character to be used to delimit thousand groups</param>
        public FormatIntegerAttribute(int? negativePattern = null, string groupSeparator = null)
        {
            this.FormatInfo = (NumberFormatInfo)this.Culture.NumberFormat.Clone();
            this.FormatString = "N0";

            // use a different negative pattern
            if (negativePattern.HasValue) this.FormatInfo.NumberNegativePattern = negativePattern.Value;

            // set the group separators
            if (groupSeparator != null) this.FormatInfo.NumberGroupSeparator = groupSeparator;
        }

        /// <summary>
        /// Integer format constructor that accepts a format string
        /// </summary>
        /// <param name="formatString">Integer format string</param>
        public FormatIntegerAttribute(string formatString)
        {
            this.FormatString = formatString;
        }

        /// <summary>
        /// Integer format constructor that accepts a number formatter
        /// </summary>
        /// <param name="formatInfo">A <cref="NumberFormatInfo" /> to be used for formatting</param>
        public FormatIntegerAttribute(NumberFormatInfo formatInfo)
        {
            this.FormatInfo = formatInfo;
            this.FormatString = "N0";
        }

        private NumberFormatInfo FormatInfo { get; }

        private string FormatString { get; }

        /// <summary>
        /// Formats an object using the elements provided to this formatter's constructor
        /// </summary>
        /// <param name="data">Data to be formatted</param>
        /// <returns>Object data formatted as an integer string</returns>
        public override string FormatData(object data)
        {
            if (data == null) return "";

            if (double.TryParse(data.ToString(), out var dbl))
            {
                return dbl.ToString(this.FormatString, this.FormatInfo);
            }

            return "";
        }

    }

    /// <summary>
    /// Attribute for formatting property data as a numeric string.
    /// </summary>
    public class FormatNumberAttribute : FormatAttribute
    {

        /// <summary>
        /// Numeric formatter constructor that accepts format parameters
        /// </summary>
        /// <param name="decimalPlaces">Number of decimal places to include in the result</param>
        /// <param name="negativePattern">Pattern to be used for representing a negative value</param>
        /// <param name="groupSeparator">Character to be used to delimit thousand groups</param>
        /// <param name="decimalSeparator">Character to be used as a decimal separator</param>
        public FormatNumberAttribute(int? decimalPlaces = null, int? negativePattern = null,
            string groupSeparator = null, string decimalSeparator = null)
        {
            this.FormatInfo = (NumberFormatInfo)this.Culture.NumberFormat.Clone();
            this.FormatString = "N";

            if (decimalPlaces.HasValue)
            {
                // override number of decimal places shown
                if (decimalPlaces.Value < 0) throw new ArgumentException("Number of decimal places must be greater than zero");
                this.FormatInfo.NumberDecimalDigits = decimalPlaces.Value;
            }

            // change the negative sign pattern
            if (negativePattern.HasValue) this.FormatInfo.NumberNegativePattern = negativePattern.Value;

            // change the group separator
            if (groupSeparator != null) this.FormatInfo.NumberGroupSeparator = groupSeparator;

            // change the decimal separator
            if (decimalSeparator != null) this.FormatInfo.NumberDecimalSeparator = decimalSeparator;
        }

        /// <summary>
        /// Numeric format constructor that accepts a format string
        /// </summary>
        /// <param name="formatString">Numeric format string</param>
        public FormatNumberAttribute(string formatString)
        {
            this.FormatString = formatString;
        }

        /// <summary>
        /// Numeric format constructor that accepts a number formatter
        /// </summary>
        /// <param name="formatInfo">A <cref="NumberFormatInfo" /> to be used for formatting</param>
        public FormatNumberAttribute(NumberFormatInfo formatInfo)
        {
            this.FormatInfo = formatInfo;
            this.FormatString = "N";
        }

        private NumberFormatInfo FormatInfo { get; }

        private string FormatString { get; set; }

        /// <summary>
        /// Formats an object using the elements provided to this formatter's constructor
        /// </summary>
        /// <param name="data">Data to be formatted</param>
        /// <returns>Object data formatted as a numeric string</returns>
        public override string FormatData(object data)
        {
            if (data == null) return "";

            if(double.TryParse(data.ToString(), out var dbl))
            {
                return dbl.ToString(this.FormatString, this.FormatInfo);
            }

            return "";
        }

    }

    /// <summary>
    /// Attribute for formatting property data as a percentage string.
    /// </summary>
    public class FormatPercentAttribute : FormatAttribute
    {

        /// <summary>
        /// Percentage formatter constructor that accepts format parameters
        /// </summary>
        /// <param name="decimalPlaces">Number of decimal places to include in the result</param>
        /// <param name="negativePattern">Pattern to be used for representing a negative value</param>
        /// <param name="groupSeparator">Character to be used to delimit thousand groups</param>
        /// <param name="decimalSeparator">Character to be used as a decimal separator</param>
        public FormatPercentAttribute(int? decimalPlaces = null, int? negativePattern = null,
            string groupSeparator = null, string decimalSeparator = null)
        {
            this.FormatInfo = (NumberFormatInfo)this.Culture.NumberFormat.Clone();
            this.FormatString = "P";

            if (decimalPlaces.HasValue)
            {
                // override number of decimal places shown
                if (decimalPlaces.Value < 0) throw new ArgumentException("Number of decimal places must be greater than zero");
                this.FormatInfo.PercentDecimalDigits = decimalPlaces.Value;
            }

            // change the negative number format
            if (negativePattern.HasValue) this.FormatInfo.PercentNegativePattern = negativePattern.Value;

            // change the group separator
            if (groupSeparator != null) this.FormatInfo.PercentGroupSeparator = groupSeparator;

            // change the decimal separator
            if (decimalSeparator != null) this.FormatInfo.PercentDecimalSeparator = decimalSeparator;
        }

        /// <summary>
        /// Percentage format constructor that accepts a format string
        /// </summary>
        /// <param name="formatString">Percentage format string</param>
        public FormatPercentAttribute(string formatString)
        {
            this.FormatString = formatString;
        }

        /// <summary>
        /// Percentage format constructor that accepts a number formatter
        /// </summary>
        /// <param name="formatInfo">A <cref="NumberFormatInfo" /> to be used for formatting</param>
        public FormatPercentAttribute(NumberFormatInfo formatInfo, string formatString = null)
        {
            this.FormatInfo = formatInfo;
            this.FormatString = "P";
        }

        private NumberFormatInfo FormatInfo { get; }

        private string FormatString { get; }

        /// <summary>
        /// Formats an object using the elements provided to this formatter's constructor
        /// </summary>
        /// <param name="data">Data to be formatted</param>
        /// <returns>Object data formatted as a percentage string</returns>
        public override string FormatData(object data)
        {
            if (data == null) return "";

            if (double.TryParse(data.ToString(), out var dbl))
            {
                return dbl.ToString(this.FormatString, this.FormatInfo);
            }

            return "";
        }

    }

}
