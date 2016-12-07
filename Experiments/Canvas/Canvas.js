var _mapWidth = 800;
var _mapHeight = 600;
var _zoomMinDistance = 0.1;
var _zoomMaxDistance = 500;
var _sizeParameter1 = 6;
var _sizeParameter2 = 8;

var _scale;
var _xFocus;
var _yFocus;
var _sizeInMapUnit;

var canvas  = document.querySelector('#canvas');
var _context = canvas.getContext('2d');
_context.lineCap = 'round';
_context.lineJoin = 'round';
_context.textBaseline = 'bottom';

var array1 = [ { x: 0, y: 1 }, { x: -1, y: 0 }, { x: -1, y: -1 }, { x: 0, y: -2 }, { x: 1, y: -2 }, { x: 2, y: -1 }, { x: 2, y: 1}, { x: 0, y: 3} ];
var array2 = [ { x: 1, y: -1 }, { x: 1, y: 0 }, { x: 0, y: 2 }, { x: -3, y: 0 }, { x: 0, y: -3 } ];

var polyline1 =
{
   points: array1,
   size: 4,
   color: 'rgba(255, 30, 30, 0.8)'
};

var polyline2 =
{
   points: array2,
   size: 1,
   color: 'blue'
};

var polygon2 =
{
   points: array2,
   color: 'rgb(192, 224, 255)'
};

var point1 =
{
   x: 0,
   y: -1,
   color: 'rgb(0, 255, 255)',
   radius: 2,
   stroke: 0.8
};

var text1 =
{
   x: 0.1,
   y: -1.1,
   text: 'abfgpbdqW_A',
   color: 'rgb(0, 0, 255)',
   size: 5,
   font: 'arial',
   bgColor: 'rgba(0, 0, 255, 0.3)'
}

_context.save();
var helpSize = 40;
_context.font = helpSize + 'px ' + text1.font;
var measure = _context.measureText(text1.text);
text1.width = measure.width * text1.size / helpSize;
text1.height = _context.measureText('W').width * text1.size / helpSize;
_context.restore();

console.log(text1);

setFocus(1, 1, 10, 10);

var mustTranslate = false;
var xRef, yRef;

canvas.addEventListener('mousedown', function(e)
{
   xRef = e.clientX;
   yRef = e.clientY;
   mustTranslate = true;
   canvas.style.cursor = 'move';   
});

canvas.addEventListener('mousemove', function(e)
{
   if (mustTranslate && (e.buttons & 1))
   {
      var dx = e.clientX - xRef;
      var dy = e.clientY - yRef;

      _xFocus -= dx / _scale;
      _yFocus -= dy / _scale;
      updateFocus();

      xRef = e.clientX;
      yRef = e.clientY;
   }
});

canvas.addEventListener('mouseup', function(e)
{
   mustTranslate = false;
   canvas.style.cursor = 'default'; 
});

canvas.addEventListener('wheel', function(e)
{
   var delta = e.deltaY;
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
         _xFocus += (0.5 * _mapWidth - e.clientX) * (1.0 - 1.0 / zoomFactor) / _scale;
         _yFocus += (0.5 * _mapHeight - e.clientY) * (1.0 - 1.0 / zoomFactor) / _scale;
      }
      else
      {
         _xFocus += (0.5 * _mapWidth - e.clientX) * (zoomFactor - 1.0) / _scale;
         _yFocus += (0.5 * _mapHeight - e.clientY) * (zoomFactor - 1.0) / _scale;
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
     
   _context.clearRect(0, 0, canvas.width, canvas.height);
   //_context.save();
   //_context.translate(0.5 * _mapWidth - _xFocus * _scale, 0.5 * _mapHeight - _yFocus * _scale);
   //_context.scale(_scale, _scale);
   
   drawPolygon(polygon2);
   drawPolyline(polyline1, sizeFactor);
   drawPolyline(polyline2, sizeFactor);
   drawPoint(point1, sizeFactor);
   drawText(text1, sizeFactor);
   
   //_context.restore();
}

function drawPolyline(polyline, sizeFactor)
{
   _context.save();
   _context.translate(0.5 * _mapWidth - _xFocus * _scale, 0.5 * _mapHeight - _yFocus * _scale);
   _context.scale(_scale, _scale);
   
   _context.beginPath();
   polyline.points.forEach(function(p, i)
   {
      if (i == 0) _context.moveTo(p.x, p.y);
      else _context.lineTo(p.x, p.y);
      
      //var x = (p.x - _xFocus) * _scale + 0.5 * _mapWidth;
      //var y = (p.y - _yFocus) * _scale + 0.5 * _mapHeight;
      //if (i == 0) _context.moveTo(x, y);
      //else _context.lineTo(x, y);
   });
   
   _context.strokeStyle = polyline.color;
   _context.lineWidth = polyline.size * sizeFactor;
   //_context.lineWidth = polyline.size * sizeFactor * _scale;
   _context.stroke();
   
   _context.restore();
}

function drawPolygon(polygon)
{
   _context.save();
   _context.translate(0.5 * _mapWidth - _xFocus * _scale, 0.5 * _mapHeight - _yFocus * _scale);
   _context.scale(_scale, _scale);
   
   _context.beginPath();
   polygon.points.forEach(function(p, i)
   {
      if (i == 0) _context.moveTo(p.x, p.y);
      else _context.lineTo(p.x, p.y);
      
      //var x = (p.x - _xFocus) * _scale + 0.5 * _mapWidth;
      //var y = (p.y - _yFocus) * _scale + 0.5 * _mapHeight;
      //if (i == 0) _context.moveTo(x, y);
      //else _context.lineTo(x, y);
   });
   
   _context.fillStyle = polygon.color;
   _context.fill();
   
   _context.restore();
}

function drawPoint(point, sizeFactor)
{
   _context.beginPath();
   //_context.arc(point.x, point.y, point.radius * sizeFactor, 0, 2 * Math.PI);
   
   var x = (point.x - _xFocus) * _scale + 0.5 * _mapWidth;
   var y = (point.y - _yFocus) * _scale + 0.5 * _mapHeight;
   _context.arc(x, y, point.radius * sizeFactor * _scale, 0, 2 * Math.PI);
   
   _context.fillStyle = point.color;
   _context.fill();
   
   _context.strokeStyle = 'black';
   //_context.lineWidth = point.stroke * sizeFactor;
   _context.lineWidth = point.stroke * sizeFactor * _scale;
   _context.stroke();
}

function drawText(text, sizeFactor)
{
   var x = (text.x - _xFocus) * _scale + 0.5 * _mapWidth;
   var y = (text.y - _yFocus) * _scale + 0.5 * _mapHeight;
   var w = text.width * sizeFactor * _scale;
   var h = text.height * sizeFactor * _scale;
   
   _context.fillStyle = text.bgColor;
   _context.fillRect(x, y - h, w, h);
   
   _context.fillStyle = text.color;
   _context.font = text.size * sizeFactor * _scale + 'px ' + text.font;
   _context.fillText(text.text, x, y);
}
