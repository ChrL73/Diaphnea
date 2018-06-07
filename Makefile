all: makeQuiz makeMap Map/VisualStudio/MapColors/XmlQuizData.cs Quiz/React/quiz/public/map_api.js Quiz/NodeJS/public/map_api.js Quiz/NodeJS/public/jquery.min.js Quiz/React/quiz/public/jquery.min.js Map/NodeJS/arial.ttf

.PHONY: makeMap
makeMap: Map/Mono/GenerateMakefile/bin/Debug/GenerateMakefile.exe
	cd Map/Mono/GenerateMakefile/bin/Debug && mono GenerateMakefile.exe
	cd Map && $(MAKE)
	
.PHONY: makeQuiz
makeQuiz: Quiz/Mono/GenerateMakefile/bin/Debug/GenerateMakefile.exe
	cd Quiz/Mono/GenerateMakefile/bin/Debug && mono GenerateMakefile.exe
	cd Quiz && $(MAKE)

Map/Mono/GenerateMakefile/bin/Debug/GenerateMakefile.exe: Map/Mono/GenerateMakefile/GenerateMakefile.csproj Map/Mono/GenerateMakefile/GenerateMakefileMain.cs Map/Mono/GenerateMakefile/Rule.cs Map/Mono/GenerateMakefile/XmlMapData.cs
	cd Map/Mono/GenerateMakefile && mdtool build GenerateMakefile.csproj
	
Quiz/Mono/GenerateMakefile/bin/Debug/GenerateMakefile.exe: Quiz/Mono/GenerateMakefile/GenerateMakefile.csproj Quiz/Mono/GenerateMakefile/GenerateMakefileMain.cs Quiz/Mono/GenerateMakefile/Rule.cs Quiz/Mono/GenerateMakefile/XmlQuizData.cs
	cd Quiz/Mono/GenerateMakefile && mdtool build GenerateMakefile.csproj

Map/VisualStudio/MapColors/XmlQuizData.cs: Quiz/VisualStudio/QuestionInstantiation/XmlQuizData.cs
	cp Quiz/VisualStudio/QuestionInstantiation/XmlQuizData.cs Map/VisualStudio/MapColors/

Map/Mono/GenerateMakefile/XmlMapData.cs: Map/VisualStudio/MapDataProcessing/XmlMapData.cs
	cp Map/VisualStudio/MapDataProcessing/XmlMapData.cs Map/Mono/GenerateMakefile/

Map/VisualStudio/MapDataProcessing/XmlMapData.cs: Map/VisualStudio/MapDataProcessing/XmlMapData.xsd
	cd Map/VisualStudio/MapDataProcessing && xsd XmlMapData.xsd /classes

Quiz/Mono/GenerateMakefile/XmlQuizData.cs: Quiz/VisualStudio/QuestionInstantiation/XmlQuizData.cs
	cp Quiz/VisualStudio/QuestionInstantiation/XmlQuizData.cs Quiz/Mono/GenerateMakefile/

Quiz/VisualStudio/QuestionInstantiation/XmlQuizData.cs: Quiz/VisualStudio/QuestionInstantiation/XmlQuizData.xsd
	cd Quiz/VisualStudio/QuestionInstantiation && xsd XmlQuizData.xsd /classes

Quiz/React/quiz/public/map_api.js: Map/NodeJS/API/map_api.js
	cp Map/NodeJS/API/map_api.js Quiz/React/quiz/public/

Quiz/NodeJS/public/map_api.js: Map/NodeJS/API/map_api.js
	cp Map/NodeJS/API/map_api.js Quiz/NodeJS/public/

Quiz/NodeJS/public/jquery.min.js: Quiz/React/quiz/public/jquery.min.js
	cp Quiz/React/quiz/public/jquery.min.js Quiz/NodeJS/public/

Quiz/React/quiz/public/jquery.min.js:
	wget -O Quiz/React/quiz/public/jquery.min.js https://ajax.googleapis.com/ajax/libs/jquery/1.12.4/jquery.min.js

Map/NodeJS/arial.ttf: /usr/share/fonts/truetype/msttcorefonts/arial.ttf
	cp /usr/share/fonts/truetype/msttcorefonts/arial.ttf Map/NodeJS