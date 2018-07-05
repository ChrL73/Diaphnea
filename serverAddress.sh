#!/bin/bash
serverAddress=albertine
sed -i -E "s/(http:\/\/)[^:]+(:)/\1$serverAddress\2/" Quiz/React/quiz/public/index.html
sed -i -E "s/(io.connect\(')[^:]+(:)/\1$serverAddress\2/" Quiz/React/quiz/src/index.js
#sed -i -E "s/(http:\/\/)[^:]+(:)/\1$serverAddress\2/" Experiments/NodeJS/CookieSession3/views/app2.ejs
#sed -i -E "s/(io.connect\(')[^:]+(:)/\1$serverAddress\2/" Experiments/NodeJS/CookieSession3/views/app2.ejs
sed -i -E "s/(http:\/\/)[^:]+(:)/\1$serverAddress\2/" Map/NodeJS/Examples/Bug.js
sed -i -E "s/(http:\/\/)[^:]+(:)/\1$serverAddress\2/" Map/NodeJS/Examples/MapExample1.js
sed -i -E "s/(http:\/\/)[^:]+(:)/\1$serverAddress\2/" Map/NodeJS/Examples/MapExample2.js
sed -i -E "s/(http:\/\/)[^:]+(:)/\1$serverAddress\2/" Map/NodeJS/Examples/MinimalExample.js