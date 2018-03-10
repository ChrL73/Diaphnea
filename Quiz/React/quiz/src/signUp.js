import React from 'react';
import { Form, FormGroup, Modal, Button, Tabs, Tab } from 'react-bootstrap';
import waitGif from './wait.gif'

export class SignUp extends React.Component
{
   constructor(props)
   {
      super(props);
      
      props.socket.on('displayPage', (data) => this.handleDisplayPage(data));
      props.socket.on('updateSignUp', (texts) => this.handleUpdateSiteLanguage(texts));
      props.socket.on('signUpError', (data) => this.handleServerError(data));
      props.socket.on('unknownName', () => this.handleUnknownName());
      props.socket.on('indexError', () => this.handleServerError());
      
      this.stateReset =
      {
         pass1: '',
         pass2: '',
         showModal1: false,
         showModal2: false,
         pass1aMessage: '',
         pass1bMessage: '',
         pass2MessageDisplay: 'none',
         languageWaitDisplay: 'none',
         buttonWaitDisplay: 'none',
         texts: {},
         questionnaireName: undefined,
         levelName: undefined,
         rightAnswerCount: undefined,
         finalTime: undefined,
         tabKey: 0
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
               <div className="col-md-6 col-md-offset-3 col-sm-offset-3 col-sm-6">  
                  <header>
                     <div className="row" style={{display: this.state.questionnaireName ? 'none' : 'block'}}>
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
                     <div style={{display: this.state.questionnaireName ? 'block' : 'none'}}>
                        <p className="text-center"><strong>{this.state.texts.yourScore}:</strong></p>
                        <div className="text-center">
                           <span>{this.state.texts.questionnaire}:</span>
                           <span> </span>
                           <span>{this.state.questionnaireName}</span>
                        </div>
                        <div className="text-center">
                           <span>{this.state.texts.level}:</span>
                           <span> </span>
                           <span>{this.state.levelName}</span>
                        </div>
                        <div className="text-center">
                           <span>{this.state.texts.score}:</span>
                           <span> </span>
                           <span>{this.state.rightAnswerCount}</span>
                        </div>
                        <div className="text-center">
                           <span>{this.state.texts.time}:</span>
                           <span> </span>
                           <span>{this.state.finalTime}s</span>
                        </div>
                        <p className="text-center" style={{marginTop: '8px'}}>{this.state.texts.signUpProposal}</p>
                     </div>
                  </header>
               </div>
               <div className="col-md-4 col-md-offset-4 col-sm-offset-3 col-sm-6">
                  <Modal show={this.state.showModal1} onHide={() => this.setState({ showModal1: false })}>               
                     <Modal.Body className="bg-danger">
                        <span>{this.state.texts.unknownUserOrWrongPassword}</span>
                        <Button className="close" onClick={() => this.setState({ showModal1: false })}>x</Button>
                     </Modal.Body>
                  </Modal>
                  <Modal show={this.state.showModal2} onHide={() => this.setState({ showModal2: false })} >               
                     <Modal.Body className="bg-danger">
                        <span>{this.state.texts.internalServerError}</span>
                        <Button className="close" onClick={() => this.setState({ showModal2: false })}>x</Button>
                     </Modal.Body>
                  </Modal>
                  <Tabs activeKey={this.state.tabKey} onSelect={(key) => this.handleSelectTab(key)} style={{marginBottom: '12px', display: this.state.questionnaireName ? 'block' : 'none'}} id="tabs" >
                     <Tab key={0} eventKey={0} title={this.state.texts.signUp}></Tab>
                     <Tab key={1} eventKey={1} title={this.state.texts.signIn}></Tab>
                  </Tabs>
                  <Form>
                     <FormGroup id="nameGroup" validationState={(this.state.tabKey === 0 && (this.state.name1Message || this.state.name2Message)) ? 'error' : null}>
                        <label htmlFor="signUpNameInput">{this.state.texts.name}:</label>
                        <input className="form-control" type="text" id="signUpNameInput" value={this.state.name ? this.state.name : ''}
                               onChange={(e) => this.setState({ name: e.target.value })}/>
                        <div className="text-danger" style={{display: (this.state.tabKey === 0 && this.state.name1Message ? 'block' : 'none')}}>{this.state.texts.nameMustBeBetween2And16Chars}</div>
                        <div className="text-danger" style={{display: (this.state.tabKey === 0 && this.state.name2Message ? 'block' : 'none')}}>{this.state.texts.theNameIsAlreadyUsed}</div>
                     </FormGroup>
                     <FormGroup id="pass1Group" validationState={(this.state.tabKey === 0 && (this.state.pass1aMessage || this.state.pass1bMessage)) ? 'error' : null}>
                        <label htmlFor="signUpPassInput1">{this.state.texts.password}:</label>
                        <input className="form-control" type="password" id="signUpPassInput1" value={this.state.pass1}
                               onChange={(e) => this.setState({ pass1: e.target.value })}/>
                        <div className="text-danger" style={{display: (this.state.tabKey === 0 && this.state.pass1aMessage ? 'block' : 'none')}}>{this.state.texts.passwordMustContainAtLeast8Chars}</div>
                        <div className="text-danger" style={{display: (this.state.tabKey === 0 && this.state.pass1bMessage ? 'block' : 'none')}}>{this.state.texts.passwordMustContainOnlyLettersNumbersEtc}</div>
                     </FormGroup>
                     <FormGroup id="pass2Group" validationState={(this.state.pass2MessageDisplay !== 'none') ? 'error' : null} style={{display: this.state.tabKey === 0 ? 'block' : 'none'}}>
                        <label htmlFor="signUpPassInput2">{this.state.texts.confirmPassword}:</label>
                        <input className="form-control" type="password" id="signUpPassInput2" value={this.state.pass2}
                               onChange={(e) => this.setState({ pass2: e.target.value })}/>
                        <div className="text-danger" style={{display: this.state.pass2MessageDisplay}}>{this.state.texts.twoPasswordsAreNotIdentical}</div>
                     </FormGroup>
                     <FormGroup>
                        <div className="row">
                           <div className="col-sm-10 col-xs-10">
                              <Button className="btn btn-primary" onClick={(e) => this.handleSignInBtnClick(e)} style={{display: this.state.tabKey === 1 ? 'inline' : 'none'}}>
                                 {this.state.texts.signIn}
                              </Button>
                              <Button className="btn btn-info" onClick={(e) => this.handleSignUpBtnClick(e)} style={{display: this.state.tabKey === 0 ? 'inline' : 'none'}}>
                                 {this.state.texts.signUp}
                              </Button>
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
   
   handleSignInBtnClick(e)
   {
      e.preventDefault();
      this.props.socket.emit('signIn', { name: this.state.name, pass: this.state.pass1 });
      this.setState({ buttonWaitDisplay: 'inline' });
   }
   
   handleSelectTab(key)
   {
      this.setState({ tabKey: key });
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
   
   handleUnknownName()
   {
      if (this.state.page === 'signUp') this.setState({ buttonWaitDisplay: 'none', showModal1: 'true' });
   }
   
   handleServerError(data)
   {
      if (data)
      {
         this.setState(
         {
            buttonWaitDisplay: 'none',
            name1Message: data.name1Message,
            name2Message: data.name2Message,
            pass1aMessage: data.pass1aMessage,
            pass1bMessage: data.pass1bMessage,
            pass2MessageDisplay: data.pass2Message ? 'block' : 'none',
            showModal2: Boolean(data.errorMessage)
         });
      }
      else if (this.state.page === 'signUp')
      {
         this.setState({ buttonWaitDisplay: 'none', showModal2: true });
      }
   }
}
