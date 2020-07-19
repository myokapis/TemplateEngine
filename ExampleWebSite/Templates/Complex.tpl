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
    <link rel="stylesheet" href="css/complex.css" />
	<script src="js/complex.js" type="text/javascript"></script>
<!-- @@HEAD@@ -->

<!-- The BODY section provides content to be rendered into the BODY field of the master template. -->
<!-- Body content is typically the html for the page. -->
<!-- @@BODY@@ -->
		<div class="content-section-div">
			<!-- This field is a placeholder for content that can be rendered from a registered provider. -->
			<!-- A registered provider is an independent writer that is bound to a template. -->
			<!-- The bound template does not have to be the current template or the master template. -->
			<!-- The independent writer is registered with the main writer as the source for a particular field in the template bound to the main writer. -->
			@@PAGE_SCOPE@@
			<div class="items-div">
				<!-- This part of the template has nested sections for rendering items using a different layout for each type of item -->
				<!-- Three levels of nested sections are required. -->
				<!-- The ITEM_BODY section is the main container for the generated content -->
				<!-- The ITEMS section is necessary in order to preserve the order of the items that are appended -->
				<!-- The ITEM_ANIMAL, ITEM_MINERAL, AND ITEM_VEGGIE sections define the layout for each kind of data -->
				<!-- @@ITEM_BODY@@ -->
				<!-- @@ITEMS@@ -->
				<!-- @@ITEM_ANIMAL@@ -->
				<div class="item-container">
					<h3>Animal</h3>
					<div class="item-headline">
						<div class="animal-headline">
							<input type="checkbox" name="Selected" @@Selected@@ />
							<div name="Name">@@Name@@</div>
						</div>
						<img src="@@ImageUrl@@" />
					</div>
					<div class="content">@@Description@@</div>
					<div class="content">@@FunFact@@</div>
				</div>
				<!-- @@ITEM_ANIMAL@@ -->
				<!-- @@ITEM_MINERAL@@ -->
				<div class="item-container">
					<h3>Mineral</h3>
					<div class="item-headline">
						<input type="checkbox" name="Selected" @@Selected@@ />
						<div name="Name">@@Name@@</div>
					</div>
					<div class="columns">
						<div class="content">@@Description@@</div>
						<div class="content">@@FunFact@@</div>
					</div>
					<img src="@@ImageUrl@@" />
				</div>
				<!-- @@ITEM_MINERAL@@ -->
				<!-- @@ITEM_VEGGIE@@ -->
				<div class="item-container">
					<h3>Vegetable</h3>
					<div class="item-headline">
						<input type="checkbox" name="Selected" @@Selected@@ />
						<div name="Name">@@Name@@</div>
					</div>
					<img src="@@ImageUrl@@" />
					<div class="content">@@Description@@</div>
					<div class="content">@@FunFact@@</div>
				</div>
				<!-- @@ITEM_VEGGIE@@ -->
				<!-- @@ITEMS@@ -->
				<!-- @@ITEM_BODY@@ -->
			</div>
			<div class="submit-div"><button type="button" id="selectButton">Select</button></div>
		</div>
		<div id="modalPopup" class="modal-popup"></div>
<!-- @@BODY@@ -->

<!-- The TAIL section is not present in this template; however, if it were present it would provide content for the -->
<!--	TAIL field in the master template.-->
<!-- Tail content is typically javascript or other content that needs to be appended to the end of the page HTML. -->

<!-- The POPUP section is a partial view. -->
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
