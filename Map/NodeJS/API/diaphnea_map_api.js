var mapServerInterface =
{
   createNewConnection: function(url, onConnected)
   {
      var connection = new Connection(function()
      {
         onConnected(connection);
      });

      function Connection(onConnected)
      {   
         var mapIds;
         this.getMapIds = function() { return mapIds; }
         
         var socket;
         var requestCounter = -1;
            
         var contexts = {};
         function setContext(id, context)
         {
            contexts[id.toString()] = context;
            setTimeout(function() { delete contexts[id.toString()]; }, 20000);
         }
         
         function getContext(response, dontDelete)
         {
            var requestId = response.requestId.toString();
            var context = contexts[requestId];
            if (context && !dontDelete) delete contexts[requestId];
            return context;
         }

         $.getScript(url + '/socket.io/socket.io.js', function()
         {         
            socket = io(url);
            
            var id = ++requestCounter;
            var request = { id: id };
            socket.emit('mapIdsReq', request);
            
            socket.on('mapIdsRes', function(response)
            {
               if (!mapIds)
               {
                  mapIds = response.content;
                  onConnected();
               }
            });
         
            socket.on('mapInfoRes', function(response)
            {
               var context = getContext(response);
               if (context) context.onMapLoaded(new Map(context.mapId, context.canvasId, response.content));
            });
         });  
         
         this.loadMap = function(mapId, canvasId, onMapLoaded)
         {
            var id = ++requestCounter;
            var request = { id: id, mapId: mapId };
            setContext(id, { mapId: mapId, canvasId: canvasId, onMapLoaded: onMapLoaded });
            socket.emit('mapInfoReq', request);
         }
         
         function Map(mapId, canvasId, mapInfo)
         {
            var canvas = document.querySelector('#' + canvasId);
            var ctx = canvas.getContext('2d');
            ctx.lineCap = 'round';
            ctx.lineJoin = 'round';
            
            var reqAnimFrameId;
            
            var visibleElements = {};
            var items = {};
            var addedItems = {};
            var itemsToRemove = {};
            var removableTexts = {};
            var looks = {};
            var xFocus, yFocus, scale;
            
            var addedItemsByZIndex = [];
            var i;
            for (i = 0; i < 32; ++i) addedItemsByZIndex.push({});
            
            this.getLanguages = function() { return mapInfo.languages; };
            this.getName = function(languageId) { return mapInfo.names[languageId]; };
            this.getElementIds = function() { return mapInfo.elementIds; };  
            
            this.loadElement = function(elementId, onElementLoaded)
            {
               var id = ++requestCounter;
               var request = { id: id, mapId: mapId, elementId: elementId };
               setContext(id, { elementId: elementId, onElementLoaded: onElementLoaded });
               socket.emit('elementInfoReq', request);
            }
            
            socket.on('elementInfoRes', function(response)
            {
               var context = getContext(response);
               if (context) context.onElementLoaded(new Element(context.elementId, response.content));
            });
            
            this.loadElements = function(elementIds, onElementsLoaded)
            {
               var id = ++requestCounter;
               var request = { id: id, mapId: mapId, elementIds: elementIds };
               setContext(id, { elementIds: elementIds, onElementsLoaded: onElementsLoaded });
               socket.emit('elementsInfoReq', request);
            }
            
            socket.on('elementsInfoRes', function(response)
            {
               var context = getContext(response);
               if (!context) return;
               var elements = [];
               response.content.forEach(function(elementInfo, i)
               {
                  elements.push(new Element(context.elementIds[i], elementInfo));
               });
               context.onElementsLoaded(elements);
            });
               
            function Element(elementId, elementInfo)
            {
               this.getId = function() { return elementId; }
               this.getName = function(languageId) { return elementInfo.names[languageId]; }
               
               this.show = function()
               {
                  visibleElements[elementId] = true;
                  scheduleRenderRequest();
               }
               
               this.hide = function()
               {
                  visibleElements[elementId] = false;
                  var textItemKey = '_' + elementId;
                  var textItem = items[textItemKey];
                  if (textItem)
                  {
                     delete addedItems[textItemKey];
                     delete addedItemsByZIndex[textItem.zI][textItemKey];
                     delete removableTexts[textItemKey];
                  }
                  scheduleRenderRequest();
               }
            }
            
            var potentialImageRequested = false;
            this.requestPotentialImage = function()
            {
               potentialImageRequested = true;
               scheduleRenderRequest();
            }
            
            this.redraw = function()
            {
               scheduleRenderRequest();
               renderCanvas();
            }
            
            var renderRequestTimeout;
            function scheduleRenderRequest()
            {
               clearTimeout(renderRequestTimeout);
               renderRequestTimeout = setTimeout(sendRenderRequest, 100);
            }
            
            var lastRenderRequestId;
            function sendRenderRequest()
            {
               var elementIds = [];
               Object.getOwnPropertyNames(visibleElements).forEach(function(elementId)
               {
                  if (visibleElements[elementId]) elementIds.push(elementId);
               });
               
               if (potentialImageRequested)
               {
                  elementIds.push('#img');
                  potentialImageRequested = false;
               }
               
               if (elementIds.length)
               {
                  Object.getOwnPropertyNames(addedItems).forEach(function(itemKey)
                  { 
                     var item = items[itemKey];
                     if (item.type == 'text') removableTexts[itemKey] = true;
                  });
                  
                  var id = ++requestCounter;
                  // Todo: Add API method to choose language
                  var request = { id: id, mapId: mapId, language: 'fr', elementIds: elementIds, width: canvas.width, height: canvas.height };
                  var context = { scale: scale };
                  if (scale && xFocus && yFocus)
                  {
                     request.scale = scale;
                     request.xFocus = xFocus;
                     request.yFocus = yFocus;
                     context.ignoreServerScale = true;
                  }
                  setContext(id, context);
                  lastRenderRequestId = id;
                  socket.emit('renderReq', request);
               }
               else
               {
                  Object.getOwnPropertyNames(addedItems).forEach(function(itemKey)
                  {
                     removeItem(items[itemKey]);
                  });
               }
            }
               
            socket.on('renderRes', function(response)
            {
               var context = getContext(response, true);
               if (!context || response.requestId != lastRenderRequestId) return;
               
               var renderInfo = response.content;
               //console.log(renderInfo);      
               
               if (!context.ignoreServerScale)
               {
                  xFocus = renderInfo.xFocus;
                  yFocus = renderInfo.yFocus;
                  scale = renderInfo.scale; 
                  if (!context.scale) context.scale = scale;
               }
               
               var lookToItems = {};
               var itemsToRender_itemKey = {};
               var itemsToRender_itemId = {};
               var itemsToRender_keyAndLook = [];
               
               renderInfo.items.forEach(function(itemInfo)
               {
                  var itemKey = itemInfo[0].toString();
                  var lookId = itemInfo[1];
                  var resolution = itemInfo[2];
                  if (resolution || resolution === 0) itemKey += '_' + resolution;
                  
                  itemsToRender_itemKey[itemKey] = true;
                  itemsToRender_itemId[itemInfo[0]] = true;
         
                  if (!items[itemKey])
                  {                           
                     var id = ++requestCounter;
                     var request = { id: id, mapId: mapId, itemId: itemInfo[0], resolution: (resolution ? resolution : 0) };
                     setContext(id.toString(), { itemKey: itemKey, itemId: itemInfo[0], resolution: resolution, lookId: lookId });
                     socket.emit('itemDataReq', request);
                     
                     items[itemKey] =
                     {
                        key: itemKey,
                        itemId: itemInfo[0],
                        resolution: resolution
                     };
                  }

                  if (!looks[lookId])
                  {
                     if (!lookToItems[lookId]) lookToItems[lookId] = [];
                     lookToItems[lookId].push(itemKey);
                  } 
                  
                  if (!addedItems[itemKey] || items[itemKey].lookId != lookId)
                  {
                     itemsToRender_keyAndLook.push({ itemKey: itemKey, lookId: lookId });
                  }
               });
               
               Object.getOwnPropertyNames(lookToItems).forEach(function(lookId)
               {
                  var id = ++requestCounter;
                  var request = { id: id, mapId: mapId, lookId: lookId };
                  setContext(id, { lookId: lookId, itemKeyArray: lookToItems[lookId] });
                  socket.emit('lookReq', request);
               });
               
               Object.getOwnPropertyNames(addedItems).forEach(function(itemKey)
               {
                  var item = items[itemKey];
                  if (!itemsToRender_itemKey[itemKey])
                  {
                     var itemId = item.itemId;
                     if (itemsToRender_itemId[itemId])
                     {
                        itemsToRemove[itemKey] = true;
                     }
                     else
                     {      
                        removeItem(item);
                     }
                  }
               });
               
               itemsToRender_keyAndLook.forEach(function(ids)
               {
                  addItem(ids.itemKey, ids.lookId);
               });
            });
            
            socket.on('textRes', function(response)
            {
               var context = getContext(response, true);
               if (!context || response.requestId != lastRenderRequestId) return;
               
               var textInfo = response.content;
               //console.log(textInfo);
               if (!visibleElements[textInfo.e]) return;
               var itemKey = '_' + textInfo.e;
               
               items[itemKey] =
               {
                  key: itemKey,
                  type: 'text',
                  textLines: textInfo.t,
                  xMin: textInfo.x1,
                  xMax: textInfo.x2,
                  yMin: textInfo.y1,
                  yMax: textInfo.y2,
                  zI: textInfo.z,
                  size: textInfo.s,
                  //color: 'rgba(' + textInfo.r + ', ' + textInfo.g + ', ' + textInfo.b + ', ' + (textInfo.a / 255.0) + ')',
                  color: '#' + (256 + textInfo.r).toString(16).substr(1) + ((1 << 24) + (textInfo.g << 16) | (textInfo.b << 8) | textInfo.a).toString(16).substr(1),
                  scale: context.scale
               }
                          
               delete removableTexts[itemKey];
               Object.getOwnPropertyNames(removableTexts).forEach(function(itemKey2)
               {
                  var item2 = items[itemKey2];
                  if (textInfo.x1 <= item2.xMax && textInfo.x2 >= item2.xMin && textInfo.y1 <= item2.yMax && textInfo.y2 >= item2.yMin)
                  {
                     delete addedItems[itemKey2];
                     delete addedItemsByZIndex[item2.zI][itemKey2];
                     delete removableTexts[itemKey2];
                  }
               });
               
               addItem(itemKey);
               cancelAnimationFrame(reqAnimFrameId);
               reqAnimFrameId = requestAnimationFrame(renderCanvas);
            });
            
            socket.on('removeTextRes', function(response)
            {
               var context = getContext(response, true);
               if (!context || response.requestId != lastRenderRequestId) return;
               
               var itemKey = '_' + response.content.e;
               var item = items[itemKey];
               
               if (item)
               {
                  delete addedItems[itemKey];
                  delete addedItemsByZIndex[item.zI][itemKey];
                  delete removableTexts[itemKey];
               }
            });

            socket.on('itemDataRes', function(response)
            {
               var context = getContext(response);
               if (!context) return;
               
               var itemData = response.content;
               //console.log(itemData);

               var item = items[context.itemKey];
               
               if (item)
               {
                  item.type = itemData.type;
                  if (item.type == 'line' || item.type == 'polygon')
                  {
                     item.points = itemData.points;
                  }
                  else if (item.type == 'point')
                  {
                     item.x = itemData.x;
                     item.y = itemData.y;
                  }
                  
                  addItem(context.itemKey, context.lookId);
               }
            });

            socket.on('lookRes', function(response)
            {
               var context = getContext(response);
               if (!context) return;
               
               var lookData = response.content;
               
               looks[context.lookId] =
               {
                  zI: lookData.zI,
                  size: lookData.size,
                  //color: 'rgba(' + lookData.r + ', ' + lookData.g + ', ' + lookData.b + ', ' + (lookData.a / 255.0) + ')'
                  color: '#' + (256 + lookData.r).toString(16).substr(1) + ((1 << 24) + (lookData.g << 16) | (lookData.b << 8) | lookData.a).toString(16).substr(1)
               };
               
               context.itemKeyArray.forEach(function(itemKey)
               {
                  addItem(itemKey, context.lookId);
               });
            });
            
            function addItem(itemKey, lookId)
            {
               var item = items[itemKey];
               if (addedItems[itemKey] && item.lookId === lookId) return;
               
               if (item.type && item.lookId != lookId)
               {
                  if (item.lookId) delete addedItemsByZIndex[looks[item.lookId].zI][itemKey];
                  item.lookId = lookId;
               }
               
               var look;
               if (lookId) look = looks[lookId];
               else look = item; // Case of a text item: Item has no 'lookId', look parameters are in the 'item' object
                  
               if (item.type && look)
               {
                  // console.log('Add item ' + itemKey);
                  
                  Object.getOwnPropertyNames(itemsToRemove).forEach(function(itemKey2)
                  {
                     var item2 = items[itemKey2];
                     if (itemKey != itemKey2 && item.itemId === item2.itemId)
                     {
                        removeItem(item2);
                     }
                  });
                  
                  addedItems[itemKey] = true;
                  addedItemsByZIndex[look.zI][itemKey] = true;
                          
                  cancelAnimationFrame(reqAnimFrameId);
                  reqAnimFrameId = requestAnimationFrame(renderCanvas);
               }
            }
            
            function removeItem(item)
            {
               var itemKey = item.key;
               if (itemKey.charAt(0) == '_') return; // If the item is a text item, do nothing (text items are removed in another way)
               
               if (addedItems[itemKey])
               {
                  delete addedItems[itemKey];
                  delete itemsToRemove[itemKey];
                  delete addedItemsByZIndex[looks[item.lookId].zI][itemKey];
                  
                  cancelAnimationFrame(reqAnimFrameId);
                  reqAnimFrameId = requestAnimationFrame(renderCanvas);
               }
            }
            
            function renderCanvas()
            {
               ctx.clearRect(0, 0, canvas.width, canvas.height);
               var sizeFactor = mapInfo.sizeParameter1 / (mapInfo.sizeParameter2 + scale);
               
               var i;
               for (i = 31; i >= 0; --i)
               {
                  var itemList = addedItemsByZIndex[i];
                  Object.getOwnPropertyNames(itemList).forEach(function(itemKey)
                  {
                     var item = items[itemKey];
                     var look = looks[item.lookId];

                     if (item.type == 'line')
                     {
                        ctx.save();
                        ctx.translate(0.5 * canvas.width - xFocus * scale, 0.5 * canvas.height - yFocus * scale);
                        ctx.scale(scale, scale);
                        
                        ctx.beginPath();
                        item.points.forEach(function(p, i)
                        {
                           if (i === 0) ctx.moveTo(p.x, p.y);
                           else ctx.lineTo(p.x, p.y);
                        });

                        ctx.strokeStyle = look.color;
                        ctx.lineWidth = look.size * sizeFactor;
                        ctx.stroke();
                        
                        ctx.restore();
                     }
                     else if (item.type == 'polygon')
                     {
                        ctx.save();
                        ctx.translate(0.5 * canvas.width - xFocus * scale, 0.5 * canvas.height - yFocus * scale);
                        ctx.scale(scale, scale);
                        
                        ctx.beginPath();
                        item.points.forEach(function(p, i)
                        {
                           if (i === 0) ctx.moveTo(p.x, p.y);
                           else ctx.lineTo(p.x, p.y);
                        });

                        ctx.fillStyle = look.color;
                        ctx.fill();
                        
                        ctx.restore();
                     }
                     else if (item.type == 'point')
                     {
                        var x = (item.x - xFocus) * scale + 0.5 * canvas.width;
                        var y = (item.y - yFocus) * scale + 0.5 * canvas.height;
                        
                        ctx.beginPath();
                        ctx.arc(x, y, 0.5 * look.size * scale * sizeFactor, 0, 2 * Math.PI);

                        ctx.fillStyle = look.color;
                        ctx.fill();

                        ctx.strokeStyle = 'black';
                        ctx.lineWidth = scale * sizeFactor;
                        ctx.stroke();
                     }
                     else if (item.type = 'text')
                     {
                        if (item.scale == scale
                              && (item.xMin - xFocus) * scale + 0.5 * canvas.width >= 0
                              && (item.xMax - xFocus) * scale - 0.5 * canvas.width <= 0
                              && (item.yMin - yFocus) * scale + 0.5 * canvas.height >= 0
                              && (item.yMax - yFocus) * scale - 0.5 * canvas.height <= 0)
                        {
                           ctx.fillStyle = item.color;
                           ctx.font = item.size + 'px arial';
                           
                           item.textLines.forEach(function(line)
                           {
                              var x = (line[1] - xFocus) * scale + 0.5 * canvas.width;
                              var y = (line[2] - yFocus) * scale + 0.5 * canvas.height;
                              ctx.fillText(line[0], x, y);
                           });
                        }
                        else
                        {
                           // If the scale has changed or if the text overflows the map frame, the text item is obsolete, remove it
                           delete addedItems[itemKey];
                           delete addedItemsByZIndex[item.zI][itemKey];
                           delete removableTexts[itemKey];
                        }
                     }
                  });
               }
            }
            
            this.testSvg = function(svgId)
            {
               var sizeFactor = mapInfo.sizeParameter1 / (mapInfo.sizeParameter2 + scale);
               var html = "";
               
               var i;
               for (i = 31; i >= 0; --i)
               {
                  var itemList = addedItemsByZIndex[i];
                  Object.getOwnPropertyNames(itemList).forEach(function(itemKey)
                  {
                     var item = items[itemKey];
                     var look = looks[item.lookId];

                     if (item.type == 'line')
                     {
                        var color = look.color.substr(0, look.color.length - 2);                      
                        var opacity = parseInt(look.color.substr(-2, 2), 16) / 255;
                        html += '<path style="fill:none;stroke:' + color + ';stroke-opacity:' + opacity + ';stroke-width:'
                           + look.size * scale * sizeFactor + ';stroke-linecap:round;stroke-linejoin:round" d="';
                        
                        item.points.forEach(function(p, i)
                        {
                           var x = (p.x - xFocus) * scale + 0.5 * canvas.width;
                           var y = (p.y - yFocus) * scale + 0.5 * canvas.height;
                           
                           if (i === 0) html += 'M ';
                           else html += 'L ';
                           
                           html += x + ',' + y + ' ';
                        });
                        
                        html += '"></path>\n';
                     }
                     else if (item.type == 'polygon')
                     {
                        var color = look.color.substr(0, look.color.length - 2);                       
                        var opacity = parseInt(look.color.substr(-2, 2), 16) / 255;
                        html += '<path style="fill:' + color + ';fill-opacity:' + opacity + ';stroke:none" d="';
                        
                        item.points.forEach(function(p, i)
                        {
                           var x = (p.x - xFocus) * scale + 0.5 * canvas.width;
                           var y = (p.y - yFocus) * scale + 0.5 * canvas.height;
                           
                           if (i === 0) html += 'M ';
                           else html += 'L ';
                           
                           html += x + ',' + y + ' ';
                        });
                        
                        html += '"></path>\n';
                     }
                     else if (item.type == 'point')
                     {
                        var x = (item.x - xFocus) * scale + 0.5 * canvas.width;
                        var y = (item.y - yFocus) * scale + 0.5 * canvas.height;
                        var color = look.color.substr(0, look.color.length - 2);                      
                        var opacity = parseInt(look.color.substr(-2, 2), 16) / 255;
                        
                        html += '<circle cx="' + x + '" cy="' + y + '" r="' + 0.5 * look.size * scale * sizeFactor
                           + '" stroke="black" stroke-width="' + scale * sizeFactor + '" fill="' + color + '" fill-opacity="' + opacity + '"></circle>\n';
                     }
                     else if (item.type = 'text')
                     {        
                        var color = item.color.substr(0, item.color.length - 2);
                        var opacity = parseInt(item.color.substr(-2, 2), 16) / 255;
                        
                        item.textLines.forEach(function(line)
                        {
                           var x = (line[1] - xFocus) * scale + 0.5 * canvas.width;
                           var y = (line[2] - yFocus) * scale + 0.5 * canvas.height;
                           
                           html += '<text x="' + x + '" y="' + y + '" font-size="' + item.size + '" font-family="arial" fill="'
                              + color + '" fill-opacity="' + opacity + '">' + line[0] + '</text>\n'
                        });
                     }
                  });
               }
               
               $('#' + svgId).html(html);
            };
            
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

                  xFocus -= dx / scale;
                  yFocus -= dy / scale;

                  cancelAnimationFrame(reqAnimFrameId);
                  reqAnimFrameId = requestAnimationFrame(renderCanvas);
                  scheduleRenderRequest();

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
               var geoSize = Math.sqrt(canvas.width * canvas.width + canvas.height * canvas.height) / scale;
               var geoSize0 = geoSize;

               if (delta > 0) geoSize *= 1.15;
               else geoSize /= 1.15;

               if (geoSize < mapInfo.zoomMinDistance) geoSize = mapInfo.zoomMinDistance;
               else if (geoSize > mapInfo.zoomMaxDistance) geoSize = mapInfo.zoomMaxDistance;

               if (geoSize != geoSize0)
               {   
                  var zoomFactor = geoSize0 / geoSize;
                  if (delta > 0)
                  {
                     xFocus += (0.5 * canvas.width - e.clientX) * (1.0 - zoomFactor) / scale;
                     yFocus += (0.5 * canvas.height - e.clientY) * (1.0 - zoomFactor) / scale;
                  }
                  else
                  {
                     xFocus += (0.5 * canvas.width - e.clientX) * (1.0 / zoomFactor - 1.0) / scale;
                     yFocus += (0.5 * canvas.height - e.clientY) * (1.0 / zoomFactor - 1.0) / scale;
                  }
                  
                  scale *= zoomFactor;
                   
                  cancelAnimationFrame(reqAnimFrameId);
                  reqAnimFrameId = requestAnimationFrame(renderCanvas);
                  scheduleRenderRequest();
               }
            });
         }
      }
   }
}
