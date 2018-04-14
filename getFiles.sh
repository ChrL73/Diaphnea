#!/bin/bash -v
cp Map/NodeJS/API/map_api.js Quiz/React/quiz/public/
cp Map/NodeJS/API/map_api.js Quiz/NodeJS/public/
cp Quiz/VisualStudio/QuestionInstantiation/XmlQuizData.cs Map/VisualStudio/MapColors/
cp Quiz/VisualStudio/QuestionInstantiation/XmlQuizData.cs Quiz/Mono/GenerateMakefile/
wget -O Quiz/React/quiz/public/jquery.min.js https://ajax.googleapis.com/ajax/libs/jquery/1.12.4/jquery.min.js
wget -O Quiz/NodeJS/public/jquery.min.js https://ajax.googleapis.com/ajax/libs/jquery/1.12.4/jquery.min.js
cp /usr/share/fonts/truetype/msttcorefonts/arial.ttf Map/NodeJS