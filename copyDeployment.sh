#!/bin/bash -v
set -e
rm -rf ../Deployment
mkdir -p ../Deployment/Map/NodeJS
mkdir -p ../Deployment/Quiz/NodeJS/languages
mkdir -p ../Deployment/Quiz/NodeJS/views
mkdir -p ../Deployment/database
mkdir -p ../Deployment/lib
cp Map/NodeJS/*.js Map/NodeJS/map_server.exe Map/NodeJS/package.json Map/NodeJS/arial.ttf ../Deployment/Map/NodeJS
cp Quiz/NodeJS/languages/*.js ../Deployment/Quiz/NodeJS/languages/
cp -R Quiz/NodeJS/public ../Deployment/Quiz/NodeJS/
cp Quiz/NodeJS/views/reactIndex.ejs ../Deployment/Quiz/NodeJS/views/reactIndex.ejs
cp Quiz/NodeJS/*.js Quiz/NodeJS/produce_questions.exe Quiz/NodeJS/package.json ../Deployment/Quiz/NodeJS
cp tmp/databaseExport/*.json ../Deployment/database/
echo "#!/bin/bash -v" >> ../Deployment/database/importDatabase.sh
echo "set -e" >> ../Deployment/database/importDatabase.sh
echo "mongoimport --db diaphnea --drop --collection questionnaires --file questionnaires.json" >> ../Deployment/database/importDatabase.sh
echo "mongoimport --db diaphnea --drop --collection levels --file levels.json" >> ../Deployment/database/importDatabase.sh
echo "mongoimport --db diaphnea --drop --collection choice_lists --file choice_lists.json" >> ../Deployment/database/importDatabase.sh
echo "mongoimport --db diaphnea --drop --collection question_lists --file question_lists.json" >> ../Deployment/database/importDatabase.sh
echo "mongoimport --db diaphnea --drop --collection maps --file maps.json" >> ../Deployment/database/importDatabase.sh
echo "mongoimport --db diaphnea --drop --collection polygon_elements --file polygon_elements.json" >> ../Deployment/database/importDatabase.sh
echo "mongoimport --db diaphnea --drop --collection point_elements --file point_elements.json" >> ../Deployment/database/importDatabase.sh
echo "mongoimport --db diaphnea --drop --collection line_elements --file line_elements.json" >> ../Deployment/database/importDatabase.sh
echo "mongoimport --db diaphnea --drop --collection items --file items.json" >> ../Deployment/database/importDatabase.sh
echo "mongoimport --db diaphnea --drop --collection point_lists --file point_lists.json" >> ../Deployment/database/importDatabase.sh
chmod u+x ../Deployment/database/importDatabase.sh
cp /usr/lib/x86_64-linux-gnu/libboost_regex.so.1.58.0 ../Deployment/lib/
cp /usr/lib/x86_64-linux-gnu/libboost_system.so.1.58.0 ../Deployment/lib/
cp /usr/lib/x86_64-linux-gnu/libboost_thread.so.1.58.0 ../Deployment/lib/