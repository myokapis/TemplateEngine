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
    <link rel="stylesheet" href="css/home.css" />
<!-- @@HEAD@@ -->
<!-- @@BODY@@ -->
		@@PAGE_SCOPE@@
		<div class="content-section-div">
			<h3>Template Engine - Home</h3>
			<p>Welcome to the Template Engine example website. Here you will find code samples, 
				documentation, and other useful information to help you get started with using 
				the Template Engine package.</p>
			<h4>Examples</h4>
			<p>Two example web pages are part of this site. Each example shows how to apply
				Template Engine to render a multi-section web page that includes common web
				elements such as tables, checkboxes, and dropdown lists. Both pages use the
				same underlying dataset; however, the complexity of the layout varies between
				the two examples.
			</p>
			<p>Each example displays a collection of animals, vegetables, and minerals. The 
				displayed items can be filtered by selecting an item type in a dropdown box. 
				Items can be selected using checkboxes and the selections can be sent to the 
				server by clicking a button. The selections are then processed on the server 
				and HTML is returned for display in a modal dialog.
			</p>
			<p><span>Simple:</span>The simple example displays items on the page using a consistent 
				layout for each item. This use case is common when displaying lists of information
				in a table format.
			</p>
			<p><span>Complex:</span>The complex example displays items on the page using a different 
				layout for each item type. This use case is less common. One example might be a 
				tabular report or worksheet in which some of the report fields could be edited
				depending upon characteristics of the row of data. One way that use case could be 
				met is by using two different layouts - one for the editable records and one for
				the static records.
			</p>
		</div>
<!-- @@BODY@@ -->
