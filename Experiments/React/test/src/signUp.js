import React from 'react';
import { Button } from 'react-bootstrap';

export class SignUp extends React.Component
{
   render()
   {
      const data = this.props.userInterfaceState.data;
      
      return (
         <div style={{display: (data.page === 'signUp' ? 'block' : 'none')}} className="signUp">
            <div className="container">
               <Button className="btn btn-warning" onClick={(e) => this.handleCancelBtnClick(e)}>{data.texts.cancel}</Button>
            </div>
         </div>);
   }
   
   handleCancelBtnClick(e)
   {
      e.preventDefault();
      this.props.socket.emit('cancelSignUp', {});
   }
}
