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

(function docsPageScript() {

    $(document).ready(function () {
        bindEvents();
        selectFirstTopic();
    });

    function bindEvents() {
        $("dt,dd").click(clickTopic);
    }

    function clickTopic() {
        // get the link
        let elem = $(this);
		let dataLink = elem.attr("data-link");

		// get the topic
		let topicElem = (elem.is("dt")) ? elem : 
			(elem.prev().is("dt")) ? elem.prev(): elem.prevUntil("dt").last().prev();

		let topic = topicElem.attr("data-link");

        // hide any topics that aren't already hidden
        $(".topic-text:not(.hidden)").addClass("hidden");

        // set the header text to the currently selected topic
        $("#topic").text(topic);

        // find the selected topic and unhide it
        let selectedTopic = $(".topic-text").filter("[data-link='" + topic + "']");
        selectedTopic.removeClass("hidden");

        // focus on the selected topic or subtopic
        let focusElement = (topic == dataLink) ? selectedTopic : 
			$("h4").filter("[data-link='" + dataLink + "']").first();

        if (focusElement) focusElement.get(0).scrollIntoView();
    }

    function selectFirstTopic() {
        $("dt").first().click();
    }

})();
