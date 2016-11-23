var canvas = new fabric.Canvas('canvas');

canvas.hoverCursor = 'default';
canvas.selection = false;

var rectangles = [];

var i, n = 5;
for (i = 0; i < n; ++i)
{
   var r =   new fabric.Rect(
   {
      left: 100 + 20 * i,
      top: 50 + 20 * i,
      fill: 'rgb(' + (i * 255 / n) + ', ' + (255 - i * 255 / n) + ', 192)',
      width: 100,
      height: 100,
      hasControls: false, hasBorders: false, lockMovementX: true, lockMovementY: true, selectable: false
   });
   
   rectangles.push(r);
   canvas.add(r);
}

for (i = 0; i < n; ++i)
{
   $('#list').append('<p> ' + i + ' <input type="text" id="i' + i + '"></input><button id="b' + i + '">Apply</button><span id="s' + i + '">'
                     + canvas.getObjects().indexOf(rectangles[i]) + '</span></p>');
   
   $('#b' + i).click(f(i));
   
   function f(j)
   {
      return function()
      {
         var index = Number($('#i' + j).val());
         if (index || index === 0)
         {
            canvas.moveTo(rectangles[j], index);
            var k;
            for (k = 0; k < n; ++k)
            {
               $('#s' + k).text(canvas.getObjects().indexOf(rectangles[k]));
            }
         }
      };
   }
}
