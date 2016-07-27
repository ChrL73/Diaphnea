var childProcess = require('child_process');

childProcess.exec('./bin/Debug/CodeBlocksExperiment Christophe', function(err, stdout, stderr)
{
   console.log('childProcess.exec: ' + stdout);
});

var child = childProcess.spawn('./bin/Debug/CodeBlocksExperiment', ['Christophe']);     
child.stdout.on('data', function(data)
{
   console.log('childProcess.spawn: ' + data);
});
