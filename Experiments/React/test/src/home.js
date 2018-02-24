import React from 'react';
import waitGif from './wait.gif'
import { Modal, Button } from 'react-bootstrap';

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
      props.socket.on('indexError', () => this.handleServerError());
      props.socket.on('updateSelects', (data) => this.handleUpdateSelects(data));
      
      this.initialState =
      {
         showModal1: false,
         showModal2: false,
         navBarWaitDisplay: 'none',
         questionnaireWaitDisplay: 'none',
         questionnaireLanguageWaitDisplay: 'none',
         levelWaitDisplay: 'none'
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
      
      let questionnaires;
      if (data.questionnaireList)
      {
         questionnaires = data.questionnaireList.map((questionnaire) =>
         {
            return (<option key={questionnaire.id} value={questionnaire.id}>{questionnaire.name}</option>);
         });
      }
      
      let questionnaireLanguages;
      if (data.questionnaireLanguageList)
      {
         questionnaireLanguages = data.questionnaireLanguageList.map((language) =>
         {
            return (<option key={language.id} value={language.id}>{language.name}</option>);
         });
      }
      
      let levels;
      if (data.levelList)
      {
         levels = data.levelList.map((level) =>
         {
            return (<option key={level.id} value={level.id}>{level.name}</option>);
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
               <Modal show={this.state.showModal1} onHide={() => this.handleStateChange('showModal1', false)}>               
                  <Modal.Body className="bg-danger">
                     <span>{data.texts.unknownUserOrWrongPassword}</span>
                     <Button className="close" onClick={() => this.handleStateChange('showModal1', false)}>x</Button>
                  </Modal.Body>
               </Modal>
               <Modal show={this.state.showModal2} onHide={() => this.handleStateChange('showModal2', false)}>               
                  <Modal.Body className="bg-danger">
                     <span>{data.texts.internalServerError}</span>
                     <Button className="close" onClick={() => this.handleStateChange('showModal2', false)}>x</Button>
                  </Modal.Body>
               </Modal>
               <form>
                  <div className="row">
                     <div className="col-sm-4 col-sm-offset-4">
                        <label className="control-label" htmlFor="indexQuestionnaireSelect">{data.texts.questionnaire}:</label>
                     </div>
                  </div>
                  <div className="row">
                     <div className="col-sm-4 col-sm-offset-4">
                        <select className="form-control" id="indexQuestionnaireSelect" value={data.questionnaireId}
                                onChange={(e) => this.handleQuestionnaireChange(data, 'questionnaireId', e.target.value)}>
                           {questionnaires}
                        </select>
                     </div>
                     <div className="col-sm-1">
                        <img src={waitGif} id="indexQuestionnaireWait" className="waitImg"/>
                     </div>
                  </div>
                  <div className="row" style={{display: data.showQuestionnaireLanguageSelect ? 'block' : 'none'}}>
                     <div className="col-sm-4 col-sm-offset-4">
                        <label className="control-label" htmlFor="indexQuestionnaireLanguageSelect" style={{marginTop: '16px'}}>
                           {data.texts.language}:
                        </label>
                     </div>
                  </div>
                  <div className="row" style={{display: data.showQuestionnaireLanguageSelect ? 'block' : 'none'}}>
                     <div className="col-sm-4 col-sm-offset-4">
                        <select className="form-control" id="indexQuestionnaireLanguageSelect" value={data.questionnaireLanguageId}
                                onChange={(e) => this.handleQuestionnaireLanguageChange(data, 'questionnaireLanguageId', e.target.value)}>
                           {questionnaireLanguages}
                        </select>
                     </div>
                     <div className="col-sm-1">
                        <img src={waitGif} id="indexQuestionnaireLanguageWait" className="waitImg"/>
                     </div>
                  </div>
                  <div className="row">
                     <div className="col-sm-4 col-sm-offset-4">
                        <label className="control-label" htmlFor="indexLevelSelect" id="indexLevelLabel" style={{marginTop: '16px'}}>
                           {data.texts.level}:
                        </label>
                     </div>
                  </div>
                  <div className="row">
                     <div className="col-sm-4 col-sm-offset-4">
                        <select className="form-control" id="indexLevelSelect" value={data.levelId} onChange={(e) => this.handleLevelChange(data, 'levelId', e.target.value)}>
                           {levels}
                        </select>
                     </div>
                     <div className="col-sm-1">
                        <img src={waitGif} id="indexLevelWait" className="waitImg"/>
                     </div>
                  </div>
                  
               </form>
            </div>
         </div>);
   }
   
   /* html += '<div class="row" style="margin-top:20px;"><div class="col-sm-4 col-sm-offset-4"><button id="indexStartBtn" class="btn btn-success">'
         + pageData.texts.start
         + '</button></div><div class="col-sm-1"><img src="wait.gif" id="indexStartWait" class="waitImg"/></div></div>';
   */
   
   renderNavbarForm(data)
   {
      if (data.userName)
      {
         return (
            <form className="navbar-form">
               <label>{data.userName}</label>
               <span> </span>
               <Button className="btn btn-primary btn-sm" onClick={(e) => this.handleSignOutBtnClick(e)}>{data.texts.signOut}</Button>
               <div className="col-sm-1 navbar-right"><img src={waitGif} className="waitImg" style={{display: this.state.navBarWaitDisplay}}/></div>
            </form>);
      }
      else
      {
         return (
            <form className="navbar-form">
               <small>
                  <label htmlFor="indexNameInput">{data.texts.name}:</label>
                  <span> </span>
                  <input className="form-control input-sm" type="text" id="indexNameInput" value={data.tmpName ? data.tmpName : ''} onChange={(e) => this.handleDataChange(data, 'tmpName', e.target.value)}/>
                  <span> </span>
                  <label htmlFor="indexPassInput">{data.texts.password}:</label>
                  <span> </span>
                  <input className="form-control input-sm" type="password" id="indexPassInput"/>
                  <span> </span>
                  <div className="visible-xs"><br/></div>
                  <Button className="btn btn-primary btn-sm" onClick={(e) => this.handleSignInBtnClick(e)}>{data.texts.signIn}</Button>
                  <span> </span>
                  <Button className="btn btn-info btn-sm" onClick={(e) => this.handleSignUpBtnClick(e)}>{data.texts.signUp}</Button>
                  <div className="col-sm-1 navbar-right"><img src={waitGif} className="waitImg"  style={{display: this.state.navBarWaitDisplay}}/></div>
               </small>
            </form>);
      }
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
      if (!data.userName) document.cookie = 'siteLanguageId=' + value + this.props.getCookieExpires(180);
      this.props.socket.emit('languageChoice', { page: 'index', languageId: value });
      this.handleStateChange('navBarWaitDisplay', 'inline');
   }
   
   handleQuestionnaireChange(data, field, value)
   {
      this.handleDataChange(data, field, value);
      this.emitLevelChoice(data);
      document.getElementById('indexQuestionnaireWait').style.display = 'inline';
   }
   
   handleQuestionnaireLanguageChange(data, field, value)
   {
      this.handleDataChange(data, field, value);
      this.emitLevelChoice(data);
      document.getElementById('indexQuestionnaireLanguageWait').style.display = 'inline';
   }
   
   handleLevelChange(data, field, value)
   {
      this.handleDataChange(data, field, value);
      this.emitLevelChoice(data);
      document.getElementById('indexLevelWait').style.display = 'inline';
   }
   
   emitLevelChoice(data)
   {
      // Todo: Adapt this line when table will be implemented
      //d.forEach(function(i) { $('#tbody' + i).empty();});
         
      if (!data.userName)
      {
         var expires = this.props.getCookieExpires(180);
         document.cookie = 'questionnaireId=' + data.questionnaireId + expires;
         document.cookie = 'questionnaireLanguageId=' + data.questionnaireLanguageId + expires;
         document.cookie = 'levelId=' + data.levelId + expires;
      }

      this.props.socket.emit('levelChoice', { questionnaireId: data.questionnaireId, questionnaireLanguageId: data.questionnaireLanguageId, levelId: data.levelId });
   }

   handleSignInBtnClick(e)
   {
      e.preventDefault();
      this.props.socket.emit('signIn', { name: this.props.userInterfaceState.data.tmpName, pass: document.getElementById('indexPassInput').value });
      this.handleStateChange('navBarWaitDisplay', 'inline');
   }

   handleSignOutBtnClick(e)
   {
      e.preventDefault();
      this.props.socket.emit('signOut', {});
      this.handleStateChange('navBarWaitDisplay', 'inline');
   }
   
   handleSignUpBtnClick(e)
   {
      e.preventDefault();
      this.props.socket.emit('signUp', { name: this.props.userInterfaceState.data.tmpName });
      this.handleStateChange('navBarWaitDisplay', 'inline');
   }
   
   // 2- Handlers for server messages
   
   handleUpdateSiteLanguage(texts)
   {
      this.handleStateChange('navBarWaitDisplay', 'none');
      this.handleDataChange(this.props.userInterfaceState.data, 'texts', texts);
      
      // Todo: Adapt this line when table will be implemented
      /*d.forEach(function(i)
      {
         $('#nameTh' + i).text(data.name);
         $('#scoreTh' + i).text(data.score);
         $('#timeTh' + i).text(data.time);
      });*/
         
      this.emitLevelChoice(this.props.userInterfaceState.data);
   }
   
   handleUnknownName()
   {
      this.handleStateChange('navBarWaitDisplay', 'none');
      this.handleStateChange('showModal1', true);
   }
   
   handleServerError()
   {
      this.handleStateChange('navBarWaitDisplay', 'none');
      this.handleStateChange('showModal2', true);
   }
   
   handleUpdateSelects(data)
   {
      document.getElementById('indexQuestionnaireWait').style.display = 'none';
      document.getElementById('indexQuestionnaireLanguageWait').style.display = 'none';
      document.getElementById('indexLevelWait').style.display = 'none';
      
      const data0 = this.props.userInterfaceState.data;

      data0.questionnaireList = data.questionnaireList;
      data0.questionnaireId = data.questionnaireId;
      data0.questionnaireLanguageList = data.questionnaireLanguageList;
      data0.questionnaireLanguageId = data.questionnaireLanguageId;
      data0.showQuestionnaireLanguageSelect = data.showQuestionnaireLanguageSelect;
      data0.levelList = data.levelList;
      data0.levelId = data.levelId;
      this.props.changeData(data0);
      
      // Todo: Adapt this line when table will be implemented
      //updateTable();
   }
}
