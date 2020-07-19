<!DOCTYPE html>
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
<html>
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Example Web Site</title>
    <link rel="stylesheet" href="https://code.jquery.com/ui/1.12.1/themes/smoothness/jquery-ui.css" />
    <link rel="stylesheet" href="css/master.css" />
    <script src="https://code.jquery.com/jquery-1.12.4.min.js" type="text/javascript"></script>
    <script src="https://code.jquery.com/ui/1.12.1/jquery-ui.min.js" type="text/javascript"></script>
    <script src="js/master.js" type="text/javascript"></script>
    @@HEAD@@
</head>
<body>
    <div class="header-div">
        <div class="nav-button-container">
            <input type="button" class="nav-button" value="Home"/>
            <input type="button" class="nav-button" value="Simple"/>
            <input type="button" class="nav-button" value="Complex"/>
            <input type="button" class="nav-button" value="Docs"/>
            <input type="button" class="nav-button" value="About"/>
        </div>
    </div>
    <div class="body-div">
    @@BODY@@
    </div>
    <div class="footer-div">
        <div>Copyright: Gene Graves 2017-2022 </div>
    </div>
    @@TAIL@@
    <div id="errorDialog" style="display: none;">
        <p id="errorMessage"></p>
    </div>
</body>
</html>