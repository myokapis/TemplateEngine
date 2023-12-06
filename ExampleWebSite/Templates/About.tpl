<!--
/* ****************************************************************************
Copyright 2018-2023 Gene Graves

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

<!-- @@HEAD@@ -->
    <link rel="stylesheet" href="css/about.css" />
<!-- @@HEAD@@ -->

<!-- @@BODY@@ -->
		<!-- form is hard coded in order to demonstrate the WriteMasterSections() method in the presenter -->
		<!-- because of the hard coding, this page will not retain the filter options from other pages -->
		<form id="page-scope" action="" method="post" class="page-scope">
			<input type="hidden" name="pageScope.ItemType" />
		</form>

		<div class="content-section-div">
			<h3>Template Engine - About</h3>
			<p>Template Engine is an open source .Net library for merging data into a text template and
				rendering the resulting content.</p>
			<table>
				<caption style="text-align: left;">Versions</caption>
				<colgroup>
					<col style="width: 10%">
					<col>
					<col>
					<col>
				</colgroup>
				<thead>
				<tr>
					<th>Major Version</th>
					<th>.Net Version</th>
					<th>Status</th>
					<th>Planned Features</th>
				<tr>
				</thead>
				<tbody>
				<tr>
					<td>1.x</td>
					<td>.Net Standard 2.0</td>
					<td>No longer supported</td>
					<td></td>
				</tr>
				<tr>
					<td>3.x</td>
					<td>.Net Core 3.1</td>
					<td>No longer supported</td>
					<td></td>
				</tr>
				<tr>
					<td>6.0</td>
					<td>.Net 6</td>
					<td>Current</td>
					<td>
					</td>
				</tr>
				<tr>
					<td>8.0</td>
					<td>.Net 8</td>
					<td>Future</td>
					<td>
						<ul>
							<li>Differentiate between form data fields and globalized text fields</li>
							<li>Automatic registration and rendering of field providers</li>
							<li>Methods to simplify selecting nested sections</li>
							<li>Allow iteration of field names and fields</li>
							<li>Improvements to ViewModelAccessor</li>
						</ul>
					</td>
				</tr>
				</tbody>
			</table>
			<table>
				<caption style="text-align: left;">Extensions</caption>
				<thead>
				<tr>
					<th>Extension Name</th>
					<th>Description</th>
					<th>Notes</th>
				<tr>
				</thead>
				<tbody>
				<tr>
					<td>TemplateEngine.AspNetCore</td>
					<td>Automatically registers presenters with DI in Asp.Net Core web applications</td>
					<td>Currently available for use with Template Engine versions 3.x & 6.x</td>
				</tr>
				</tbody>
			</table>
			<table>
				<caption style="text-align: left;">Contributors</caption>
				<thead>
				<tr>
					<th>Contributor Name</th>
					<th>Email Address</th>
					<th>Github Site</th>
				<tr>
				</thead>
				<tbody>
				<tr>
					<td>Gene Graves</td>
					<td><a href="mailto:gemdevelopers@myokapis.net">gemdevelopers@myokapis.net</a></td>
					<td><a href="https://github.com/myokapis" target="_blank">https://github.com/myokapis</a></td>
				</tr>
				</tbody>
			</table>
			<div>
				<h4>License</h4>
				<p>
					Template Engine is licensed under the Apache License, Version 2.0. See the LICENSE.md file 
					for the license terms or visit <a href="https://www.apache.org/licenses/LICENSE-2.0.html" target="_blank">Apache Licenses</a>.
				</p>
			</div>
		</div>
<!-- @@BODY@@ -->
