#!/bin/bash
mongoimport --db diaphnea --drop --collection questionnaires --file ../../Shared/questionnaires.json
mongoimport --db diaphnea --drop --collection levels --file ../../Shared/levels.json
mongoimport --db diaphnea --drop --collection question_lists --file ../../Shared/question_lists.json
