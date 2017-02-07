var canvas  = document.querySelector('#canvas');
var ctx = canvas.getContext('2d');
ctx.lineCap = 'round';
ctx.lineJoin = 'round';

ctx.moveTo(100, 100);
ctx.lineTo(500, 100);
ctx.lineTo(500, 500);
ctx.lineTo(100, 500);
ctx.lineTo(100, 100);
ctx.lineTo(200, 200);
ctx.lineTo(400, 200);
ctx.lineTo(400, 400);
ctx.lineTo(200, 400);
ctx.lineTo(200, 200);
ctx.lineTo(100, 100);

ctx.fillStyle = 'rgb(200, 200, 200)';

// Canvas fill rule 'evenodd' is necessary to see the opening inside the square 
ctx.fill('evenodd');
