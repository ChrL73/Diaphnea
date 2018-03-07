March 2018

`kubuntu-16.04.3-desktop-ams64.iso` (downloaded on [https://kubuntu.org/getkubuntu/](https://kubuntu.org/getkubuntu/))

* If the computer is behind a proxy:

	* Edit the file `/etc/profile` and add the line:

			export https_proxy=http://userName:password@proxyAddress:portNumber
			export http_proxy=http://userName:password@proxyAddress:portNumber

	* Edit the file `/etc/bash.bashrc` and add the line:

			export https_proxy=http://userName:password@proxyAddress:portNumber
			export http_proxy=http://userName:password@proxyAddress:portNumber

	* Edit the file `/etc/wgetrc` and add the 2 lines:

			https_proxy = http_proxy=http://userName:password@proxyAddress:portNumber/
			http_proxy = http_proxy=http://userName:password@proxyAddress:portNumber/
			use_proxy = on

	* Create the file `/etc/apt/apt.conf.d/proxyPerso.conf` and add the line:

			Acquire::http::proxy "http:///userName:password@proxyAddress:portNumber/";

	* Add the following environment variables:

			export HTTP_PROXY=http://userName:password@proxyAddress:portNumber
			export HTTPS_PROXY=http://userName:password@proxyAddress:portNumber

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

* Install Mono:

		sudo apt-get install mono-complete
		sudo apt-get install mono-reference-assemblies-4.0

* Install the MonoDevelop IDE:

		sudo apt-get install monodevelop

* Download the MongoDB C# driver 2.2.4 from [https://github.com/mongodb/mongo-csharp-driver/releases](https://github.com/mongodb/mongo-csharp-driver/releases): `CSharpDriver-2.2.4.zip`

* Extract the 3 following files from `CSharpDriver-2.2.4.zip`:

		MongoDB.Bson.dll
		MongoDB.Driver.Core.dll
		MongoDB.Driver.dll

	 and copy these files to the 2 following folders:

		Diaphnea/Quiz/Mono/
		Diaphnea/Map/Mono/

* Generate the Quiz database:

	* Open `Diaphnea/Quiz/Mono/Diaphnea.sln` with MonoDevelop

	* Build and execute the project `QuestionInstantiation`

* Generate the Map database:

	* Open `Diaphnea/Map/Mono/DiaphneaMap.sln` with MonoDevelop

	* Build and execute the project `MapDataProcessing`

* Install the GCC compilers:

		sudo apt-get install build-essential

* Install Boost (required to compile the MongoDB C++ driver):

		sudo apt-get install libboost-all-dev

* Install Python 2.x (required to compile the MongoDB C++ driver):

		sudo apt-get install python

* Install SCons (required to compile the MongoDB C++ driver):

		sudo apt-get install scons

* Get the Legacy MongoDB C++ driver sources (required for the quiz server and for the map server):

		git clone -b releases/legacy https://github.com/mongodb/mongo-cxx-driver.git

* Compile the MongoDB C++ driver (see [https://mongodb.github.io/mongo-cxx-driver/legacy-v1/installation/](https://mongodb.github.io/mongo-cxx-driver/legacy-v1/installation/) for more information):

		cd mongo-cxx-driver/
		scons --disable-warnings-as-errors --c++11=on --prefix=$HOME/Diaphnea/mongo-cxx-driver install

	(Adapt `--prefix` value if you cloned the Diaphnea repository in another folder than `$HOME`)

* Download `FreeType` library from [https://download.savannah.gnu.org/releases/freetype/](https://download.savannah.gnu.org/releases/freetype/) (required for the map server): `freetype-2.7.tar.gz`.

	Extract the files from the tar.gz file.

	Go to the `freetype-2.7` folder (folder where the files were extracted) and run the 2 following commands to compile and install the library:

		make
		sudo make install

* Install libpng (required for the png++ library):

		sudo apt-get install libpng-dev

* Download the `png++` library from [http://download.savannah.nongnu.org/releases/pngpp/](http://download.savannah.nongnu.org/releases/pngpp/) (required for the map server): `png++-0.2.9.tar.gz`

	Extract the files from the tar.gz file

	Go to the `png++-0.2.9` folder (folder where the files were extracted) and run the 3 following commands to compile and install the library:

		make
		make test
		sudo make install

	The first command can produce the following message: `pixel_generator.cpp:35:19: fatal error: png.hpp: No such file or directory`. Despite this message, it works. See [http://www.nongnu.org/pngpp/doc/0.2.9/](http://www.nongnu.org/pngpp/doc/0.2.9/) for more information

* Install the Code::Blocks IDE:

		sudo apt-get install codeblocks

* Open the project `Diaphnea/Quiz/NodeJS/produce_questions/produce_questions.cpb` with Code::Blocks

	Build the project in `Release` configuration. Verify that the file `Diaphnea/Quiz/NodeJS/produce_questions.exe` has been created

* Open the project `Diaphnea/Map/NodeJS/map_server/map_server.cpb` with Code::Blocks

	Build the project in `Release` configuration. Verify that the file `Diaphnea/Map/NodeJS/map_server.exe` has been created

* Install Microsoft fonts:

		sudo apt-get install ttf-mscorefonts-installer

	Sometimes, this does not work.

	In this case: Note that the only need of the map server is the presence of the file `arial.ttf` (with a lower case initial `a`) in the folder `/usr/share/fonts/truetype/msttcorefonts/`. You can just copy this file from the Internet or from a Windows computer (folder `Windows/Fonts`). If necessary, rename `Arial.ttf` to `arial.ttf`. If `arial.ttf` is not present, the map server will work, but no text will appear on the map.

* Install curl

		sudo apt-get install curl

* Install NodeJS and npm:

		curl -sL https://deb.nodesource.com/setup_9.x | sudo -E bash -
		sudo apt-get install nodejs

* If the computer is behind a proxy:

		npm config set proxy http://userName:password@proxyAddress:portNumber 

* Go to the folder `Diaphnea` and run  the command:

		./getFiles.sh

* Install the npm packages the map server depends on: Go to the folder `Diaphnea/Map/NodeJS` and run the command:

		npm install

* Install the npm packages the quiz server back-end depends on: Go to the folder `Diaphnea/Quiz/NodeJS` and run the command:

		npm install

* Install the npm packages the quiz server front-end depends on and build the front-end: Go to the folder `Diaphnea/Quiz/React/quiz` and run the 3 following commands:

		npm install
		npm run build
		./postBuild.sh

* Run the Map server: Go to the folder `Diaphnea/Map/NodeJS` and run the command:

		node app.js

* Run the Quiz server: Go to the folder `Diaphnea/Quiz/NodeJS` and run the command:

		node app.js
