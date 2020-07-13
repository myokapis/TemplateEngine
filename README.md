# Template Engine
TemplateEngine is a library that parses text templates and populates template fields with formatted data. It is compatible with ASP.Net MVC and Web Forms as well as Web API. Although the library was developed for rendering HTML, it can be used for other purposes where data needs to be merged into a text document.

Some of the features and advantages of Template Engine are:
 - Complete separation of code and mark up
 - All parts of the template are accessible and reusable which eliminates a plethora of partial views
 - Data can be bound to templates by convention which reduces the amount of code required
 - Repeating markup such as grids, tables, and drop down options can be bound to a collection and automatically rendered
 - Templates can be cached to improve performance
 - Custom formatting and localization are supported
 - Sections of multiple templates can be combined on the fly

Using the TemplateEngine library involves these steps:
1. Create a template text document
2. Instantiate a Template instance from the template document
3. Instantiate a TemplateWriter instance from the Template instance
4. Use the TemplateWriter methods to bind data and manipulate content
5. Render the output via the GetContent writer method

## Template Document
A template document is a text file that contains text to be rendered along with special markup to indicate fields that can be replaced with data. Template documents can be partitioned into sections for greater control over rendering. Sections may even be nested in order to create hierarchical documents.

A simple template document might look like this:
```html
<h1>Example</h1>
@@PROVIDER@@
<table>
  <thead>
    <tr>
      <th>First Name</th>
      <th>Last Name</th>
    </tr>
  </thead>
  <tbody>
    <!-- @@ROW@@ -->
    <tr>
      <td>@@FirstName@@</td>
      <td>@@LastName@@</td>
    </tr>
    <!-- @@ROW@@ -->
  </tbody>
</table>
```
This example template document contains three fields (PROVIDER, FirstName, and LastName) and one section (ROW). Fields are designate with a pair of @@ symbols surrounding the field name. Valid characters for field names are Latin alphanumeric characters a-z, A-Z, and 0-9 as well as the underscore (\_). Sections are designated with a pair of @@ symbols surrounding the section name. Valid characters for section names are uppercase Latin alphanumeric characters A-Z and 0-9 as well as the underscore (\_). Section names must be enclosed in an html comment ```<!-- -->``` in order to differentiate section names from field names. Each section must be enclosed in a pair of section names in order to mark the beginning and the end of the section. Sections may be nested; however, they are not allowed to overlap.

## Template Class
The Template class parses and validates a template document. Instantiating a template instance is done as follows:
```csharp
var template = new Template(myTemplateText);
```

## TemplateWriter Class
The TemplateWriter class provides methods for manipulating templates, binding data to fields, and rendering output. A TemplateWriter instance is created as follows:
```csharp
var writer = new TemplateWriter(template);
```

### Sections
The writer contains an @MAIN section for the overall template document and any other sections explicitly defined within the template document. For example, a writer created for our template document above would contain an @Main section and a ROW section. Sections are only included in the output if they are selected and appended. If our example was rendered without appending the ROW section, the output would look like this:
```html
<h1>Example</h1>
<table>
  <thead>
    <tr>
      <th>First Name</th>
      <th>Last Name</th>
    </tr>
  </thead>
  <tbody>
  </tbody>
</table>
```
However, if we select and append the ROW section, it will be included in the content as many times as we append it. There are many helper methods available that automatically select, append, and/or deselect sections; these helpers eliminate most of the explicit select, append, and deselect statements in the code. Here is a brute force example for illustration purposes only.
```csharp
// select a section so that it can be worked with
writer.SelectSection("ROW");

// bind data to the section fields and append the content
writer.SetField("FirstName", "Ichabod");
writer.SetField("LastName", "Crane");
writer.AppendSection();

// bind data to the section fields and append the content
writer.SetField("FirstName", "Alex");
writer.SetField("LastName", "Rodriguez");

// calling this method with true appends the section then deselects it
writer.AppendSection(true);
```
The output from the example would now be:
```html
<h1>Example</h1>
<table>
  <thead>
    <tr>
      <th>First Name</th>
      <th>Last Name</th>
    </tr>
  </thead>
  <tbody>
    <tr>
      <td>Ichabod</td>
      <td>Crane</td>
    </tr>
    <tr>
      <td>Alex</td>
      <td>Rodriguez</td>
    </tr>
  </tbody>
</table>
```

### Field Providers
The writer can bind data to fields, but it can also bind another writer to a field. This is particularly useful for creating master pages with field placeholders in the head and body that can be bound on the fly to other writers that will provide additional content. The PROVIDER field in the example template could be bound to another writer using code such as this:
```csharp
// register another writer to provide content to populate the PROVIDER field
writer.RegisterFieldProvider("PROVIDER", contentWriter);
```

### Binding Data
Data can be bound to a section through four methods, SetField, SetSectionFields, SetMultiSectionFields, and SetOptionFields. The SetField method has already been demonstrated in this documentation. The SetOptionFields method is discussed under the Options heading.

SetSectionFields binds a single POCO object to a section. Binding is done automatically for every object field name that has a corresponding template field of the same name.
```csharp
var data = new MyModel { FirstName = "Ichabod", LastName = "Crane" };

// the writer selects the section, binds the data, then deselects the section
writer.SetSectionFields("ROW", data);
```
SetMultiSectionFields binds a collection of POCO objects to a section and appends the section contents once per object. This method is useful for populating multi-row tags such as tables and for repeating sections. Binding is done automatically for every object field name that has a corresponding template field of the same name.
```csharp
// create a collection of data
var data = new List<MyModel>
{
    new MyModel { FirstName = "Ichabod", LastName = "Crane" },
    new MyModel { FirstName = "Alex", LastName = "Rodriguez" }
};

// the writer selects the ROW section, binds data and appends the section once for each object in the collection, and then deselects the section
writer.SetMultiSectionFields("ROW", data);
```

### Options
Option tags can be included in a select tag using any of the data binding methods. However, the SetOptionFields method exists to simplify coding and to provide a way to set one of the options as the initially selected option.

Since an html option tag includes text to be displayed and a value associated with that text, a special Option class exists to implement this pattern. The Option class allows a single model to be used for setting all option tags and thus reduces the number of model classes that might otherwise be required.

The template text for an option field should look similar to that below. The option tag should be in its own section so that it can be appended multiple times. It should also contain TEXT, VALUE, and SELECTED fields.
```html
<select>
  <!-- @@OPTIONS_DEMO@@ --><option value="@@VALUE@@" @@SELECTED@@>@@TEXT@@</option><!-- @@OPTIONS_DEMO@@ -->
</select>
```
The Option class and SetOptionFields method can be used as follows:
```csharp
// create a collection of options
var data = new List<Option>
{
    new Option { Text = "Small", Value = "S" },
    new Option { Text = "Medium", Value = "M" },
    new Option { Text = "Large", Value = "L" }
};

// writer selects the OPTIONS_DEMO section, binds data and appends the section once for each item in the data collection, then deselects the section
// the optional third argument specifies the value of the option that should be initially selected
writer.SetOptionFields("OPTIONS_DEMO", data, "M");
```

### Formatting Output
In addition to automatically binding data models to a section, TemplateEngine provides a set of formatter attributes for decorating model properties to control how bound data is formatted. Formatters are in the TemplateEngine.Formats namespace and currently include attributes for formatting currency, date, integer, number, and percent fields.

### Partial Templates
TemplateEngine supports partial templates by allowing a writer to be created for any subsection of a template. This feature is extremely useful for writing single page applications or providing content for pages that use AJAX to replace part of a page's content.

A writer for a partial template can be obtained by calling the GetWriter method on a writer that contains the section to be used as a partial template.
```csharp
// gets a writer that only contains the ROW section (and any nested sections were they to exist)
var partialWriter = writer.GetWriter("ROW");
```

### Additional Methods
There are a few important methods that have not previously been discussed.
- AppendAll - appends and deselects nested template sections until it reaches the main section. An optional parameter can be provided that will cause it to stop when it reaches a particular section. 
- Clear - clears any unappended data from working memory in the currently selected section.
- GetContent - generates the text output containing all of the appended sections and bound data.
- Reset - clears all appended and unappended data in the currently selected section.
- SelectProvider - gets the writer that was registered as a field provider for a given field.

```csharp
// this method must be called in order to get the text output from the writer
return writer.GetContent();
```

### Debugging Hints
Some of the common mistakes that occur when working with templates are:
1. Forgetting to append a section - Selecting a section and binding data to it places the section's content in working memory. In order for a section's content to be included in the output, it must be appended. If that section is nested within another section, then the parent section must also be appended. Every section from the main section down to the innermost nested section that is to be rendered must all have been appended. Failure to append any section in this chain will result in lost content.
2. Attempting to select a section that is not a child of the current section  - When working with templates that contain multiple sections, particularly templates that have nested sections, it is important to know which section is the current section before selecting or deselecting a section. The TemplateWriter class has a SelectedSectionName property that is useful to examine when debugging. This property gives the name of the section that is currently selected.
3. Mismatch between the data model field names and the template field names - For autobinding to work, the name of the data field needs to exactly match the name of the template field to which it will be bound. Field names are case sensitive.
4. Calling SelectSection instead of SelectProvider - When a writer is bound to a field as a field provider, it must be selected by calling SelectProvider. Once the provider has been selected, any nested sections within the writer can be accessed by calling SelectSection.

## Credits
Template Engine was developed by [Gene Graves](https://github.com/myokapis). Questions and comments are welcome and can be directed to [gemdevelopers@myokapis.net](mailto:gemdevelopers@myokapis.net).

Thank you to Niels Wojciech Tadeusz Andersen [haj@zhat.dk](mailto:haj@zhat.dk) for the TemplateEngine (C library) and TemplateEngine for .NET libraries that I used for so many years. None of the code from those libraries has been reused here; however, I have borrowed the ```<!-- @@SECTION@@ -->``` and ```@@Field@@``` template markdown since it was already familiar to me and a good idea.

## License
Template Engine is licensed under the Apache License, Version 2.0. See the [LICENSE.md](https://github.com/myokapis/TemplateEngine/blob/master/LICENSE.md) file for the license terms or visit [Apache License](http://www.apache.org/licenses/LICENSE-2.0).
