#!/bin/bash
mongoimport --db diaphnea --drop --collection maps --file ../../Shared/maps.json
mongoimport --db diaphnea --drop --collection polygon_elements --file ../../Shared/polygon_elements.json
mongoimport --db diaphnea --drop --collection point_elements --file ../../Shared/point_elements.json
mongoimport --db diaphnea --drop --collection items --file ../../Shared/items.json
mongoimport --db diaphnea --drop --collection point_lists --file ../../Shared/point_lists.json
