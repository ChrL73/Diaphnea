import React from 'react';
import { Form, FormGroup, Modal, Button } from 'react-bootstrap';
import waitGif from './wait.gif'

export class SignUp extends React.Component
{
   constructor(props)
   {
      super(props);
      
      props.socket.on('updateSignUp', (texts) => this.handleUpdateSiteLanguage(texts));
      props.socket.on('signUpError', (errorData) => this.handleServerError(errorData));
      props.socket.on('displayPage', () => this.setState(this.initialState));
      
      this.initialState =
      {
         pass1: '',
         pass2: '',
         showModal: false,
         pass1aMessageDisplay: 'none',
         pass1bMessageDisplay: 'none',
         pass2MessageDisplay: 'none',
         languageWaitDisplay: 'none',
         buttonWaitDisplay: 'none',
      };
      
      this.state = this.initialState;
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
         <div style={{display: (data.page === 'signUp' ? 'block' : 'none')}} className="signUp">
            <div className="container">
               <div className="col-md-4 col-md-offset-4 col-sm-offset-3 col-sm-6">  
                  <header>
                     <div className="row">
                        <div className="col-sm-8 col-sm-offset-2">
                           <div className="navbar text-center">
                              <form className="navbar-form">
                                 <select className="form-control input-sm"  value={data.siteLanguageId}
                                         onChange={(e) => this.handleSiteLanguageChange(data, 'siteLanguageId', e.target.value)}>
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
                  <Modal show={this.state.showModal} onHide={() => this.handleStateChange('showModal', false)}>               
                     <Modal.Body className="bg-danger">
                        <span>{data.texts.internalServerError}</span>
                        <Button className="close" onClick={() => this.handleStateChange('showModal', false)}>x</Button>
                     </Modal.Body>
                  </Modal>
                  <Form>
                     <FormGroup id="nameGroup" validationState={(data.name1Message || data.name2Message) ? 'error' : null}>
                        <label htmlFor="signUpNameInput">{data.texts.name}:</label>
                        <input className="form-control" type="text" id="signUpNameInput" value={data.name ? data.name : ''} onChange={(e) => this.handleDataChange(data, 'name', e.target.value)}/>
                        <div className="text-danger" style={{display: (data.name1Message ? 'block' : 'none')}}>{data.texts.nameMustBeBetween2And16Chars}</div>
                        <div className="text-danger" style={{display: (data.name2Message ? 'block' : 'none')}}>{data.texts.theNameIsAlreadyUsed}</div>
                     </FormGroup>
                     <FormGroup id="pass1Group" validationState={(this.state.pass1aMessageDisplay !== 'none' || this.state.pass1bMessageDisplay !== 'none') ? 'error' : null}>
                        <label htmlFor="signUpPassInput1">{data.texts.password}:</label>
                        <input className="form-control" type="password" id="signUpPassInput1" value={this.state.pass1} onChange={(e) => this.handleStateChange('pass1', e.target.value)}/>
                        <div className="text-danger" style={{display: this.state.pass1aMessageDisplay}}>{data.texts.passwordMustContainAtLeast8Chars}</div>
                        <div className="text-danger" style={{display: this.state.pass1bMessageDisplay}}>{data.texts.passwordMustContainOnlyLettersNumbersEtc}</div>
                     </FormGroup>
                     <FormGroup id="pass2Group" validationState={(this.state.pass2MessageDisplay !== 'none') ? 'error' : null}>
                        <label htmlFor="signUpPassInput2">{data.texts.confirmPassword}:</label>
                        <input className="form-control" type="password" id="signUpPassInput2" value={this.state.pass2} onChange={(e) => this.handleStateChange('pass2', e.target.value)}/>
                        <div className="text-danger" style={{display: this.state.pass2MessageDisplay}}>{data.texts.twoPasswordsAreNotIdentical}</div>
                     </FormGroup>
                     <FormGroup>
                        <div className="row">
                           <div className="col-sm-10 col-xs-10">
                              <button className="btn btn-info" onClick={(e) => this.handleSignUpBtnClick(e)}>{data.texts.signUp}</button>
                              <span> </span>
                              <Button className="btn btn-warning" onClick={(e) => this.handleCancelBtnClick(e)}>{data.texts.cancel}</Button>
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
   
   handleDataChange(data, field, value)
   {
      data[field] = value;
      this.props.changeData(data);
   }
   
   handleStateChange(field, value)
   {
      let state = this.state
      state[field] = value;
      this.setState(state);
   }
   
   // 1- Handlers for user actions
   
   handleSiteLanguageChange(data, field, value)
   {
      this.handleDataChange(data, field, value);
      document.cookie = 'siteLanguageId=' + value + this.props.getCookieExpires(180);
      this.props.socket.emit('languageChoice', { page: 'signUp', languageId: value });
      this.handleStateChange('languageWaitDisplay', 'inline');
   }
   
   handleSignUpBtnClick(e)
   {
      e.preventDefault();
      const name = this.props.userInterfaceState.data.name;
      const pass1 = this.state.pass1;
      const pass2 = this.state.pass2;
      this.props.socket.emit('submitSignUp', { name: name, pass1: pass1, pass2: pass2 });
      this.handleStateChange('buttonWaitDisplay', 'inline');
   }
   
   handleCancelBtnClick(e)
   {
      e.preventDefault();
      this.props.socket.emit('cancelSignUp', {});
   }
   
   // 2- Handlers for server messages
   
   handleUpdateSiteLanguage(texts)
   {
      this.handleStateChange('languageWaitDisplay', 'none');
      this.handleDataChange(this.props.userInterfaceState.data, 'texts', texts);
   }
   
   handleServerError(errorData)
   {
      this.handleStateChange('buttonWaitDisplay', 'none');
      
      const data = this.props.userInterfaceState.data;
      this.handleDataChange(data, 'name1Message', errorData.name1Message);
      this.handleDataChange(data, 'name2Message', errorData.name2Message);
      
      this.handleStateChange('pass1aMessageDisplay', errorData.pass1aMessage ? 'block' : 'none');
      this.handleStateChange('pass1bMessageDisplay', errorData.pass1bMessage ? 'block' : 'none');
      this.handleStateChange('pass2MessageDisplay', errorData.pass2Message ? 'block' : 'none');
      
      if (errorData.errorMessage) this.handleStateChange('showModal', true);
   }
}
