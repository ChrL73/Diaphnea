import React from 'react';
import waitGif from './wait.gif'
//import { Modal } from 'react-bootstrap';

let homeDebugCounter = 0;

export class Home extends React.Component
{
   constructor(props)
   {
      super(props);
      
      ++homeDebugCounter;
      if (homeDebugCounter !== 1) throw(String('Error: Home constructor should be called only once'));
      
      props.socket.on('updateIndex', (texts) => this.handleUpdateSiteLanguage(texts));
      props.socket.on('unknownName', () => this.handleUnknownName());
   }
   
   render()
   {
      const data = this.props.userInterfaceState.data;
      
      let siteLanguages;
      if (data.siteLanguageList)
      {
         siteLanguages = data.siteLanguageList.map((language) =>
         {
            return (<option key={language.id} value={language.id}>{language.name}</option>);
         });
      }
      
      return (
         <div style={{display: (data.page === 'index' ? 'block' : 'none')}} className="home">
            <div className="container">
               <header>
                  <div className="navbar">
                     <div className="col-md-10">     
                        {this.renderNavbarForm(data)}     
                     </div>
                     <div className="col-md-2 text-center">
                        <div className="visible-xs visible-sm"><br/></div>
                        <form className="navbar-form">
                           <select className="form-control input-sm" value={data.siteLanguageId}
                                   onChange={(e) => this.handleSiteLanguageChange(data, 'siteLanguageId', e.target.value)}>
                              {siteLanguages}
                           </select>
                        </form>
                     </div>
                  </div>
               </header>
               <div className="modal fade" id="indexErrorMessage1">
                  <div className="modal-dialog">
                     <div className="modal-content">
                        <div className="modal-body bg-danger">
                           <span>data.texts.unknownUserOrWrongPassword</span>
                           <button type="button" className="close" data-dismiss="modal">x</button>
                        </div>
                     </div>
                  </div>
               </div>
            </div>
         </div>);
   }
   
   /* <div class="modal fade" id="indexErrorMessage2"><div class="modal-dialog"><div class="modal-content"><div class="modal-body bg-danger"><span id="indexErrorSpan2">'
         + pageData.texts.internalServerError
         + '</span><button type="button" class="close" data-dismiss="modal">x</button></div></div></div></div>*/
   
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
   
   // 1- Handlers for user actions
   
   handleSiteLanguageChange(data, field, value)
   {
      this.handleDataChange(data, field, value);
      if (!data.userName) document.cookie = 'siteLanguageId=' + value + this.props.getCookieExpires(180);
      this.props.socket.emit('languageChoice', { page: 'index', languageId: value });
      document.getElementById('indexNavBarWait').style.display = 'inline';
   }

   handleSignInBtnClick(e)
   {
      e.preventDefault();
      this.props.socket.emit('signIn', { name: this.props.userInterfaceState.data.tmpName, pass: document.getElementById('indexPassInput').value });
      document.getElementById('indexNavBarWait').style.display = 'inline';
   }

   handleSignOutBtnClick(e)
   {
      e.preventDefault();
      this.props.socket.emit('signOut', {});
      document.getElementById('indexNavBarWait').style.display = 'inline';
   }
   
   handleSignUpBtnClick(e)
   {
      e.preventDefault();
      this.props.socket.emit('signUp', { name: this.props.userInterfaceState.data.tmpName });
      document.getElementById('indexNavBarWait').style.display = 'inline';
   }
   
   // 2- Handlers for server messages
   
   handleUpdateSiteLanguage(texts)
   {
      this.handleDataChange(this.props.userInterfaceState.data, 'texts', texts);
      
      // Todo: remove the following line and emit 'levelChoice' instead
      document.getElementById('indexNavBarWait').style.display = 'none';
   }
   
   handleUnknownName()
   {
      document.getElementById('indexNavBarWait').style.display = 'none';
      
      //document.getElementById('indexErrorMessage1').modal();
      
   }
}
