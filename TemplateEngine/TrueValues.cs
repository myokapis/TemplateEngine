using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TemplateEngine
{

    public static class TrueValues
    {

        // TODO: provide a way for developers to set and get values that are localized
        static TrueValues()
        {
            Values = new List<string> { "Yes", "Y", "T", "True" };
        }

        public static bool Contains(string value)
        {
            return Values.Any(v => string.Equals(value, v, StringComparison.CurrentCultureIgnoreCase));
        }

        public static IEnumerable<string> Values { get; set; }

    }

}
