#!/bin/bash
cp Map/NodeJS/API/map_api.js Quiz/React/quiz/public/
cp Map/NodeJS/API/map_api.js Quiz/NodeJS/public/
wget -O Quiz/React/quiz/public/jquery.min.js https://ajax.googleapis.com/ajax/libs/jquery/1.12.4/jquery.min.js
wget -O Quiz/NodeJS/public/jquery.min.js https://ajax.googleapis.com/ajax/libs/jquery/1.12.4/jquery.min.js
wget -O Quiz/NodeJS/public/bootstrap.min.css https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap.min.css
wget -O Quiz/NodeJS/public/bootstrap.min.js https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/js/bootstrap.min.js
