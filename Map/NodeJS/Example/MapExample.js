$(function()
{
   var url = 'http://192.168.50.52:3001';
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
         
         var noya, estuaire, woleuNtem;
         
         map.loadElement('Noya', onElementLoaded);
         
         function onElementLoaded(element)
         {
            noya = element;
            console.log(noya.getName('fr'));
            
            //map.loadElements(['Estuaire', 'WoleuNtem'], onElementsLoaded);
         }
         
         /*function onElementsLoaded(elements)
         {
            estuaire = elements[0];
            woleuNtem = elements[1];
            
            noya.show();
            estuaire.show();
            woleuNtem.show();
            noya.hide();
            map.render();
         }*/
      }
   }
});
