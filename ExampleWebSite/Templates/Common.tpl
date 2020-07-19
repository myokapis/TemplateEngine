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

<!-- This template contains a common partial view. -->

<!-- @@SELECTOR@@ -->
<form id="page-scope" action="" method="post" class="page-scope">
	<div class="selector-div">
		<div>Filter Options:</div>
		<select id="typeSelector" name="pageScope.ItemType">
			<!-- @@ITEM_TYPES@@ --><option value="@@VALUE@@" @@SELECTED@@>@@TEXT@@</option><!-- @@ITEM_TYPES@@ -->
		</select>
    </div>
</form>
<!-- @@SELECTOR@@ -->