## Diaphnea

Diaphnea is a geography quiz.

It is a web application.

An alpha version of the application is currently deployed on the [Heroku](https://www.heroku.com/) platform: [https://diaphnea.herokuapp.com](https://diaphnea.herokuapp.com)

The file [BuildServers_Ubuntu16.md](https://github.com/ChrL73/Diaphnea/blob/master/BuildServers_Ubuntu16.md) explains how to build and run the application in a development configuration on [Ubuntu](https://www.ubuntu.com/) 16.

The application consists of two web servers: the "Quiz server" and the "Map server".
The backends of these servers use [NodeJS](https://nodejs.org). They are written in Javascipt and C++.

The frontend of the application uses the [React](https://reactjs.org/) framework.

The source of the application contains a significant amount of C# code. This C# code is not executed at the application runtime, but before the compilation of the servers. The C# code performs various treatments and as a result of these treatments, it generates C++ code.

The application uses a [MondoDB](https://www.mongodb.com/) database.

