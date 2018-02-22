import React from 'react';

export class SignUp extends React.Component
{
   render()
   {
      const data = this.props.userInterfaceState.data;
      
      return (
         <div style={{display: (data.page === 'signUp' ? 'block' : 'none')}} className="signUp">
            <div className="container">
               <button className="btn btn-warning" onClick={() => this.handleCancelBtnClick()}>{data.texts.cancel}</button>
            </div>
         </div>);
   }
   
   handleCancelBtnClick()
   {
      this.props.socket.emit('cancelSignUp', {});
   }
}
