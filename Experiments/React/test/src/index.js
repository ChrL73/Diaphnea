console.log('test');

let io = window.io;
let socket = io.connect('albertine:3002');

socket.on('displayPage', function(data)
{
  console.log(data);
});
