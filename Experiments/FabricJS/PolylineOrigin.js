var canvas = new fabric.Canvas('canvas');

canvas.hoverCursor = 'default';
canvas.selection = false;

var width = 800;
var height = 600;

var array1 = [ { x: 3, y: -1.5 }, { x: 0, y: -1 }, { x: 3, y: -0.5 } ];
var array2 = [ { x: 3, y: 0.5 }, { x: 0, y: 1 }, { x: 3, y: 1.5 } ];

var scale = 100;

var options1 =
{
   strokeLineCap: 'round',
   strokeLineJoin: 'round',
   fill: '',
   stroke: 'red',
   scaleX: scale,
   scaleY: scale,
   strokeWidth: 0.5,
   hasControls: false,
   hasBorders: false,
   lockMovementX: true,
   lockMovementY: true,
};

var options2 =
{
   strokeLineCap: 'round',
   strokeLineJoin: 'miter',
   fill: '',
   stroke: 'blue',
   scaleX: scale,
   scaleY: scale,
   strokeWidth: 0.5,
   hasControls: false,
   hasBorders: false,
   lockMovementX: true,
   lockMovementY: true
};

var line1 = new fabric.Polyline(array1, options1);
var line2 = new fabric.Polyline(array2, options2);

line1.left = (line1.left - 0.5 * line1.strokeWidth) * scale + 0.5 * width;
line1.top = (line1.top - 0.5 * line1.strokeWidth) * scale + 0.5 * height;
line2.left = (line2.left - 0.5 * line2.strokeWidth) * scale + 0.5 * width;
line2.top = (line2.top - 0.5 * line2.strokeWidth) * scale + 0.5 * height;

canvas.add(line1);
canvas.add(line2);

var lineOptions = { stroke: 'black', hasControls: false, hasBorders: false, lockMovementX: true, lockMovementY: true };
var lineX0 = new fabric.Line([width * 0.5, 0, width * 0.5, height], lineOptions);
canvas.add(lineX0);
var lineY1 = new fabric.Line([0, height * 0.5 - scale, width, height * 0.5 - scale], lineOptions);
canvas.add(lineY1);
var lineY2 = new fabric.Line([0, height * 0.5 + scale, width, height * 0.5 + scale], lineOptions);
canvas.add(lineY2);

canvas.renderAll();


