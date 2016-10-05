var canvas = new fabric.Canvas('canvas');

canvas.hoverCursor = 'default';
canvas.selection = false;

var _scale;
var _xFocus;
var _yFocus;
var _sizeInMapUnit;
var _mapWidth = 800;
var _mapHeight = 600;

var _zoomMinDistance = 0.1;
var _zoomMaxDistance = 500;

var _line1Size = 4;
var _line2Size = 1;
var _sizeParameter1 = 6;
var _sizeParameter2 = 8;

var options =
{
   strokeLineCap: 'round',
   strokeLineJoin: 'round',
   fill: '',
   hasControls: false, hasBorders: false, lockMovementX: true, lockMovementY: true
};

var array1 = [ { x: 0, y: 1 }, { x: -1, y: 0 }, { x: -1, y: -1 }, { x: 0, y: -2 }, { x: 1, y: -2 }, { x: 2, y: -1 }, { x: 2, y: 1}, { x: 0, y: 3} ];
var array2 = [ { x: 1, y: -1 }, { x: 1, y: 0 }, { x: 0, y: 2 }, { x: -3, y: 0 }, { x: 0, y: -3 } ];

var line1 = new fabric.Polyline(array1, options);
var line2 = new fabric.Polyline(array2, options);

line1.stroke = 'red';
line2.stroke = 'blue';

// 'group' does not work properly when lines have different strokeWidths:
// Property 'top' (or 'left') is calculated in a different way for 'line1' and 'line2' in the function 'updateFocus' .
//var group = new fabric.Group([ line1, line2 ], { hasControls: false, hasBorders: false, lockMovementX: true, lockMovementY: true });

//canvas.add(group);
canvas.add(line1);
canvas.add(line2);

var lineOptions = { stroke: 'black', hasControls: false, hasBorders: false, lockMovementX: true, lockMovementY: true };
var lineX0 = new fabric.Line([_mapWidth * 0.5, 0, _mapWidth * 0.5, _mapHeight], lineOptions);
canvas.add(lineX0);
var lineY0 = new fabric.Line([0, _mapHeight * 0.5, _mapWidth, _mapHeight * 0.5], lineOptions);
canvas.add(lineY0);

//var top0 = group.top;
//var left0 = group.left;
var top1_0 = line1.top;
var left1_0 = line1.left;
var top2_0 = line2.top;
var left2_0 = line2.left;

setFocus(0, 0, 10, 10);

var mustTranslate = false;
var xRef, yRef;

canvas.on('mouse:down', function(arg)
{
   xRef = arg.e.clientX;
   yRef = arg.e.clientY;
   mustTranslate = true;
   canvas.defaultCursor = 'move';   
});

canvas.on('mouse:move', function(arg)
{
   if (mustTranslate && (arg.e.buttons & 1))
   {
      var dx = arg.e.clientX - xRef;
      var dy = arg.e.clientY - yRef;

      _xFocus -= dx / _scale;
      _yFocus -= dy / _scale;
      updateFocus();

      xRef = arg.e.clientX;
      yRef = arg.e.clientY;
   }
});

canvas.on('mouse:up', function(arg)
{
   mustTranslate = false;
   canvas.defaultCursor = 'default';
});

canvas.on('mouse:wheel', function(arg)
{
   var delta = arg.e.deltaY;
   var sizeInMapUnit0 = _sizeInMapUnit;

   if (delta > 0) _sizeInMapUnit *= 1.15;
   else _sizeInMapUnit /= 1.15;

   if (_sizeInMapUnit < _zoomMinDistance) _sizeInMapUnit = _zoomMinDistance;
   else if (_sizeInMapUnit > _zoomMaxDistance) _sizeInMapUnit = _zoomMaxDistance;

   if (_sizeInMapUnit != sizeInMapUnit0)
   {   
      var zoomFactor = _sizeInMapUnit / sizeInMapUnit0;
      if (delta > 0)
      {
         _xFocus += (0.5 * _mapWidth - arg.e.clientX) * (1.0 - 1.0 / zoomFactor) / _scale;
         _yFocus += (0.5 * _mapHeight - arg.e.clientY) * (1.0 - 1.0 / zoomFactor) / _scale;
      }
      else
      {
         _xFocus += (0.5 * _mapWidth - arg.e.clientX) * (zoomFactor - 1.0) / _scale;
         _yFocus += (0.5 * _mapHeight - arg.e.clientY) * (zoomFactor - 1.0) / _scale;
      }

      updateFocus();
   }
});

function setFocus(x, y, widthInMapUnit, heightInMapUnit)
{
   _xFocus = x;
   _yFocus = y;

   if (widthInMapUnit * _mapHeight > heightInMapUnit * _mapWidth)
   {
       var a = _mapHeight / _mapWidth;
       _sizeInMapUnit = widthInMapUnit * Math.sqrt(1.0 + a * a);
   }
   else
   {
       var a = _mapWidth / _mapHeight;
       _sizeInMapUnit = heightInMapUnit * Math.sqrt(1.0 + a * a);
   }

   if (_sizeInMapUnit < _zoomMinDistance) _sizeInMapUnit = _zoomMinDistance;
   
   updateFocus();
}

function updateFocus()
{
   _scale = Math.sqrt(_mapWidth * _mapWidth + _mapHeight * _mapHeight) / _sizeInMapUnit;  
   var sizeFactor = _sizeParameter1 / (_sizeParameter2 + _scale);
   
   line1.strokeWidth = _line1Size * sizeFactor;
   line2.strokeWidth = _line2Size * sizeFactor;
   
   /*group.top = (top0 - 0.5 * line2.strokeWidth - _yFocus) * _scale + 0.5 * _mapHeight;   
   group.left = (left0 - 0.5 * line2.strokeWidth - _xFocus) * _scale + 0.5 * _mapWidth;
   
   group.scaleX = _scale;
   group.scaleY = _scale;*/
   
   line1.top = (top1_0 - 0.5 * line1.strokeWidth - _yFocus) * _scale + 0.5 * _mapHeight;   
   line1.left = (left1_0 - 0.5 * line1.strokeWidth - _xFocus) * _scale + 0.5 * _mapWidth;
   line1.scaleX = _scale;
   line1.scaleY = _scale;
   
   line2.top = (top2_0 - 0.5 * line2.strokeWidth - _yFocus) * _scale + 0.5 * _mapHeight;   
   line2.left = (left2_0 - 0.5 * line2.strokeWidth - _xFocus) * _scale + 0.5 * _mapWidth;
   line2.scaleX = _scale;
   line2.scaleY = _scale;

   canvas.renderAll();
}

