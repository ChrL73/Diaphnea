var childProcess = require('child_process');

var child = childProcess.spawn('./bin/Debug/ChildProcessInteraction');     
child.stdout.on('data', function(data)
{
   var array = data.toString().split('\n');  
   console.log(array.length.toString() + ' responses received:');
   
   array.forEach(function(s)
   {
      console.log(s);
   });
});
 
var i;
for (i = 0; i < 30; ++i)
{
   child.stdin.write(i.toString() + '\n');
}

//child.stdin.write('q\n');

// Crash node to verify that the spawned process terminates
setTimeout(function() {aaa;}, 5000);
