using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TemplateEngine
{

    public static class Extensions
    {

        public static string Concat(this IEnumerable<string> collection, string separator = "")
        {
            return string.Join(separator, collection);
        }

    }

}
