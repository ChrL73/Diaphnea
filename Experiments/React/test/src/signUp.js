import React from 'react';

export class SignUp extends React.Component
{
   render()
   {
      return (
         <div style={{display: (this.props.userInterfaceState.currentPage == 'signUp' ? 'block' : 'none')}} className="signUp">
            <div class="container">
               Sign up
               <button class="btn btn-warning" onClick={() => this.props.socket.emit('cancelSignUp', {})}>Cancel</button>
            </div>
         </div>);
   }
}
