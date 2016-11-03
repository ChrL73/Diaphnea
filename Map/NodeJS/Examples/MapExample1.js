$(function()
{
   var url = 'http://192.168.50.91:3001';
   mapServerInterface.createNewConnection(url, onConnected);
   
   function onConnected(mapServerConnection)
   {
      console.log(mapServerConnection.getMapIds());
      
      var canvasId = 'canvas';
      var mapId = '_Gabon';   
      mapServerConnection.loadMap(mapId, canvasId, onMapLoaded);
      
      function onMapLoaded(map)
      {
         console.log(map.getLanguages());
         console.log(map.getName('fr'));
         console.log(map.getElementIds());
         
         var libreville, estuaire, woleuNtem;
         
         map.loadElement('Libreville', onElementLoaded);
         
         function onElementLoaded(element)
         {
            libreville = element;
            console.log(libreville.getName('fr'));
            
            map.loadElements(['Estuaire', 'WoleuNtem', 'KomoOcean'], onElementsLoaded);
         }
         
         function onElementsLoaded(elements)
         {
            estuaire = elements[0];
            woleuNtem = elements[1];
            komoOcean = elements[2];
             
            console.log(estuaire.getName('fr'));
            console.log(woleuNtem.getName('fr'));
            
            libreville.show();
            estuaire.show();
            woleuNtem.show();
            komoOcean.show();
            woleuNtem.hide();
            map.render();
            //setTimeout(function() { map.render(); }, 1000);
         }
      }
   }
});
