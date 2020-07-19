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

<!-- @@HEAD@@ -->
    <link rel="stylesheet" href="css/simple.css" />
	<script src="js/simple.js" type="text/javascript"></script>
<!-- @@HEAD@@ -->
<!-- @@BODY@@ -->
		<div class="content-section-div">
			@@PAGE_SCOPE@@
			<table>
				<caption>Animal-Vegetable-Mineral</caption>
				<thead>
					<tr>
						<th></th>
						<th>Name</th>
						<th>Type</th>
						<th>Description</th>
						<th>Fun Fact</th>
					</tr>
				</thead>
				<tbody>
					<!-- @@ITEMS@@ -->
					<tr data-url="@@ImageUrl@@">
						<td><input type="checkbox" name="Selected" @@Selected@@ /></td>
						<td name="Name">@@Name@@</td>
						<td>@@Type@@</td>
						<td>@@Description@@</td>
						<td>@@FunFact@@</td>
					</tr>
					<!-- @@ITEMS@@ -->
				</tbody>
			</table>
			<div class="submit-div"><button type="button" id="selectButton">Select</button></div>
			<div id="modalPopup" class="modal-popup"></div>
		</div>
<!-- @@BODY@@ -->
<!-- @@POPUP@@ -->
<div class="popup-content">
	<!-- @@POPUP_ITEMS@@ -->
	<div class="popup-item">
		<div>@@Name@@</div>
		<div>@@Type@@</div>
		<div>@@Description@@</div>
		<div><img src="@@ImageUrl@@" /></div>
	</div>
	<!-- @@POPUP_ITEMS@@ -->
	<!-- @@EMPTY_POPUP@@ -->
	<div class="popup-item">
		<div>No items were selected for display</div>
	</div>
<!-- @@EMPTY_POPUP@@ -->
</div>
<!-- @@POPUP@@ -->