<!--
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
-->

<!-- The HEAD section provides content to be rendered into the HEAD field of the master template. -->
<!-- Head content is typically stylesheet and javascript file references. -->
<!-- @@HEAD@@ -->
    <link rel="stylesheet" href="css/docs.css" />
	<script src="js/docs.js" type="text/javascript"></script>
<!-- @@HEAD@@ -->

<!-- The BODY section provides content to be rendered into the BODY field of the master template. -->
<!-- Body content is typically the html for the page. -->
<!-- @@BODY@@ -->
		<!-- TODO: maybe change this to use real page scope since no longer using WriteMasterSections() on this page -->
		<!-- form is hard coded in order to demonstrate the WriteMasterSections() method in the presenter -->
		<!-- because of the hard coding, this page will not retain the filter options from other pages -->
		<form id="page-scope" action="" method="post" class="page-scope">
			<input type="hidden" name="pageScope.ItemType" />
		</form>
		<div class="content-section-div">
			<h3>Template Engine - Documentation</h3>
			<div class="content">
				<div class="topics">
					<div class="topics-header">Topics</div>
					<div class="topics-scroll">
						<dl>
							<dt data-link="Overview">Overview</dt>
							<dt data-link="Documents">Documents</dt>
								<dd data-link="Sections">Sections</dd>
								<dd data-link="Nesting">Nesting</dd>
								<dd data-link="Literals">Literals</dd>
								<dd data-link="Fields">Fields</dd>
								<dd data-link="Markup Fields">Markup Fields</dd>
							<dt data-link="C# Classes">C# Classes</dt>
								<dd data-link="Template">Template</dd>
								<dd data-link="TemplateWriter">Template Writer</dd>
								<dd data-link="TemplateLoader">Template Loader</dd>
								<dd data-link="TemplateCache">Template Loader</dd>
							<dt data-link="Web">Web & HTML</dt>
								<dd data-link="WebWriter">WebWriter</dd>
								<dd data-link="WebLoader">WebLoader</dd>
								<dd data-link="WebTemplateCache">WebTemplateCache</dd>
								<dd data-link="Option">Option</dd>
								<dd data-link="DropdownDefinition">DropdownDefinition</dd>
								<dd data-link="FieldDefinitions">FieldDefinitions</dd>
							<dt data-link="Examples">Code Examples</dt>
								<dd data-link="GettingStarted">Getting Started</dd>
								<dd data-link="BindingData">Binding Data</dd>
								<dd data-link="FieldProviders">Field Providers</dd>
								<dd data-link="Partials">Partial Views</dd>
								<dd data-link="Options">Options</dd>
								<dd data-link="WebFields">Other Web Fields</dd>
							<dt data-link="Miscellaneous">Miscellaneous</dt>
								<dd data-link="Asp.NetCore">Asp.Net Core</dd>
								<dd data-link="Formatting">Formatting Output</dd>
								<dd data-link="AdditionalMethods">Additional Methods</dd>
								<dd data-link="DebuggingHints">Debugging Hints</dd>
						</dl>
					</div>
				</div>
				<div class="topic-view">
					<div class="topics-header" id="topic">Overview</div>
					<div class="topic-sections">
						<div class="topic-text" data-link="Overview">
							<!-- Overview -->
							<p>
								TemplateEngine is a library that parses text templates and populates template fields with formatted data.
								It is compatible with ASP.Net MVC and Web Forms as well as Web API. Although the library was developed 
								for rendering HTML, it can be used for other purposes where data needs to be merged into a text document.
							</p>
							<div>
								<div class="caption">Some of the features and advantages of Template Engine are:</div>
								<ul>
									<li>Complete separation of code and mark up</li>
									<li>All parts of the template are accessible and reusable which eliminates a plethora of partial views</li>
									<li>Data can be bound to templates by convention which reduces the amount of code required</li>
									<li>Repeating markup such as grids, tables, and drop down options can be bound to a collection and automatically rendered</li>
									<li>Templates can be cached to improve performance</li>
									<li>Custom formatting and localization are supported</li>
									<li>Sections from multiple templates can be combined</li>
								</ul>
								<div class="caption">Getting Started</div>
								<ol>
									<li>Create a template document</li>
									<li>Instantiate a Template object from the template document</li>
									<li>Instantiate a TemplateWriter object based on the Template</li>
									<li>Bind data to the template using the TemplateWriter (optional)</li>
									<li>Append the content and generate the output</li>
								</ol>
								<p>
									Template Engine was developed by 
									<a href="https://github.com/myokapis" target="_blank" rel="nofollow">Gene Graves</a>
									and is licensed under the Apache License, Version 2.0. See the LICENSE.md file for the license terms 
									or visit <a href="http://www.apache.org/licenses/LICENSE-2.0" target="_blank" rel="nofollow">Apache License</a>.
								</p>
								<p>
									Questions and comments are welcome and can be directed to 
									<a href="mailto:gemdevelopers@myokapis.net">gemdevelopers@myokapis.net</a>.
								</p>
							</div>
						</div>
						<div class="topic-text hidden" data-link="Documents">
							<!-- Documents -->
							<p>
								A template document is a text file that contains text to be rendered along with special markup to indicate fields 
								that can be replaced with data. Template documents can be partitioned into sections for greater control over 
								rendering. Sections may even be nested in order to create hierarchical documents.
							</p>
							<p>
								A simple document might look like this:
								<pre class="code">@@EXAMPLE_DOCUMENT@@</pre>
							</p>
							<p>
								<h4 data-link="Sections">Sections</h4>
								Template sections are specially marked areas of the document. The beginning and end of each 
								section is marked with an html comment of the form, <code>&lt;!-- section name --&gt;</code>.
								Valid section names begin and end with @@ and contain only uppercase Latin alphanumeric 
								characters A-Z, numbers 0-9, and the underscore (_). An example of a valid section name is 
								<code>&lt;!-- &#64;@MY_SECTION_NAME@@ --&gt;</code>.
							</p>
							<p>
								Each template document belongs to an implied main section that is automatically created when 
								the document is loaded. The main section is named, @MAIN, and exists only for internal use by 
								the template engine.
							</p>
							<p>
								<h4 data-link="Literals">Literals</h4>
								There is a special type of literal section for safely rendering html, xml, or other literal
								text within an html document. The beginning and end of literal sections are marked with an 
								html comment; however, literal sections names are enclosed in ** rather than @@ in order to 
								differentiate them from standard sections. An example of a literal section comment is 
								<code>&lt;!-- **MY_LITERAL_SECTION** --&gt;</code>.
							</p>
							<p>
								<h4 data-link="Nesting">Nesting</h4>
								Sections may be nested; however, they are not allowed to overlap. Nested sections can serve a number of purposes:
								<ul>
									<li>A partial view can be created from a nested section.</li>
									<li>Content that might be conditionally rendered can be enclosed in a nested section.</li>
									<li>Content that may need to be repeated can be enclosed in a nested section.</li>
								</ul>
								The example template document above contains two sections, BODY and ROW. The ROW section is 
								nested within the BODY section. When the ROW section is selected, it can be rendered multiple 
								times in order to create rows within the table.
							</p>
							<p>
								<h4 data-link="Fields">Fields</h4>
								Fields are placeholders in the template document for data or markup that will be added dynamically.
								Fields are designated with a pair of @@ symbols surrounding the field name. Valid characters for 
								field names are Latin alphanumeric characters a-z, A-Z, and 0-9 as well as the underscore (_).
								The example template document contains three fields, MARKUP, FirstName, and LastName.
							</p>
							<p>
								<h4 data-link="Markup Fields">Markup Fields</h4>
								Markup fields are placeholders in a template for content that will be populated from a field
								provider, which is a separate template writer bound to its own template or template subsection.
								Any field can be bound to either data or to a markup provider. However, if a field is intended to
								be populated with markup rather than with data, it is a convention to use only upper case
								characters, numbers, or the underscore in the field name. This convention is not enforced, but
								it does improve readability of the template document.
							</p>
						</div>
						<div class="topic-text hidden" data-link="C# Classes">
							<!-- C# Classes -->
							<p>
								The primary classes in Template Engine are the Template and TemplateWriter
								which provide the means to parse a template document and manipulate it to
								render text output. Two other convenience classes, TemplateLoader and
								TemplateCache, exist to load template documents from the file system or 
								memory cache and to acquire TemplateWriters for working with the template
								documents.
							</p>
							<p>
								<h4 data-link="Template">Template</h4>
								The Template class parses the text from a template document into sections,
								subsections, and fields that can be manipulated with a TemplateWriter.
							</p>
							<p>
								<h4 data-link="TemplateWriter">Template Writer</h4>
								The TemplateWriter class provides methods for binding data to a Template
								and for rendering views and partial views.
							</p>
							<p>
								<h4 data-link="TemplateLoader">Template Loader</h4>
								The TemplateLoader provides an easy way to load a template from a predefined
								template directory in which template documents resided and to create
								TemplateWriters for working with the template documents.
							</p>
							<p>
								<h4 data-link="TemplateCache">Template Cache</h4>
								The TemplateCache is a TemplateLoader that has a built-in memory cache from
								which Template objects can be retrieved if they have already been cached.
								Otherwise the requested template document is loaded from the file system, and
								the resulting Template object is cached for future use.
							</p>
						</div>
						<div class="topic-text hidden" data-link="Web">
							<!-- Web & HTML -->
							<p>
								The TemplateEngine.Web namespace contains a number of objects that are useful
								for working with HTML documents.
							</p>
							<p>
								<h4 data-link="WebWriter">WebWriter</h4>
								The WebWriter class extends the TemplateWriter class to provide additional
								functionality for working with HTML templates.
							</p>
							<p>
								<h4 data-link="WebLoader">WebLoader</h4>
								The WebLoader inherits from TemplateLoader and provides a document loader
								specifically for the WebWriter class.
							</p>
							<p>
								<h4 data-link="WebTemplateCache">WebTemplateCache</h4>
								The WebTemplateCache provides a document loader and cache specifically for 
								the WebWriter class.
							</p>
							<p>
								<h4 data-link="Option">Option</h4>
								Option is a common data model for specifying text and values for populating
								the <code>&lt;option&gt;</code> tags within a <code>&lt;select&gt;</code> tag.
							</p>
							<p>
								<h4 data-link="DropdownDefinition">DropdownDefinition</h4>
								DropdownDefinition is a master model for binding <code>&lt;option&gt;</code> tags
								to a section within a <code>&lt;select&gt;</code> tag and for selecting a default
								option value.
							</p>
							<p>
								<h4 data-link="FieldDefinitions">FieldDefinitions</h4>
								FieldDefinitions is a master model for binding checkboxes and dropdowns to a section.
							</p>
						</div>
						<div class="topic-text hidden" data-link="Examples">
							<!-- Code Examples -->
							<p>
								Follow the Getting Started instructions below, and look through the various examples. 
								For additional documentation see the 
								<a href="https://github.com/dotnet/aspnetcore" target="_blank">Github project page</a>.
							</p>
							<p>
								This example web project is also a good resource since it contains working code that 
								demonstrates how to apply Template Engine for a variety of use cases.
							</p>
							<p>
								<h4 data-link="GettingStarted">Getting Started</h4>
								Create a template document and save it to a file. <br/>
								@@CREATE_TEMPLATE_HTML@@
							<p>
							<p>
								Use a loader to obtain a TemplateWriter from the template file
								@@GET_WRITER_CODE@@
							</p>
							<p>
								Select a section, bind data to it, and generate the output.
								@@SIMPLE_BINDING_CODE@@
							</p>
							<p>
								The output from the example would be:
								@@SIMPLE_OUTPUT@@
							</p>
							<p>
								<h4 data-link="BindingData">Binding Data</h4>
								Data can be bound to a section through four methods, SetField, SetSectionFields, SetMultiSectionFields, 
								and SetOptionFields. The SetField method has already been demonstrated in this documentation. The 
								SetOptionFields method is discussed under the Options heading. The SetSectionFields and 
								SetMultiSectionFields methods are presented below.
							</p>
							<p>
								<span class="emphasis">SetSectionFields</span> binds a single POCO object to a section. Binding is done 
								automatically for every object field name that has a corresponding template field of the same name.
								@@SET_SECTION_FIELDS_CODE@@
							</p>
							<p>
								<span class="emphasis">SetMultiSectionFields</span> binds a collection of POCO objects to a section 
								and appends the section once per object. This method is useful for populating tables and other 
								repeating structures. Binding is done automatically for every field on the object that has a field 
								name matching a corresponding template field name.
								@@SET_MULTI_SECTION_FIELDS_CODE@@
							</p>
							<p>
								<h4 data-link="FieldProviders">Field Providers</h4>
								The writer can bind data to fields, but it can also bind another writer to a field. This allows
								a field to be replaced by markup from the bound writer. This feature is particularly useful for 
								creating master pages with field placeholders in the head and body that can be bound on the fly 
								to other writers that provide content. The MARKUP_FIELD field in the example template could be
								bound to another writer using code such as this.
							</p>
							<p>
								@@FIELD_PROVIDER_HTML@@
							</p>
							<p>
								@@FIELD_PROVIDER_CODE@@
							</p>
							<p>
								<h4 data-link="Partials">Partial Views</h4>
								TemplateEngine supports partial views by allowing a writer to be created for any subsection 
								of a template. This feature is extremely useful for writing single page applications or for
								composing a page from multiple source templates. Partial views can be used in conjunction 
								with a master page to generate web pages that have a consistent look across a web site.
							</p>
							<p>
								A writer for a partial view can be obtained by calling the GetWriter method on a TemplateWriter 
								object that contains the section from which the partial view is to be created.
							</p>
							<p>
								@@PARTIALS_HTML@@
							</p>
							<p>
								@@PARTIALS_CODE@@
							</p>
							<p>
								<h4 data-link="Options">Options</h4>
								Any of the data binding methods can be used to bind data to the <code>&lt;option&gt;</code> tags 
								witin a <code>&lt;select&gt;</code> tag; however, the SetOptionFields method exists to simplify 
								coding and to provide a way to set one of the options as the initially selected option.
							</p>
							<p>
								Since an html option tag includes text to be displayed and a value associated with that text, a 
								special Option class exists to implement this pattern. The Option class allows a single model to 
								be used for setting all option tags and thus reduces the number of model classes that might 
								otherwise be required.
							</p>
							<p>
								The template text for an option field should look similar to that below. The option tag should be 
								in its own section so that it can be appended multiple times. It should also contain TEXT, VALUE, 
								and SELECTED fields.
								@@OPTIONS_HTML@@
							</p>
							<p>
								The code below demonstrates how to use the Option class and SetOptionFields method.
								@@OPTIONS_CODE@@
							</p>
							<p>
								<h4 data-link="WebFields">Other Web Fields</h4>
								Template Engine provides two master models that can be used to map data to web fields such as
								checkboxes and dropdown lists.
							</p>
							<p>
								A <span class="emphasis">DropdownDefinition</span> is used when a section contains a 
								<code>&lt;select&gt;</code> tag along with other fields that need to be bound 
								to data. A DropdownDefinition can be passed to the writer along with the data record 
								to be bound to the section.
							</p>
							<p>
								The DropdownDefinition provides the data and metadata for populating the 
								<code>&lt;option&gt;</code> tags along with a reference to a field in the main data set 
								from which the initial option value can be selected.
							</p>
							<p>
								A <span class="emphasis">FieldDefinitions</span> model is used when there are multiple web fields 
								that need to be bound to a section and have their initial values set from the main 
								data record being used to populate the section fields. In addition to dropdowns, 
								FieldDefinitions also support binding checkboxes and setting their initial state.
							</p>
							<p>
								Consider this partial view template and code.
								@@DROPDOWN_DEFINITION_HTML@@
							</p>
							<p>
								@@DROPDOWN_DEFINITION_CODE@@
							</p>
							<p>
								The BUDGET_PERIODS section of the template would be bound to the <code>periods</code> 
								options data, and an initial value in that dropdown would be selected for the option 
								matching the value of <code>dataModel.PeriodId</code>.
							</p>
							<p>
								Similarly, the BANK_ACCOUNTS section of the template would be bound to the 
								<code>bankAccounts</code> options data, and an initial value in that dropdown would be
								selected for the value matching the value of <code>dataModel.BankAccountId</code>.
							</p>
						</div>
						<div class="topic-text hidden" data-link="Miscellaneous">
							<!-- Miscellaneous -->
							<p>
								Here are a few miscellaneous topics to assist with developing and troubleshooting.
							</p>
							<p>
								<h4 data-link="Asp.NetCore">Asp.Net Core</h4>
								The TemplateEngine.AspNetCore Nuget package provides extension methods for
								easily integrating Template Engine into an Asp.Net Core web application.
							<p>
							<p>
								To use Template Engine in an Asp.Net Core project follow these steps:
								<ol>
									<li>Install the TemplateEngine.AspNetCore Nuget package.</li>
									<li>Create a Templates directory in the root of your project. Save all of your 
										template document files to this Templates directory.</li>
									<li>Call the UseTemplateEngine() extension method on the HostBuilder. This will
										set the default template directory and register the following objects for
										dependency injection.
										<ul>
											<li>TemplateLoader</li>
											<li>TemplateCache</li>
											<li>TemplateEngineSettings</li>
											<li>All classes that descend from the MasterPresenterBase class</li>
										</ul>
										@@REGISTRATION_CODE@@</li>
									<li>Optional - Add a TemplateEngine configuration section to your appsettings.json
										configuration file.
								</ol>
							</p>
							<p>
								<h4 data-link="Formatting">Formatting Output</h4>
								Template Engine provides a set of format attributes in the TemplateEngine.Formats 
								namespace. A format attribute applied to a model field causes the writer to format
								the field data using the formatter. Format attributes exist for currency, date, 
								integer, number, and percent fields.
							<p>
							<p>
								<h4 data-link="AdditionalMethods">Additional Methods</h4>
								The TemplateWriter class has a few important methods that have not been explicitly
								discussed in this documentation.
								<ul>
									<li>
										AppendAll - appends and deselects nested template sections until it reaches 
										the main section. An optional parameter can be provided that will cause it 
										to stop appending when it reaches the specified section.
									</li>
									<li>
										Clear - clears any unappended data from working memory in the currently selected section.
									</li>
									<li>
										GetContent - generates the text output containing all of the appended sections and 
										bound data. This method must be called in order to get the text output from the 
										writer.
									</li>
									<li>
										Reset - clears all appended and unappended data in the currently selected section.
									</li>
									<li>
										SelectProvider - gets the writer that was registered as a field provider for a given field.
									</li>
								</ul>
							<p>
							<p>
								<h4 data-link="DebuggingHints">Debugging Hints</h4>
								Some of the common mistakes that occur when working with templates are:
							<p>
							<p>
								<ol>
									<li>
										Forgetting to append a section - Selecting a section and binding data to it places the 
										section's contents in working memory. In order for a section's contents to be included in 
										the output, the section must be appended. If a section is nested within another section, then 
										the parent section must also be appended. Every section from the main section down to the 
										innermost nested section that is to be rendered must all have been appended. Failure to 
										append any section in this chain will result in lost content.
									</li>
									<li>
										Attempting to select a section that is not a child of the current section - When working 
										with templates that contain multiple sections, particularly templates that have nested 
										sections, it is important to know which section is the current section before selecting or 
										deselecting a section. The TemplateWriter class has a SelectedSectionName property that is 
										useful to examine when debugging. This property gives the name of the section that is 
										currently selected.
									</li>
									<li>
										Mismatch between the data model field names and the template field names - For autobinding 
										to work, the name of the data field needs to exactly match the name of the template field to 
										which it will be bound. Field names are case sensitive.
									</li>
									<li>
										Calling SelectSection instead of SelectProvider - When a writer is bound to a field as a 
										field provider, it must be selected by calling SelectProvider. Once the provider has been 
										selected, any nested sections within the writer can be accessed by calling SelectSection.
									</li>
								</ol>
							</p>
						</div>
					</div>
				</div>
			</div>
		</div>
<!-- @@BODY@@ -->

<!-- Since there is preformated code and HTML, a partial view can be used to make -->
<!-- the overall document more readable. -->
<!-- @@EXAMPLE_DOCUMENT@@ -->
<!-- **1** -->
<!-- @@BODY@@ -->
<h1>Example</h1>
@@MARKUP_FIELD@@
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
<!-- @@BODY@@ -->
<!-- **1** -->
<!-- @@EXAMPLE_DOCUMENT@@ -->

<!-- Since there is preformated code and HTML, a partial view can be used to make -->
<!-- the overall document more readable. -->
<!-- @@CREATE_TEMPLATE_HTML@@ -->
<pre class="code">
<!-- **2** -->
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
<!-- **2** -->
</pre>
<!-- @@CREATE_TEMPLATE_HTML@@ -->

<!-- @@GET_WRITER_CODE@@ -->
<pre class="code">
// instantiate a loader pointing to a directory containing the template files
var loader = new TemplateLoader(templateDirectoryPath);

// get a writer for a template file in the template directory
var writer = await loader.GetWriterAsync(templateFileName);
</pre>
<!-- @@GET_WRITER_CODE@@ -->

<!-- @@SIMPLE_BINDING_CODE@@ -->
<pre class="code">
// select a section so that it can be worked with
writer.SelectSection("ROW");

// bind data to the section fields and append the content
writer.SetField("FirstName", "Ichabod");
writer.SetField("LastName", "Crane");
writer.AppendSection();

// bind data to the section fields
writer.SetField("FirstName", "Alex");
writer.SetField("LastName", "Rodriguez");

// calling this method with true appends the current section then deselects it
writer.AppendSection(true);

// render the output (all selected sections must first be appended and deselected)
writer.GetContent();
</pre>
<!-- @@SIMPLE_BINDING_CODE@@ -->

<!-- @@SIMPLE_OUTPUT@@ -->
<pre class="code">
<!-- **3** -->
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
<!-- **3** -->
</pre>
<!-- @@SIMPLE_OUTPUT@@ -->

<!-- @@SET_SECTION_FIELDS_CODE@@ -->
<pre class="code">
var data = new MyModel { FirstName = "Ichabod", LastName = "Crane" };

// the writer selects the section, binds the data, then deselects the section
writer.SetSectionFields("ROW", data);
</pre>
<!-- @@SET_SECTION_FIELDS_CODE@@ -->

<!-- @@SET_MULTI_SECTION_FIELDS_CODE@@ -->
<pre class="code">
// create a collection of data
var data = new List&lt;MyModel&gt;
{
    new MyModel { FirstName = "Ichabod", LastName = "Crane" },
    new MyModel { FirstName = "Alex", LastName = "Rodriguez" }
};

// the writer selects the ROW section, binds data,
// appends the section once for each object in the collection,
// and then deselects the section
writer.SetMultiSectionFields("ROW", data);
</pre>
<!-- @@SET_MULTI_SECTION_FIELDS_CODE@@ -->

<!-- @@FIELD_PROVIDER_HTML@@ -->
<pre class="code">
<!-- **8** -->
...
<h1>Example</h1>
@@MARKUP_FIELD@@
<table>
...
<!-- **8** -->
</pre>
<!-- @@FIELD_PROVIDER_HTML@@ -->

<!-- @@FIELD_PROVIDER_CODE@@ -->
<pre class="code">
// register another writer to provide content to populate the MARKUP_FIELD field
writer.RegisterFieldProvider("MARKUP_FIELD", contentWriter);

// select the field provider
writer.SelectProvider("MARKUP_FIELD");

// treat the provider like it is a regular section
// bind data, select subsections, etc. to generate content
[your code here]

// append the provider content
writer.AppendSection(true);
</pre>
<!-- @@FIELD_PROVIDER_CODE@@ -->

<!-- @@OPTIONS_HTML@@ -->
<pre class="code">
<!-- **4** -->
<select>
  <!-- @@OPTIONS_DEMO@@ --><option value="@@VALUE@@" @@SELECTED@@>@@TEXT@@</option><!-- @@OPTIONS_DEMO@@ -->
</select>
<!-- **4** -->
</pre>
<!-- @@OPTIONS_HTML@@ -->

<!-- @@OPTIONS_CODE@@ -->
<pre class="code">
<!-- **5** -->
// create a collection of options
var data = new List<Option>
{
    new Option { Text = "Small", Value = "S" },
    new Option { Text = "Medium", Value = "M" },
    new Option { Text = "Large", Value = "L" }
};

// writer selects the OPTIONS_DEMO section, binds data, 
// and appends the section once for each item in the data collection
// the OPTIONS_DEMO section is automatically deselected
// the optional third argument specifies the value of the option that should be initially selected
writer.SetOptionFields("OPTIONS_DEMO", data, "M");
<!-- **5** -->
</pre>
<!-- @@OPTIONS_CODE@@ -->

<!-- @@PARTIALS_HTML@@ -->
<pre class="code">
<!-- **9** -->
...
  <tbody>
    <!-- @@ROW@@ -->
    <tr>
      <td>@@FirstName@@</td>
      <td>@@LastName@@</td>
    </tr>
    <!-- @@ROW@@ -->
  </tbody>
...
<!-- **9** -->
</pre>
<!-- @@PARTIALS_HTML@@ -->

<!-- @@PARTIALS_CODE@@ -->
<pre class="code">
// gets a writer that only contains the ROW section (and any nested sections were they to exist)
var partialWriter = writer.GetWriter("ROW");
</pre>
<!-- @@PARTIALS_CODE@@ -->

<!-- @@DROPDOWN_DEFINITION_HTML@@ -->
<pre class="code">
<!-- **6** -->
<!-- @@SELECTOR@@ -->
<div class="selector-div">
  <div>Budget Period</div>
  <select id="periodId">
    <!-- @@BUDGET_PERIODS@@ --><option value="@@VALUE@@" @@SELECTED@@>@@TEXT@@</option><!-- @@BUDGET_PERIODS@@ -->
  </select>
  <div>Bank Account</div>
  <select id="bankAccountId">
    <!-- @@BANK_ACCOUNTS@@ --><option value="@@VALUE@@" @@SELECTED@@>@@TEXT@@</option><!-- @@BANK_ACCOUNTS@@ -->
  </select>
</div>
<!-- @@SELECTOR@@ -->
<!-- **6** -->
</pre>
<!-- @@DROPDOWN_DEFINITION_HTML@@ -->

<!-- @@DROPDOWN_DEFINITION_CODE@@ -->
<pre class="code">
<!-- **7** -->
// get the data model to be bound to this section
var dataModel = dataService.GetMyFormData();

// get data to populate options tags in the two dropdowns
var periods = await dataService.GetPeriods<Option>();
var bankAccounts = await dataService.GetBankAccounts<Option>();

// create a field definition to map the data and dropdowns to the template
var definitions = new FieldDefinitions
(
	new DropdownDefinition("BUDGET_PERIODS", "PeriodId", periods),
	new DropdownDefinition("BANK_ACCOUNTS", "BankAccountId", bankAccounts)
);

// bind the data model to the current section and set up the dropdowns
writer.SetSectionFields(dataModel, SectionOptions.AppendDeselect, definitions);
<!-- **7** -->
</pre>
<!-- @@DROPDOWN_DEFINITION_CODE@@ -->

<!-- @@REGISTRATION_CODE@@ -->
<pre class="code">
<!-- **10** -->
public static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .UseSerilog()
        .UseTemplateEngine()
        .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>());
<!-- **10** -->
</pre>
<!-- @@REGISTRATION_CODE@@ -->
