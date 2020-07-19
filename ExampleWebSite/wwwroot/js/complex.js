﻿/* ****************************************************************************
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

(function complexPageScript() {

    $(document).ready(function () {
        bindEvents();
    });

    function bindEvents() {
        $("#typeSelector").change(changePageScope);
        $("#selectButton").click(postSelections);
    }

    function changePageScope() {
        master.postForm("Complex/ChangePageScope",
            function (data, textStatus, jqXHR) {
                $(".items-div").html(data.html);
            },
            function (xhr, textStatus, errorThrown) {
                master.openErrorDialog({ title: "Filter By Item Type" }, "An error occurred.");
            })
    }

    function postSelections() {
        master.postForm("Complex/Select",
            function (data, textStatus, jqXHR) {
                $(".modal-popup").html(data.html);
                $(".modal-popup").dialog({ title: "Selected Items.", modal: true, maxHeight: 620, minWidth: 620 });
            },
            function (xhr, textStatus, errorThrown) {
                master.openErrorDialog({ title: "Posting selected items failed.", modal: true, width: 600 }, "An error occurred.");
            },
            function (formData) {
                $(".item-container").each(function (index, item) {
                    let thisItem = $(item);

                    if (thisItem.find("[name='Selected']").first().is(":checked"))
                        formData.append("itemNames[]", thisItem.find("[name='Name']").first().html());
                });
            });
    }

})();
