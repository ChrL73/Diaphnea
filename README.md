## Diaphnea

Diaphnea is: A game, a geography quiz, a web application.

An alpha version of the application is currently deployed on the [Heroku](https://www.heroku.com/) platform: [https://diaphnea.herokuapp.com](https://diaphnea.herokuapp.com)

The file [BuildServers_Ubuntu16.md](https://github.com/ChrL73/Diaphnea/blob/master/BuildServers_Ubuntu16.md) explains how to build and run the application in a development configuration on [Ubuntu](https://www.ubuntu.com/) 16.

### Quick description of the application

Before starting a game, the player chooses a questionnaire and a difficulty level. In the current version, 2 questionnaires are available: "France" and "Gabon". Other questionnaires can be added in further versions.

A game consists in a set of questions about geography. These questions are randomly drawn from a large database of questions. After the player has answered a question, an interactive map appears to illustrate the question.

### Quick overview of the software design

The application consists of two servers: the "quiz server" and the "map server". The quiz server implements the game itself. The map server allows to include a map in the game.

The map server is completely independent from the quiz server. The map server could be used to include an interactive map component in any web page.

The 2 servers are written in JavaScript and C++. Each of these servers is a [NodeJS](https://nodejs.org) process that spawns one or several C++ processes.

The front-end of the application uses the [React](https://reactjs.org/) framework. This front-end consists in static files served by the quiz server. Once downloaded from the quiz server, the front-end communicates with two back-ends (quiz server and map server) using web sockets thanks to the [socket.io](https://socket.io/) library.

The source of the application contains C# code. This C# code is not executed at the application runtime, but before the compilation of the servers. The C# code performs a significant amount of processing and as a result of this processing, it generates C++ code.

The application uses a [MondoDB](https://www.mongodb.com/) database to store session data, user data and best score tables.
