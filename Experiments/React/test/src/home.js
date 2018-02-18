import React from 'react';

export class Home extends React.Component
{
   render()
   {
      return (
         <div style={{display: (this.props.userInterfaceState.currentPage == 'index' ? 'block' : 'none')}} className="home">
            <div class="container">
               <button class="btn btn-info btn-sm" onClick={() => this.handleSignUpBtnClick()}>Sign up</button>
            </div>
         </div>);
   }
   
   handleSignUpBtnClick()
   {
      this.props.socket.emit('signUp', { name: 'test' });
   }
   
}
