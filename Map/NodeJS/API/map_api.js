/* --------------------------------------------------------------------
 *
 * Copyright (C) 2018
 *
 * This file is part of the Diaphnea project.
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Affero General Public License
 * version 3 as published by the Free Software Foundation
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 *
 * -------------------------------------------------------------------- */

var mapServerInterface =
{
   createNewConnection: function(url, onConnected, onError)
   {
      var connection = new Connection(function()
      {
         onConnected(connection);
      },
      onError);

      function Connection(onConnected, onError)
      {   
         var mapIds;
         this.getMapIds = function() { return mapIds; }
         
         var serverVersion = 'unknown';
         this.getServerVersion = function() { return serverVersion; }
         
         var usedMapIds = {};
         var usedCanvas = {};
         
         var socket;
         var requestCounter = -1;
         var popCounter = 0;
            
         var contexts = {};
         function setContext(id, context)
         {
            contexts[id.toString()] = context;
            setTimeout(function() { delete contexts[id.toString()]; }, 50000);
         }
         
         function getContext(response, dontDelete, mapId)
         {
            var requestId = response.requestId.toString();
            var context = contexts[requestId];
      
            if (context)
            {
               if (mapId && context.mapId != mapId) return;
               if(!dontDelete) delete contexts[requestId];
            }
            
            return context;
         }

         var _url;
         $.getScript(url + '/socket.io/socket.io.js').done(function()
         {         
            _url = url;
            socket = io(url);
            
            socket.on('version', function(version)
            {
               serverVersion = version;
            });
            
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
            
            socket.on('errorRes', function(response)
            {
               var context = getContext(response);
               onError({ error: response.content.error, message: response.content.message, requestId: response.requestId, context: context });
            });
         })
         .fail(function()
         {
            onError({ error: -1, message: 'Failed to load ' + url + '/socket.io/socket.io.js' });
         });
         
         function postHttpRequest(route, data)
         {
            var form = document.createElement("form");
            form.action = _url + '/' + route;
            form.method = 'post';
            form.target = '_blank';

            var input = document.createElement('textarea');
            //var input = document.createElement('input');
            //input.setAttribute('type', 'text');
            input.name = 'data';
            input.value = JSON.stringify(data);
            form.appendChild(input);           

            form.style.display = 'none';
            document.body.appendChild(form);
            form.submit();
         }
         
         this.loadMap = function(mapId, canvasId, onMapLoaded)
         {
            if (usedMapIds[mapId])
            {
               onError({ error: -1, message: "Can't load 2 instances of the same map ('" + mapId + "')" });
               return;
            }
            
            if (usedCanvas[canvasId])
            {
               onError({ error: -1, message: "Can't load 2 maps on the same canvas ('" + canvasId + "')" });
               return;
            }
            
            usedMapIds[mapId] = true;
            usedCanvas[canvasId] = true;
            
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
            var framingExceptions = {};
            var items = {};
            var addedItems = {};
            var itemsToRemove = {};
            var removableTexts = {};
            var xFocus, yFocus, scale;
            
            var looks = {};
            var lookIds = [];
            var visibleLookIds = {};
            mapInfo.looks.forEach(function(look)
            {
               looks[look.id] =
               {
                  zI: look.zI,
                  size: look.size,
                  defaultR: look.r,
                  defaultG: look.g,
                  defaultB: look.b,
                  defaultA: look.a,
                  customR: look.r,
                  customG: look.g,
                  customB: look.b,
                  customA: look.a,
                  r: look.r,
                  g: look.g,
                  b: look.b,
                  a: look.a,
                  canvasColor: 'rgba(' + look.r + ', ' + look.g + ', ' + look.b + ', ' + (look.a / 255.0) + ')',
                  name: look.name
               };
               
               lookIds.push(look.id);
            });
            
            this.getColorIds = function() { return lookIds; };
            this.getColorInfo = function(colorId, languageId)
            {
               var look = looks[colorId];
               if (look)
               { 
                  var info =
                  {
                     defaultR: look.defaultR,
                     defaultG: look.defaultG,
                     defaultB: look.defaultB,
                     defaultA: look.defaultA,
                     customR: look.customR,
                     customG: look.customG,
                     customB: look.customB,
                     customA: look.customA,
                     r: look.r,
                     g: look.g,
                     b: look.b,
                     a: look.a,
                     name: look.name[languageId]
                  };
                  return info;
               }
            };
            
            this.isColorVisible = function(colorId) { return visibleLookIds[colorId]; };
            
            this.setCustomColor = function(colorId, r, g, b, a)
            {
               var look = looks[colorId];
               if (look)
               {
                  if ((r || r === 0) && (g || g === 0) && (b || b === 0) && (a || a === 0))
                  {
                     look.customR = r;
                     look.customG = g;
                     look.customB = b;
                     look.customA = a;
                  }
                  
                  look.r = look.customR;
                  look.g = look.customG;
                  look.b = look.customB;
                  look.a = look.customA;
                  look.canvasColor = 'rgba(' + look.r + ', ' + look.g + ', ' + look.b + ', ' + (look.a / 255.0) + ')';
               }
            };
            
            this.restoreDefaultColor = function(colorId)
            {
               var look = looks[colorId];
               if (look)
               {
                  look.r = look.defaultR;
                  look.g = look.defaultG;
                  look.b = look.defaultB;
                  look.a = look.defaultA;
                  look.canvasColor = 'rgba(' + look.r + ', ' + look.g + ', ' + look.b + ', ' + (look.a / 255.0) + ')';
               }
            };
            
            var currentLanguageId = mapInfo.languages[0].id;
            this.setLanguage = function(languageId)
            {
               currentLanguageId = languageId;
            };
            
            var currentFillingStyle = 0;
            this.setFillingStyle = function(fillingStyle)
            {
               currentFillingStyle = fillingStyle;
            };
            
            var currentFramingLevel = 0;
            this.setFramingLevel = function(framingLevel)
            {
               currentFramingLevel = framingLevel;
            };
            
            var addedItemsByZIndex = [];
            var i;
            for (i = 0; i < 32; ++i) addedItemsByZIndex.push({});
            
            this.getLanguages = function() { return mapInfo.languages; };
            this.getName = function(languageId) { return mapInfo.names[languageId]; };
            this.getElementIds = function() { return mapInfo.elementIds; };
            this.getCategories = function() { return mapInfo.categories; };
            
            var savedStates = {};
            this.pushState = function(stateId)
            {
               savedStates[stateId] =
               {
                  xFocus: xFocus,
                  yFocus: yFocus,
                  scale: scale,
                  currentLanguageId: currentLanguageId,
                  currentFillingStyle: currentFillingStyle,
                  currentFramingLevel: currentFramingLevel
               };
               
               savedStates[stateId].visibleElements = JSON.parse(JSON.stringify(visibleElements));
               savedStates[stateId].framingExceptions = JSON.parse(JSON.stringify(framingExceptions));
               savedStates[stateId].addedItems = JSON.parse(JSON.stringify(addedItems));
               savedStates[stateId].itemsToRemove = JSON.parse(JSON.stringify(itemsToRemove));
               savedStates[stateId].removableTexts = JSON.parse(JSON.stringify(removableTexts));
               savedStates[stateId].addedItemsByZIndex = JSON.parse(JSON.stringify(addedItemsByZIndex));
            };
            
            this.popState = function(stateId)
            {
               var state = savedStates[stateId];
               
               if (state)
               {
                  ++popCounter;
                  
                  xFocus = state.xFocus;
                  yFocus = state.yFocus;
                  scale = state.scale;
                  currentLanguageId = state.currentLanguageId;
                  currentFillingStyle = state.currentFillingStyle;
                  currentFramingLevel = state.currentFramingLevel;
                  
                  visibleElements = JSON.parse(JSON.stringify(state.visibleElements));
                  framingExceptions = JSON.parse(JSON.stringify(state.framingExceptions));
                  addedItems = JSON.parse(JSON.stringify(state.addedItems));
                  itemsToRemove = JSON.parse(JSON.stringify(state.itemsToRemove));
                  removableTexts = JSON.parse(JSON.stringify(state.removableTexts));
                  addedItemsByZIndex = JSON.parse(JSON.stringify(state.addedItemsByZIndex));
                  
                  delete savedStates[stateId];
                  cancelAnimationFrame(reqAnimFrameId);
                  reqAnimFrameId = requestAnimationFrame(renderCanvas);
                  scheduleRenderRequest();
               }
            };
            
            this.loadElement = function(elementId, onElementLoaded)
            {
               var id = ++requestCounter;
               var request = { id: id, mapId: mapId, elementId: elementId };
               setContext(id, { mapId: mapId, elementId: elementId, onElementLoaded: onElementLoaded });
               socket.emit('elementInfoReq', request);
            }
            
            socket.on('elementInfoRes', function(response)
            {
               var context = getContext(response, false, mapId);
               if (context) context.onElementLoaded(new Element(context.elementId, response.content));
            });
            
            this.loadElements = function(elementIds, onElementsLoaded)
            {
               var id = ++requestCounter;
               var request = { id: id, mapId: mapId, elementIds: elementIds };
               setContext(id, { mapId: mapId, elementIds: elementIds, onElementsLoaded: onElementsLoaded });
               socket.emit('elementsInfoReq', request);
            }
            
            socket.on('elementsInfoRes', function(response)
            {
               var context = getContext(response, false, mapId);
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
               this.getCategoryIndex = function() { return elementInfo.category; }
               this.getLinkedElements1 = function() { return elementInfo.linkedElements1; }
               this.getLinkedElements2 = function() { return elementInfo.linkedElements2; }
               
               this.show = function(exceptFromFraming)
               {
                  if (exceptFromFraming) framingExceptions[elementId] = true;
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
                     delete addedItemsByZIndex[looks[textItem.lookId].zI][textItemKey];
                     delete removableTexts[textItemKey];
                  }
                  scheduleRenderRequest();
               }
            }
            
            var svgRequested = false;
            this.requestSvg = function()
            {
               svgRequested = true;
               scheduleRenderRequest();
            };
                     
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
            var popCounter0;
            function scheduleRenderRequest()
            {
               clearTimeout(renderRequestTimeout);
               popCounter0 = popCounter;
               renderRequestTimeout = setTimeout(sendRenderRequest, 100);
            }
            
            var lastRenderRequestId;
            function sendRenderRequest()
            {
               var elementIds = [];
               var scaleExists = scale && xFocus && yFocus;
               Object.getOwnPropertyNames(visibleElements).forEach(function(elementId)
               {
                  if (visibleElements[elementId])
                  {
                     elementIds.push((!scaleExists && framingExceptions[elementId]) ? ('-' + elementId) : elementId);
                  }
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
                  var request = { id: id, mapId: mapId, language: currentLanguageId, elementIds: elementIds, width: canvas.width, height: canvas.height, lookIndex: currentFillingStyle };
                  
                  var context;
                  if (!svgRequested) context = { scale: scale };
                  
                  if (scaleExists)
                  {
                     request.scale = scale;
                     request.xFocus = xFocus;
                     request.yFocus = yFocus;
                     if (!svgRequested) context.ignoreServerScale = true;
                  }
                  else
                  {
                     request.framingLevel = currentFramingLevel;
                  }
                  
                  if (svgRequested)
                  {
                     var customColors = [];
                     Object.getOwnPropertyNames(looks).forEach(function(lookId)
                     { 
                        var look = looks[lookId];
                        if (look.r != look.defaultR || look.g != look.defaultG || look.b != look.defaultB || look.a != look.defaultA)
                        {
                           var customColor = 
                           {
                              i: lookId,
                              c: '#' + (0x1000000 | look.b | (look.g << 8) | (look.r << 16)).toString(16).substr(1),
                              o: (look.a / 255.0).toFixed(3)
                           }
                           
                           customColors.push(customColor);
                        }
                     });
                     
                     if (customColors.length != 0) request.customColors = customColors;
                     
                     postHttpRequest('map.svg', request);
                  }
                  else
                  {
                     context.mapId = mapId;
                     context.popCounter = popCounter0;
                     setContext(id, context);
                     lastRenderRequestId = id;
                     socket.emit('renderReq', request);
                  }
               }
               else
               {
                  Object.getOwnPropertyNames(addedItems).forEach(function(itemKey)
                  {
                     removeItem(items[itemKey]);
                  });
               }
               
               svgRequested = false;
            }
               
            socket.on('renderRes', function(response)
            {
               var context = getContext(response, true, mapId);
               if (!context || response.requestId != lastRenderRequestId || context.popCounter != popCounter) return;
               
               var renderInfo = response.content;
               
               if (!context.ignoreServerScale)
               {
                  xFocus = renderInfo.xFocus;
                  yFocus = renderInfo.yFocus;
                  scale = renderInfo.scale; 
                  if (!context.scale) context.scale = scale;
               }
               
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
                     setContext(id.toString(), { mapId: mapId, itemKey: itemKey, itemId: itemInfo[0], resolution: resolution, lookId: lookId, popCounter: popCounter });
                     socket.emit('itemDataReq', request);
                     
                     items[itemKey] =
                     {
                        key: itemKey,
                        itemId: itemInfo[0],
                        resolution: resolution
                     };
                  }
                  
                  if (!addedItems[itemKey] || items[itemKey].lookId != lookId)
                  {
                     itemsToRender_keyAndLook.push({ itemKey: itemKey, lookId: lookId });
                  }
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
               var context = getContext(response, true, mapId);
               if (!context || response.requestId != lastRenderRequestId) return;
               
               var textInfo = response.content;
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
                  lookId: textInfo.look,
                  size: textInfo.s,
                  scale: context.scale
               }
                          
               delete removableTexts[itemKey];
               Object.getOwnPropertyNames(removableTexts).forEach(function(itemKey2)
               {
                  var item2 = items[itemKey2];
                  if (textInfo.x1 <= item2.xMax && textInfo.x2 >= item2.xMin && textInfo.y1 <= item2.yMax && textInfo.y2 >= item2.yMin)
                  {
                     delete addedItems[itemKey2];
                     delete addedItemsByZIndex[looks[item2.lookId].zI][itemKey2];
                     delete removableTexts[itemKey2];
                  }
               });
               
               addItem(itemKey, textInfo.look);
               cancelAnimationFrame(reqAnimFrameId);
               reqAnimFrameId = requestAnimationFrame(renderCanvas);
            });
            
            socket.on('removeTextRes', function(response)
            {
               var context = getContext(response, true, mapId);
               if (!context || response.requestId != lastRenderRequestId) return;
               
               var itemKey = '_' + response.content.e;
               var item = items[itemKey];
               
               if (item)
               {
                  delete addedItems[itemKey];
                  delete addedItemsByZIndex[looks[item.lookId].zI][itemKey];
                  delete removableTexts[itemKey];
               }
            });

            socket.on('itemDataRes', function(response)
            {
               var context = getContext(response, false, mapId);
               if (!context) return;
               
               var itemData = response.content;

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
                  
                  if (!itemsToRemove[context.itemKey] && popCounter == context.popCounter) addItem(context.itemKey, context.lookId);
               }
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
               
               var look = looks[lookId];
               if (item.type && look)
               {
                  Object.getOwnPropertyNames(itemsToRemove).forEach(function(itemKey2)
                  {
                     var item2 = items[itemKey2];
                     if (itemKey != itemKey2 && item.itemId === item2.itemId)
                     {
                        removeItem(item2);
                     }
                  });
                  
                  addedItems[itemKey] = true;
                  addedItemsByZIndex[looks[item.lookId].zI][itemKey] = true;
                          
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
               
               Object.getOwnPropertyNames(visibleLookIds).forEach(function(id) { visibleLookIds[id] = false });
               
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
                        visibleLookIds[item.lookId] = true;
                        
                        ctx.save();
                        ctx.translate(0.5 * canvas.width - xFocus * scale, 0.5 * canvas.height - yFocus * scale);
                        ctx.scale(scale, scale);
                        
                        ctx.beginPath();
                        item.points.forEach(function(p, i)
                        {
                           if (i === 0) ctx.moveTo(p.x, p.y);
                           else ctx.lineTo(p.x, p.y);
                        });

                        ctx.strokeStyle = look.canvasColor;
                        ctx.lineWidth = look.size * sizeFactor;
                        ctx.stroke();
                        
                        ctx.restore();
                     }
                     else if (item.type == 'polygon')
                     {
                        visibleLookIds[item.lookId] = true;
                        
                        ctx.save();
                        ctx.translate(0.5 * canvas.width - xFocus * scale, 0.5 * canvas.height - yFocus * scale);
                        ctx.scale(scale, scale);
                        
                        ctx.beginPath();
                        item.points.forEach(function(p, i)
                        {
                           if (i === 0) ctx.moveTo(p.x, p.y);
                           else ctx.lineTo(p.x, p.y);
                        });

                        ctx.fillStyle = look.canvasColor;
                        
                        // Canvas fill rule 'evenodd' is necessary to see the openings inside the polygon 
                        ctx.fill('evenodd');
                        
                        ctx.restore();
                     }
                     else if (item.type == 'point')
                     {
                        visibleLookIds[item.lookId] = true;
                        
                        var x = (item.x - xFocus) * scale + 0.5 * canvas.width;
                        var y = (item.y - yFocus) * scale + 0.5 * canvas.height;
                        
                        ctx.beginPath();
                        ctx.arc(x, y, 0.5 * look.size * scale * sizeFactor, 0, 2 * Math.PI);

                        ctx.fillStyle = look.canvasColor;
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
                           visibleLookIds[item.lookId] = true;
                           
                           ctx.fillStyle = look.canvasColor;
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
                           delete addedItemsByZIndex[looks[item.lookId].zI][itemKey];
                           delete removableTexts[itemKey];
                        }
                     }
                  });
               }
            }
            
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
                  var rect = canvas.getBoundingClientRect();
                  var x = e.clientX - rect.left;
                  var y = e.clientY - rect.top;
                  
                  var zoomFactor = geoSize0 / geoSize;
                  if (delta > 0)
                  {
                     xFocus += (0.5 * canvas.width - x) * (1.0 - zoomFactor) / scale;
                     yFocus += (0.5 * canvas.height - y) * (1.0 - zoomFactor) / scale;
                  }
                  else
                  {
                     xFocus += (0.5 * canvas.width - x) * (1.0 / zoomFactor - 1.0) / scale;
                     yFocus += (0.5 * canvas.height - y) * (1.0 / zoomFactor - 1.0) / scale;
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
