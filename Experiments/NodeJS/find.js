var dbs = db.adminCommand("listDatabases");
printjson(dbs);

db = connect("testDb");

var documents = db.levels2560.find();

var i = 0;
documents.forEach(function(document)
{
   printjson(document);
   ++i;
});

print(i);
