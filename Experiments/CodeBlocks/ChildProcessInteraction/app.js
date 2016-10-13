var childProcess = require('child_process');

var child;

var i;
for (i = 0; i < 30; ++i)
{
   childWrite(i.toString());
}

// Crash node to verify that in this case, the spawned process terminates
//setTimeout(function() { unknown_var; }, 1000);

childWrite('q');

setTimeout(f, 500);

function f()
{
   console.log('try to speak to terminated process...');
   childWrite('50');
}

function childWrite(str, recursiveCall)
{
   try
   {
      child.stdin.write(str + '\n');
   }
   catch (err)
   {
      child = childProcess.spawn('./bin/Debug/ChildProcessInteraction');
      
      child.stdout.on('data', function(data)
      {
         var array = data.toString().split('\n');  
         console.log(array.length.toString() + ' responses received:');

         array.forEach(function(s)
         {
            console.log(s);
         });
      });
      
      if (recursiveCall) throw new Error(err);
      childWrite(str, true);
   }
}

