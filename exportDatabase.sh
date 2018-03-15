#!/bin/bash -v
set -e
mkdir -p tmp/databaseExport
rm -rf tmp/databaseExport
mongoexport --db diaphnea --collection questionnaires --out tmp/databaseExport/questionnaires.json
mongoexport --db diaphnea --collection levels --out tmp/databaseExport/levels.json
mongoexport --db diaphnea --collection choice_lists --out tmp/databaseExport/choice_lists.json
mongoexport --db diaphnea --collection question_lists --out tmp/databaseExport/question_lists.json
mongoexport --db diaphnea --collection maps --out tmp/databaseExport/maps.json
mongoexport --db diaphnea --collection polygon_elements --out tmp/databaseExport/polygon_elements.json
mongoexport --db diaphnea --collection point_elements --out tmp/databaseExport/point_elements.json
mongoexport --db diaphnea --collection line_elements --out tmp/databaseExport/line_elements.json
mongoexport --db diaphnea --collection items --out tmp/databaseExport/items.json
mongoexport --db diaphnea --collection point_lists --out tmp/databaseExport/point_lists.json