/* ****************************************************************************
Copyright 2018-2022 Gene Graves

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

using System.Collections.Generic;

namespace TemplateEngine.Tests.Helpers
{

    public static class TemplateText
    {

        public static List<string> Items = new List<string>()
        {
            new List<string>
            {
                "<!-- @@SECTION1@@ -->\r\n",
                "  some text\r\n",
                "<!-- @@SECTION2@@ -->\r\n",
                "  section 2 text\r\n",
                "<!-- @@SECTION2@@ -->\r\n",
                "  post text section 2\r\n",
                "<!-- @@SECTION1@@ -->\r\n",
                "<!-- @@SECTION3@@ -->",
                "  section 3 text\r\n",
                "<!-- @@SECTION3@@ -->"
            }.Concat(),
            new List<string>
            {
                "<!-- @@SECTION1@@ -->\r\n",
                "  some text\r\n",
                "<!-- @@SECTION2@@ -->\r\n",
                "  Field1: @@Field1@@\r\n",
                "  Field2: @@Field2@@\r\n",
                "<!-- @@SECTION2@@ -->\r\n",
                "  post text section 2\r\n",
                "<!-- @@SECTION1@@ -->\r\n",
                "<!-- @@SECTION3@@ -->",
                "  section 3 text\r\n",
                "<!-- @@SECTION3@@ -->"
            }.Concat(),
            new List<string>
            {
                "<!-- @@SECTION1@@ -->\r\n",
                "  some text\r\n",
                "<!-- @@SECTION2@@ -->\r\n",
                "  Field1: @@Field1@@\r\n",
                "  Field2: @@Field2@@\r\n",
                "<!-- @@SECTION2@@ -->\r\n",
                "  post text section 2\r\n",
                "<!-- @@SECTION1@@ -->\r\n",
                "<!-- @@SECTION3@@ -->",
                "  section 3 text\r\n",
                "  Field3: @@Field3@@\r\n",
                "  Field4: @@Field4@@\r\n",
                "  <!-- @@OPTION_SECTION1@@ -->@@TEXT@@;@@VALUE@@;@@SELECTED@@<!-- @@OPTION_SECTION1@@ -->\r\n",
                "  <!-- @@OPTION_SECTION2@@ -->@@TEXT@@;@@VALUE@@;@@SELECTED@@<!-- @@OPTION_SECTION2@@ -->\r\n",
                "  Checkbox1: @@Field5@@\r\n",
                "  Checkbox2: @@Field6@@\r\n",
                "<!-- @@SECTION3@@ -->"
            }.Concat(),
            new List<string>
            {
                "<!-- @@SECTION1@@ -->\r\n",
                "-->@@Field1@@<--\r\n",
                "-->@@Field2@@<--\r\n",
                "-->@@Field3@@<--\r\n",
                "-->@@Field4@@<--\r\n",
                "-->@@Field5@@<--\r\n",
                "-->@@Field6@@<--\r\n",
                "<!-- @@SECTION1@@ -->\r\n"
            }.Concat(),
            new List<string>
            {
                "Main1: @@Main1@@\r\n",
                "<!-- @@SECTION1@@ -->\r\n",
                "  some text\r\n",
                "<!-- @@SECTION2@@ -->\r\n",
                "  Field1: @@Field1@@\r\n",
                "  Field2: @@Field2@@\r\n",
                "<!-- @@SECTION2@@ -->\r\n",
                "  post text section 2\r\n",
                "<!-- @@SECTION1@@ -->\r\n",
                "<!-- @@SECTION3@@ -->",
                "  section 3 text\r\n",
                "<!-- @@SECTION3@@ -->\r\n",
                "Main2: @@Main2@@"
            }.Concat(),
            new List<string>
            {
                "Main1: @@Main1@@\r\n",
                "<!-- @@SECTION1@@ -->\r\n",
                "  Field1.1: @@Field1_1@@\r\n",
                "<!-- @@SECTION2@@ -->\r\n",
                "  Field2.1: @@Field2_1@@\r\n",
                "<!-- @@SECTION3@@ -->",
                "  section 3 text\r\n",
                " Field3.1: @@Field3_1@@\r\n",
                "<!-- @@SECTION3@@ -->\r\n",
                "  Field2.2: @@Field2_2@@\r\n",
                "<!-- @@SECTION2@@ -->\r\n",
                "  post text section 2\r\n",
                "<!-- @@SECTION1@@ -->\r\n",
                "Main2: @@Main2@@"
            }.Concat(),
            new List<string>
            {
                "Start of section @@SectionName@@",
                "@@Field1@@",
                "End of section @@SectionName@@"
            }.Concat(),
            new List<string>
            {
                "<html>",
                "<head>",
                "    <title>Test</title>",
                "    @@HEAD@@",
                "</head>",
                "<body>",
                "  Some test info",
                "</body>",
                "</html>"
            }.Concat(),
            new List<string>
            {
                "<!-- @@HEAD@@ -->",
                "<link rel=\"stylesheet\" href=\"Content/Import.css\" />",
                "<script type=\"text/javascript\" src=\"Scripts/App/import.js\"></script>",
                "<!-- @@HEAD@@ -->"
            }.Concat(),
            new List<string>
            {
                "<h1>Example</h1>",
                "@@PROVIDER@@",
                "<table>",
                "  <thead>",
                "    <tr>",
                "      <th>First Name</th>",
                "      <th>Last Name</th>",
                "    </tr>",
                "  </thead>",
                "  <tbody>",
                "    <!-- @@ROW@@ -->",
                "    <tr>",
                "      <td>@@FirstName@@</td>",
                "      <td>@@LastName@@</td>",
                "    </tr>",
                "    <!-- @@ROW@@ -->",
                "  </tbody>",
                "</table>"
            }.Concat("\r\n"),
            new List<string>
            {
                "<!-- **LITERAL1** -->",
                "<!-- @@SECTION1@@ -->",
                "<!-- **LITERAL2** -->",
                "<!-- @@SECTION1@@ -->",
                "<!-- **LITERAL1** -->"
            }.Concat("\r\n"),
            new List<string>
            {
                "Some text before plus a field @@Field1@@",
                "<!-- **LITERAL1** -->",
                "Field2: @@Field2@@",
                "<!-- @@SECTION1@@ -->",
                "Field3: @@Field3@@",
                "<!-- **LITERAL2** -->",
                "<!-- @@SECTION1@@ -->",
                "<!-- **LITERAL1** -->Some text after @@Field4@@ plus a field"
            }.Concat("\r\n"),
            new List<string>
            {
                "<!-- @@SECTION1@@ -->",
                "<!-- **LITERAL1** -->",
                "<!-- @@SECTION1@@ -->",
                "<!-- @@SECTION2@@ -->",
                "<!-- @@SECTION2@@ -->",
                "<!-- @@SECTION1@@ -->",
                "<!-- **LITERAL1** -->",
                "<!-- @@SECTION1@@ -->",
                "<!-- @@SECTION2@@ -->",
                "<!-- @@SECTION2@@ -->"
            }.Concat("\r\n")
        };

    }

}
