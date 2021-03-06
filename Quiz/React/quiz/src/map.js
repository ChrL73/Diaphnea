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

import React from 'react';

export class Map extends React.Component
{
   constructor(props)
   {
      super(props); 
      this.maps = {};   
      this.state = { mapIds: [] };
   }
   
   render()
   {
      let canvas;
      
      if (this.state.mapIds.length)
      {
         canvas = this.state.mapIds.map((mapId) =>
         {
            return (
               <canvas style={{display: mapId === this.props.gameState.mapId ? 'block' : 'none' }} id={mapId} key={mapId}
                       height={this.props.gameState.mapHeight} width={this.props.gameState.mapWidth}></canvas>);
         });
      }
      else
      {
         canvas = (<canvas height={this.props.gameState.mapHeight} width={this.props.gameState.mapWidth}></canvas>);
      }
      
      return (
         <div className="map">
            {canvas}
         </div>);
   }
   
   init()
   {
      if (this.mapServerConnection)
      {
         this.loadMap();
      }
      else
      {
         window.mapServerInterface.createNewConnection(this.props.gameState.mapServerUrl,
                                                       (mapServerConnection) => { this.mapServerConnection = mapServerConnection; this.loadMap(); },
                                                       (error) => this.onError(error));
      }
   }
      
   onError(error)
   {
      console.error(error);
   }

   loadMap()
   {
      const mapId = this.props.gameState.mapId;
      
      if (this.maps[mapId])
      {
         this.map = this.maps[mapId].map;
         this.map.popState(-1);
         this.map.pushState(-1);
         this.mapElements = this.maps[mapId].mapElements;
         this.props.gameState.questionStates.forEach((s, i) => { this.map.pushState(i); });
         this.update(this.props.gameState.displayedQuestion);
      }
      else
      {
         let mapIds = this.state.mapIds;
         mapIds.push(mapId);
         this.map = undefined;
         this.mapElements = undefined;
         
         this.setState({ mapIds: mapIds }, () =>
         {
            this.mapServerConnection.loadMap(mapId, mapId, (map) =>
            {
               map.setFillingStyle(2);
               map.pushState(-1);
               const elementIds = map.getElementIds();
               
               map.loadElements(elementIds, (elementArray) =>
               {
                  this.maps[mapId] = { map: map };
                  this.map = map;
                  
                  let i, n = elementArray.length;
                  this.mapElements = {};
                  for (i = 0; i < n; ++i) this.mapElements[elementIds[i]] = elementArray[i];
                  this.maps[mapId].mapElements = this.mapElements;

                  this.props.gameState.questionStates.forEach((s, i) => { this.map.pushState(i); });
                  this.update(this.props.gameState.displayedQuestion);
               });
            });
         });
      }
   }
   
   redraw()
   {
      if (this.map) this.map.redraw();
   }
   
   update(newQuestion, oldQuestion)
   {
      if (!this.map) return;

      if (oldQuestion || oldQuestion === 0) this.map.pushState(oldQuestion);
      this.map.popState(newQuestion);

      if (!this.props.gameState.mapInfo[newQuestion]) return;

      const info = this.props.gameState.mapInfo[newQuestion];
      this.map.setFramingLevel(info.framingLevel);   

      let elementsToShow = {};
      info.mapIds.forEach((idInfo) =>
      {
         this.showLinkedElements(idInfo, elementsToShow);
      });

      Object.getOwnPropertyNames(elementsToShow).forEach((elementId) =>
      {
         const keepForFraming = elementsToShow[elementId];
         this.mapElements[elementId].show(!keepForFraming);
      });
   }
   
   showLinkedElements(idInfo, elementsToShow)
   {
      const element = this.mapElements[idInfo.id];
      const threshold = 50;
      let linkedElements = {};
      linkedElements[element.getId()] = true;

      let i = 0;
      while (i < idInfo.depth)
      {
         let elementsToAdd = [];

         Object.getOwnPropertyNames(linkedElements).forEach((elementId) =>
         { 
            const linkedElements1 = this.mapElements[elementId].getLinkedElements1();
            if (i < 2 || linkedElements1.length < threshold) Array.prototype.push.apply(elementsToAdd, linkedElements1);
         });        
         ++i; 

         if (i < idInfo.depth)
         {
            Object.getOwnPropertyNames(linkedElements).forEach((elementId) =>
            { 
               const linkedElements2 = this.mapElements[elementId].getLinkedElements2();
               if (i < 2 || linkedElements2.length < threshold) Array.prototype.push.apply(elementsToAdd, linkedElements2);
            });
            ++i;
         }

         elementsToAdd.forEach(function(elementId)
         {
            linkedElements[elementId] = true;
         });
      }

      let categories = {};
      idInfo.categories.forEach(function(index) { categories[index] = 1; });
      const include = (idInfo.mode === 'INCLUDE');

      Object.getOwnPropertyNames(linkedElements).forEach((elementId) =>
      { 
         const elt = this.mapElements[elementId];
         const categoryInList = (categories[elt.getCategoryIndex()] === 1);

         if (elt === element || categoryInList === include)
         {
            let keepForFraming = false;
            if (idInfo.framing === 1) keepForFraming = (elt === element);
            else if (idInfo.framing === 2) keepForFraming = true;
            if (keepForFraming) elementsToShow[elementId] = true;
            else if (!elementsToShow[elementId]) elementsToShow[elementId] = false;
         }
      });
   }
}
