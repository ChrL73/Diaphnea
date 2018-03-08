import React from 'react';
import { Form, FormGroup, Modal, Button } from 'react-bootstrap';
import waitGif from './wait.gif'

export class SignUp extends React.Component
{
   constructor(props)
   {
      super(props);
      
      props.socket.on('displayPage', (data) => this.handleDisplayPage(data));
      props.socket.on('updateSignUp', (texts) => this.handleUpdateSiteLanguage(texts));
      props.socket.on('signUpError', (data) => this.handleServerError(data));
      
      this.stateReset =
      {
         pass1: '',
         pass2: '',
         showModal: false,
         pass1aMessageDisplay: 'none',
         pass1bMessageDisplay: 'none',
         pass2MessageDisplay: 'none',
         languageWaitDisplay: 'none',
         buttonWaitDisplay: 'none',
         texts: {}
      };
      
      this.state = this.stateReset;
   }
   
   render()
   {
      let siteLanguages;
      if (this.state.siteLanguageList)
      {
         siteLanguages = this.state.siteLanguageList.map((language) =>
         {
            return (<option key={language.id} value={language.id}>{language.name}</option>);
         });
      }
      
      return (
         <div style={{display: (this.state.page === 'signUp' ? 'block' : 'none')}} className="signUp">
            <div className="container">
               <div className="col-md-4 col-md-offset-4 col-sm-offset-3 col-sm-6">  
                  <header>
                     <div className="row">
                        <div className="col-sm-8 col-sm-offset-2">
                           <div className="navbar text-center">
                              <form className="navbar-form">
                                 <select className="form-control input-sm"  value={this.state.siteLanguageId}
                                         onChange={(e) => this.handleSiteLanguageChange(e.target.value)}>
                                    {siteLanguages}
                                 </select>
                              </form>
                           </div>
                        </div>
                        <div className="col-sm-1 col-xs-1">
                           <img src={waitGif} alt="Waiting for server..." className="waitImg" style={{display: this.state.languageWaitDisplay}}/>
                        </div>
                     </div>
                  </header>
                  <Modal show={this.state.showModal} onHide={() => this.setState({ showModal: false })}>               
                     <Modal.Body className="bg-danger">
                        <span>{this.state.texts.internalServerError}</span>
                        <Button className="close" onClick={() => this.setState({ showModal: false })}>x</Button>
                     </Modal.Body>
                  </Modal>
                  <Form>
                     <FormGroup id="nameGroup" validationState={(this.state.name1Message || this.state.name2Message) ? 'error' : null}>
                        <label htmlFor="signUpNameInput">{this.state.texts.name}:</label>
                        <input className="form-control" type="text" id="signUpNameInput" value={this.state.name ? this.state.name : ''}
                               onChange={(e) => this.setState({ name: e.target.value })}/>
                        <div className="text-danger" style={{display: (this.state.name1Message ? 'block' : 'none')}}>{this.state.texts.nameMustBeBetween2And16Chars}</div>
                        <div className="text-danger" style={{display: (this.state.name2Message ? 'block' : 'none')}}>{this.state.texts.theNameIsAlreadyUsed}</div>
                     </FormGroup>
                     <FormGroup id="pass1Group" validationState={(this.state.pass1aMessageDisplay !== 'none' || this.state.pass1bMessageDisplay !== 'none') ? 'error' : null}>
                        <label htmlFor="signUpPassInput1">{this.state.texts.password}:</label>
                        <input className="form-control" type="password" id="signUpPassInput1" value={this.state.pass1}
                               onChange={(e) => this.setState({ pass1: e.target.value })}/>
                        <div className="text-danger" style={{display: this.state.pass1aMessageDisplay}}>{this.state.texts.passwordMustContainAtLeast8Chars}</div>
                        <div className="text-danger" style={{display: this.state.pass1bMessageDisplay}}>{this.state.texts.passwordMustContainOnlyLettersNumbersEtc}</div>
                     </FormGroup>
                     <FormGroup id="pass2Group" validationState={(this.state.pass2MessageDisplay !== 'none') ? 'error' : null}>
                        <label htmlFor="signUpPassInput2">{this.state.texts.confirmPassword}:</label>
                        <input className="form-control" type="password" id="signUpPassInput2" value={this.state.pass2}
                               onChange={(e) => this.setState({ pass2: e.target.value })}/>
                        <div className="text-danger" style={{display: this.state.pass2MessageDisplay}}>{this.state.texts.twoPasswordsAreNotIdentical}</div>
                     </FormGroup>
                     <FormGroup>
                        <div className="row">
                           <div className="col-sm-10 col-xs-10">
                              <button className="btn btn-info" onClick={(e) => this.handleSignUpBtnClick(e)}>{this.state.texts.signUp}</button>
                              <span> </span>
                              <Button className="btn btn-warning" onClick={(e) => this.handleCancelBtnClick(e)}>{this.state.texts.cancel}</Button>
                           </div>
                           <div className="col-sm-1 col-xs-1">
                              <img src={waitGif} alt="Waiting for server..." className="waitImg" style={{display: this.state.buttonWaitDisplay}}/>
                           </div>
                        </div>
                     </FormGroup>
                  </Form>   
               </div>
            </div>
         </div>);
   }
   
   // 1- User action handlers
   
   handleSiteLanguageChange(value)
   {
      this.setState({ siteLanguageId: value, languageWaitDisplay: 'inline' });
      document.cookie = 'siteLanguageId=' + value + this.props.getCookieExpires(180);
      this.props.socket.emit('languageChoice', { page: 'signUp', languageId: value });
   }
   
   handleSignUpBtnClick(e)
   {
      e.preventDefault();
      this.props.socket.emit('submitSignUp', { name: this.state.name, pass1: this.state.pass1, pass2: this.state.pass2 });
      this.setState({ buttonWaitDisplay: 'inline' });
   }
   
   handleCancelBtnClick(e)
   {
      e.preventDefault();
      this.props.socket.emit('cancelSignUp', {});
   }
   
   // 2- Server message handlers
   
   handleDisplayPage(data)
   {
      let state = {};
      Object.getOwnPropertyNames(this.stateReset).forEach((property) => { state[property] = this.stateReset[property]; });
      
      if (data.page === 'signUp')
      {
         Object.getOwnPropertyNames(data).forEach((property) => { state[property] = data[property]; });
         window.onresize = undefined;
      }
      else
      {
         state.page = data.page;
      }
      
      this.setState(state);
   }
   
   handleUpdateSiteLanguage(texts)
   {
      this.setState({ languageWaitDisplay: 'none', texts: texts });
   }
   
   handleServerError(data)
   {
      this.setState(
      {
         buttonWaitDisplay: 'none',
         name1Message: data.name1Message,
         name2Message: data.name2Message,
         pass1aMessageDisplay: data.pass1aMessage ? 'block' : 'none',
         pass1bMessageDisplay: data.pass1bMessage ? 'block' : 'none',
         pass2MessageDisplay: data.pass2Message ? 'block' : 'none',
         showModal: Boolean(data.errorMessage)
      });
   }
}
