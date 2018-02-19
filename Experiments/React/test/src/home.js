import React from 'react';
import waitGif from './wait.gif'

export class Home extends React.Component
{
   render()
   {
      let data = this.props.userInterfaceState.data;
      
      return (
         <div style={{display: (data.page === 'index' ? 'block' : 'none')}} className="home">
            <div className="container">
               <header>
                  <div className="navbar">
                     <div className="col-md-10">     
                        {this.renderNavbarForm(data)}     
                     </div>
                     <div className="col-md-2 text-center">
                     
                        
                     </div>
                  </div>
               </header>
            </div>
         </div>);
   }
   
   renderNavbarForm(data)
   {
      if (data.userName)
      {
         return (
            <form className="navbar-form">
               <label>{data.userName}</label>
               <span> </span>
               <button className="btn btn-primary btn-sm" onClick={(e) => this.handleSignOutBtnClick(e)}>{data.texts.signOut}</button>
               <div className="col-sm-1 navbar-right"><img src={waitGif} id="indexNavBarWait" className="waitImg"/></div>
            </form>);
      }
      else
      {
         return (
            <form className="navbar-form">
               <small>
                  <label htmlFor="indexNameInput" id="indexNameLabel">{data.texts.name}:</label>
                  <span> </span>
                  <input className="form-control input-sm" type="text" id="indexNameInput" value={data.tmpName ? data.tmpName : ''} onChange={(e) => this.handleDataChange(data, 'tmpName', e.target.value)}/>
                  <span> </span>
                  <label htmlFor="indexPassInput" id="indexPassLabel">{data.texts.password}:</label>
                  <span> </span>
                  <input className="form-control input-sm" type="password" id="indexPassInput"/>
                  <span> </span>
                  <div className="visible-xs"><br/></div>
                  <button className="btn btn-primary btn-sm" onClick={(e) => this.handleSignInBtnClick(e)}>{data.texts.signIn}</button>
                  <span> </span>
                  <button className="btn btn-info btn-sm" onClick={(e) => this.handleSignUpBtnClick(e)}>{data.texts.signUp}</button>
                  <div className="col-sm-1 navbar-right"><img src={waitGif} id="indexNavBarWait" className="waitImg"/></div>
               </small>
            </form>);
      }
   }
   
   handleDataChange(data, field, value)
   {
      data[field] = value;
      this.props.changeData(data);
   }

   handleSignInBtnClick(e)
   {
      e.preventDefault();
      this.props.socket.emit('signIn', { name: this.props.userInterfaceState.data.tmpName, pass: document.getElementById('indexPassInput').value });
   }

   handleSignOutBtnClick(e)
   {
      e.preventDefault();
      this.props.socket.emit('signOut', {});
   }
   
   handleSignUpBtnClick(e)
   {
      e.preventDefault();
      this.props.socket.emit('signUp', { name: this.props.userInterfaceState.data.tmpName });
   }

   //document.getElementById('indexPassInput').value


   
}
