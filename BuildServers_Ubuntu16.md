## Procedure to build and run the servers (quiz server and map server) in a development configuration on Ubuntu 16

* This procedure is specific to Ubuntu 16 and will not work on other operationg systems. This procedure has been tested (in june and july 2018) on the folowing operating systems:

	* `kubuntu-16.04.4-desktop-amd64.iso` (downloaded on [https://kubuntu.org/getkubuntu/](https://kubuntu.org/getkubuntu/))

	* `ubuntu-16.04.4-desktop-amd64.iso` (downloaded on [http://releases.ubuntu.com/16.04/](http://releases.ubuntu.com/16.04/))

	* `ubuntu-16.04.4-server-amd64.iso` (downloaded on [http://releases.ubuntu.com/16.04/](http://releases.ubuntu.com/16.04/))

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

* Install Mono:

		sudo apt-get install mono-complete

* Install MonoDevelop:

		sudo apt-get install monodevelop

* Install the GCC compilers:

		sudo apt-get install build-essential

* Download the `FreeType` library (file `freetype-2.7.tar.gz` on the page [https://download.savannah.gnu.org/releases/freetype/](https://download.savannah.gnu.org/releases/freetype/)):

		cd
		wget https://download.savannah.gnu.org/releases/freetype/freetype-2.7.tar.gz

* Extract the files from the tar.gz file:

		tar -xvzf freetype-2.7.tar.gz

* Run the following commands to compile and install the library:

		cd freetype-2.7
		make
		sudo make install

* Install Microsoft fonts:

		sudo apt-get install ttf-mscorefonts-installer

	Verify that the fonts has been installed:

		ls /usr/share/fonts/truetype/msttcorefonts

	This command must print a list of .ttf file names. Sometimes, this is not the case because the installation failed.

	In this case: Note that the only need of the map server is the presence of the file `arial.ttf` (with a lower case initial `a`) in the folder `~/Diaphnea/Map/NodeJS`. You can just copy this file from the Internet or from a Windows computer (folder `Windows/Fonts`). If necessary, rename `Arial.ttf` to `arial.ttf`. If `arial.ttf` is not present, the map server will work, but no text will appear on the map.

* Build the C++ parts of the servers:

		cd ~/Diaphnea
		make

* Install curl

		sudo apt-get install curl

* Install NodeJS and npm:

		curl -sL https://deb.nodesource.com/setup_9.x | sudo -E bash -
		sudo apt-get install nodejs

	Verify that a correct version of NodeJS has been installed:

		node -v

	The version must be >= v9.11.2

* If the computer is behind a proxy:

		npm config set proxy http://userName:password@proxyAddress:portNumber

* Get the latest version of npm:

		sudo npm install -g npm@latest

	Verify the version:

		npm -v

	The version must be >= 6.1.0

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

* Test the application by typing the URL of the Quiz server in a Web browser (by default, the Quiz server runs on port 3002):

		http://<serverAddressOrName>:3002

	Examples of URLs:

		http://localhost:3002
		http://192.168.1.1:3002