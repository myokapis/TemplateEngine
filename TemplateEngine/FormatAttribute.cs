using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// TODO: move these into an assembly/namespace that isn't tied to template engine

namespace TemplateEngine.Formats
{

    // TODO: spruce these up to ensure localization works

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public abstract class FormatAttribute : Attribute
    {
        public abstract string FormatData(object data);
    }

    public class FormatCurrencyAttribute : FormatAttribute
    {

        public FormatCurrencyAttribute(int decimalPlaces, bool useSeparator = false)
        {
            if (decimalPlaces< 1) throw new ArgumentException("At least one decimal place is required");

            this.FormatString = string.Format("${0}0.{1}", useSeparator? "#,##" : "",
                new string ('0', decimalPlaces));
        }

        private string FormatString { get; set; }

        public override string FormatData(object data)
        {
            if (data == null) return "";
            double dbl;

            if (double.TryParse(data.ToString(), out dbl))
            {
                return dbl.ToString(this.FormatString);
            }

            return "";
        }

}

    public class FormatDateAttribute : FormatAttribute
    {

        public FormatDateAttribute(string formatString)
        {
            this.FormatString = formatString;
        }

        private string FormatString { get; set; }

        public override string FormatData(object data)
        {
            if (data == null) return "";
            DateTime date;

            if(DateTime.TryParse(data.ToString(), out date))
            {
                return date.ToString(this.FormatString);
            }

            return "";
        }

    }

    public class FormatIntegerAttribute : FormatAttribute
    {

        public FormatIntegerAttribute(bool useSeparator = false)
        {
            this.FormatString = string.Format("{0}0", useSeparator? "#,##" : "");
        }

    private string FormatString { get; set; }

    public override string FormatData(object data)
    {
        if (data == null) return "";
        int intValue;

        if (int.TryParse(data.ToString(), out intValue))
        {
            return intValue.ToString(this.FormatString);
        }

        return "";
    }

}

    public class FormatNumberAttribute : FormatAttribute
    {

        public FormatNumberAttribute(int decimalPlaces, bool useSeparator = false)
        {
            if (decimalPlaces < 1) throw new ArgumentException("At least one decimal place is required");

            this.FormatString = string.Format("{0}0.{1}", useSeparator ? "#,##" : "",
                new string('0', decimalPlaces));
        }

        private string FormatString { get; set; }

        public override string FormatData(object data)
        {
            if (data == null) return "";
            double dbl;

            if(double.TryParse(data.ToString(), out dbl))
            {
                return dbl.ToString(this.FormatString);
            }

            return "";
        }

    }

    public class FormatPercentAttribute : FormatAttribute
    {

        public FormatPercentAttribute(int decimalPlaces)
        {
            if (decimalPlaces < 0) throw new ArgumentException("Invalid number of decimal places.");
            this.FormatString = string.Format("0{1}%", decimalPlaces == 0 ? "" :
                $".{new string('0', decimalPlaces)}");
        }

        private string FormatString { get; set; }

        public override string FormatData(object data)
        {
            if (data == null) return "";
            double dbl;

            if (double.TryParse(data.ToString(), out dbl))
            {
                return dbl.ToString(this.FormatString);
            }

            return "";
        }

    }

}
