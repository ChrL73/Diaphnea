var canvas = new fabric.Canvas('canvas');

canvas.hoverCursor = 'default';
canvas.selection = false;

var _scale = 40;
var _strokeWidth = 0.1;
var _xFocus = 0;
var _yFocus = 0;
var _sizeInMapUnit = 1;
var _mapWidth = 800;
var _mapHeight = 600;

var _zoomMinDistance = 5;
var _zoomMaxDistance = 100;

var options =
{
   strokeWidth: _strokeWidth,
   strokeLineCap: 'round',
   strokeLineJoin: 'round',
   fill: ''
};

var array1 = [ { x: 0, y: 1 }, { x: -1, y: 0 }, { x: -1, y: -1 }, { x: 0, y: -2 }, { x: 1, y: -2 }, { x: 2, y: -1 }, { x: 2, y: 1}, { x: 0, y: 3} ];
var array2 = [ { x: 1, y: -1 }, { x: 1, y: 0 }, { x: 0, y: 2 }, { x: -3, y: 0 }, { x: 0, y: -3 } ];

var line1 = new fabric.Polyline(array1, options);
var line2 = new fabric.Polyline(array2, options);

line1.stroke = 'red';
line2.stroke = 'blue';

var group = new fabric.Group([ line1, line2 ], { hasControls: false, hasBorders: false, lockMovementX: true, lockMovementY: true });

canvas.add(group);

var top0 = group.top;
var left0 = group.left;

setFocus(0, 0, 10, 10);

var mustTranslate = false;
var xRef, yRef;

canvas.on('mouse:down', function(arg)
{
   xRef = arg.e.clientX;
   yRef = arg.e.clientY;
   mustTranslate = true;
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

canvas.on('mouse:up', function(arg)
{
   mustTranslate = false;
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
   
   group.top = (top0 - _yFocus) * _scale + 0.5 * _mapHeight;
   group.left = (left0 - _xFocus) * _scale + 0.5 * _mapWidth;
   
   group.scaleX = _scale;
   group.scaleY = _scale;

   canvas.renderAll();
}

