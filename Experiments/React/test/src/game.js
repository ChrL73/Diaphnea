import React from 'react';
import { Button } from 'react-bootstrap';

export class Game extends React.Component
{
   render()
   {
      const data = this.props.userInterfaceState.data;
      
      return (
         <div style={{display: (data.page === 'game' ? 'block' : 'none')}} className="game">
            <div className="container-fluid">
               <Button className="btn btn-warning" onClick={(e) => this.handleStopBtnClick(e)}>{data.texts.stop}</Button>
            </div>
         </div>);
   }
   
   handleStopBtnClick(e)
   {
      e.preventDefault();
      this.props.socket.emit('stopGame', {});
   }
}
