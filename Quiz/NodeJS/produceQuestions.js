process.stdin.on('data', function(jsonData)
{
   var data = JSON.parse(jsonData);
   
   process.stdout.write(JSON.stringify(data));
});
