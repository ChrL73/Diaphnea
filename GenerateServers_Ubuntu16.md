March 2018

* This procedure has been tested for the following operating systems:

	* `ubuntu-16.04.4-server-amd64.iso` (downloaded on [https://www.ubuntu.com/download/server/](https://www.ubuntu.com/download/server/))

	* `kubuntu-16.04.3-desktop-ams64.iso` (downloaded on [https://kubuntu.org/getkubuntu/](https://kubuntu.org/getkubuntu/))

* If the computer is behind a proxy:

	* Edit the file `/etc/profile` and add the lines:

			export https_proxy=http://userName:password@proxyAddress:portNumber
			export http_proxy=http://userName:password@proxyAddress:portNumber

	* Apply the modifications:

			source /etc/profile

	* Create the file `/etc/apt/apt.conf.d/proxyPerso.conf` and add the line:

			Acquire::http::proxy "http://userName:password@proxyAddress:portNumber/";

	* Edit the file `/etc/wgetrc` and add the lines:

			https_proxy = http://userName:password@proxyAddress:portNumber/
			http_proxy = http://userName:password@proxyAddress:portNumber/
			use_proxy = on

* Update apt-get repositories:

		sudo apt-get update

* Install git:

		sudo apt-get install git

* If the computer is behind a proxy:

		git config --global http.proxy http://userName:password@proxyAddress:portNumber 

* Clone the git repository:

		git clone https://github.com/ChrL73/Diaphnea.git

* Install MongoDB:

		sudo apt-get install mongodb

* Install Mono (see [https://www.mono-project.com/download/stable/#download-lin](https://www.mono-project.com/download/stable/#download-lin) for more information):

		sudo -E apt-key adv --keyserver hkp://keyserver.ubuntu.com:80 --recv-keys 3FA7E0328081BFF6A14DA29AA6A19B38D3D831EF
		echo "deb http://download.mono-project.com/repo/ubuntu stable-xenial main" | sudo tee /etc/apt/sources.list.d/mono-official-stable.list
		sudo apt-get update
		sudo apt-get install mono-complete

	Verify that a correct version has been installed:

		mono --version

	The version must be >= 5.10 (some older versions have a bug that prevents them from working with the MongoDB C# driver)

* Install MonoDevelop:

		sudo apt-get install monodevelop

* Download the MongoDB C# driver 2.3.0 (`CSharpDriver-2.3.0.zip` on the page [https://github.com/mongodb/mongo-csharp-driver/releases](https://github.com/mongodb/mongo-csharp-driver/releases)):

		wget https://github.com/mongodb/mongo-csharp-driver/releases/download/v2.3.0/CSharpDriver-2.3.0.zip

* Unzip the archive `CSharpDriver-2.3.0.zip`

		sudo apt-get install unzip
		unzip CSharpDriver-2.3.0.zip -d CSharpDriver-2.3.0

* Copy the 3 files `MongoDB.Bson.dll` `MongoDB.Driver.Core.dll` and `MongoDB.Driver.dll` to the 2 folders `Diaphnea/Quiz/Mono/` and  `Diaphnea/Map/Mono/`:

		cp CSharpDriver-2.3.0/net45/MongoDB.Bson.dll Diaphnea/Quiz/Mono/
		cp CSharpDriver-2.3.0/net45/MongoDB.Driver.Core.dll Diaphnea/Quiz/Mono/
		cp CSharpDriver-2.3.0/net45/MongoDB.Driver.dll Diaphnea/Quiz/Mono/
		cp CSharpDriver-2.3.0/net45/MongoDB.Bson.dll Diaphnea/Map/Mono/
		cp CSharpDriver-2.3.0/net45/MongoDB.Driver.Core.dll Diaphnea/Map/Mono/
		cp CSharpDriver-2.3.0/net45/MongoDB.Driver.dll Diaphnea/Map/Mono/

* Build the Mono project `QuestionInstantiation`:

		cd ~/Diaphnea/Quiz/Mono/QuestionInstantiation
		mdtool build QuestionInstantiation.csproj

* Execute `QuestionInstantiation` to generate the Quiz database:

		cd bin/Debug
		chmod u+x QuestionInstantiation.exe
		./QuestionInstantiation.exe ../../../../VisualStudio/QuestionInstantiation/QuizData_France.xml ../../../../VisualStudio/QuestionInstantiation/QuizData_Gabon.xml

* Build the Mono project `MapDataProcessing`:

		cd ~/Diaphnea/Map/Mono/MapDataProcessing
		mdtool build MapDataProcessing.csproj

* Execute `MapDataProcessing` to generate the Map database:

		cd bin/Debug
		chmod u+x MapDataProcessing.exe
		./MapDataProcessing.exe ../../../../VisualStudio/MapDataProcessing/MapData_France.xml ../../../../VisualStudio/MapDataProcessing/MapData_Gabon.xml

* Install the GCC compilers:

		sudo apt-get install build-essential

* Install Boost (required to compile the MongoDB C++ driver):

		sudo apt-get install libboost-all-dev

* Install Python 2.x (required to compile the MongoDB C++ driver):

		sudo apt-get install python

* Install SCons (required to compile the MongoDB C++ driver):

		sudo apt-get install scons

* Get the Legacy MongoDB C++ driver sources (required for the quiz server and for the map server):

		cd
		git clone -b releases/legacy https://github.com/mongodb/mongo-cxx-driver.git

* Compile the MongoDB C++ driver (see [https://mongodb.github.io/mongo-cxx-driver/legacy-v1/installation/](https://mongodb.github.io/mongo-cxx-driver/legacy-v1/installation/) for more information):

		cd mongo-cxx-driver/
		scons --disable-warnings-as-errors --c++11=on --prefix=$HOME/Diaphnea/mongo-cxx-driver install

* Download the `FreeType` library (file `freetype-2.7.tar.gz` on the page [https://download.savannah.gnu.org/releases/freetype/](https://download.savannah.gnu.org/releases/freetype/) (required for the map server):

		cd
		wget https://download.savannah.gnu.org/releases/freetype/freetype-2.7.tar.gz

* Extract the files from the tar.gz file:

		tar -xvzf freetype-2.7.tar.gz

* Run the following commands to compile and install the library:

		cd freetype-2.7
		make
		sudo make install

* Install libpng (required for the png++ library):

		sudo apt-get install libpng-dev

* Download the `png++` library (file `png++-0.2.9.tar.gz` on the page [http://download.savannah.nongnu.org/releases/pngpp/](http://download.savannah.nongnu.org/releases/pngpp/) (required for the map server):

		cd
		wget http://download.savannah.nongnu.org/releases/pngpp/png++-0.2.9.tar.gz

* Extract the files from the tar.gz file:

		tar -xvzf png++-0.2.9.tar.gz

* Run the following commands to compile and install the library:

		cd png++-0.2.9
		make
		make test
		sudo make install

	The `make` command can produce the following message: `pixel_generator.cpp:35:19: fatal error: png.hpp: No such file or directory`. Despite this message, it works. See [http://www.nongnu.org/pngpp/doc/0.2.9/](http://www.nongnu.org/pngpp/doc/0.2.9/) for more information

* Build the `produce_questions` project:

		cd ~/Diaphnea/Quiz/NodeJS/produce_questions
		./rebuild.sh

* Build the `map_server` project:

		cd ~/Diaphnea/Map/NodeJS/map_server
		./rebuild.sh

* Install Microsoft fonts:

		sudo apt-get install ttf-mscorefonts-installer

	Verify that the fonts has been installed:

		ls /usr/share/fonts/truetype/msttcorefonts

	This command must print a list of .ttf file names. Sometimes, this is not the case because the installation failed.

	In this case: Note that the only need of the map server is the presence of the file `arial.ttf` (with a lower case initial `a`) in the folder `/usr/share/fonts/truetype/msttcorefonts/`. You can just copy this file from the Internet or from a Windows computer (folder `Windows/Fonts`). If necessary, rename `Arial.ttf` to `arial.ttf`. If `arial.ttf` is not present, the map server will work, but no text will appear on the map.

* Install curl

		sudo apt-get install curl

* Install NodeJS and npm:

		curl -sL https://deb.nodesource.com/setup_9.x | sudo -E bash -
		sudo apt-get install nodejs

	Verify that correct versions have been installed:

		node -v
		npm -v

	node version must be >= v9.8.0 and npm version must be >= 5.6.0

* If the computer is behind a proxy:

		npm config set proxy http://userName:password@proxyAddress:portNumber 

* Go to the folder `Diaphnea` and run the command:

		cd ~/Diaphnea
		./getFiles.sh

* Install the npm packages the map server depends on:

		cd ~/Diaphnea/Map/NodeJS
		npm install

* Install the npm packages the quiz server back-end depends on:

		cd ~/Diaphnea/Quiz/NodeJS
		npm install

* Install the npm packages the quiz server front-end depends on and build the front-end:

		cd ~/Diaphnea/Quiz/React/quiz
		npm install
		npm run build
		./postBuild.sh

* Run the Map server:

		cd ~/Diaphnea/Map/NodeJS
		node app.js

* Run the Quiz server:

		cd ~/Diaphnea/Quiz/NodeJS
		node app.js
