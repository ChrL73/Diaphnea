import React from 'react';
import waitGif from './wait.gif'
import { Modal, Button, Tabs, Tab } from 'react-bootstrap';
import { BootstrapTable, TableHeaderColumn } from 'react-bootstrap-table';
import '../node_modules/react-bootstrap-table/dist/react-bootstrap-table.min.css'

export class Home extends React.Component
{
   constructor(props)
   {
      super(props);
      
      props.socket.on('displayPage', (data) => this.handleDisplayPage(data));
      props.socket.on('updateIndex', (texts) => this.handleUpdateSiteLanguage(texts));
      props.socket.on('unknownName', () => this.handleUnknownName());
      props.socket.on('indexError', () => this.handleServerError());
      props.socket.on('updateSelects', (data) => this.handleUpdateSelects(data));
      props.socket.on('tables', (tableData) => this.handleUpdateTable(tableData));
      
      this.stateReset =
      {
         pass: '',
         showModal1: false,
         showModal2: false,
         showModal3: false,
         navBarWaitDisplay: 'none',
         questionnaireWaitDisplay: 'none',
         questionnaireLanguageWaitDisplay: 'none',
         levelWaitDisplay: 'none',
         startWaitDisplay: 'none',
         tableWaitDisplay: 'none',
         tables:
         {
            n1: [{ rank: '', name: '', score: '', time: '' }],
            n7: [{ rank: '', name: '', score: '', time: '' }],
            n30: [{ rank: '', name: '', score: '', time: '' }],
            n365: [{ rank: '', name: '', score: '', time: '' }]
         },
         texts: {},
         userName: undefined // Required to undefine 'this.state.userName' when the user signs out
      };
      
      this.state = this.stateReset;
      
      this.durations =
      [
         { n: 1, a: 'day'},
         { n: 7, a: 'week'},
         { n: 30, a: 'month'},
         { n: 365, a: 'year'},
      ];
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
      
      let questionnaires;
      if (this.state.questionnaireList)
      {
         questionnaires = this.state.questionnaireList.map((questionnaire) =>
         {
            return (<option key={questionnaire.id} value={questionnaire.id}>{questionnaire.name}</option>);
         });
      }
      
      let questionnaireLanguages;
      if (this.state.questionnaireLanguageList)
      {
         questionnaireLanguages = this.state.questionnaireLanguageList.map((language) =>
         {
            return (<option key={language.id} value={language.id}>{language.name}</option>);
         });
      }
      
      let levels;
      if (this.state.levelList)
      {
         levels = this.state.levelList.map((level) =>
         {
            return (<option key={level.id} value={level.id}>{level.name}</option>);
         });
      }
      
      function rankFormatter(cell, row)
      {
         let format = '';
         if (row.highlight) format += '<div class="lastScore">';
         format += '<strong>' + cell + '</strong>';
         if (row.highlight) format += '</div>';
         return format;
      }
      
      function defaultFormatter(cell, row)
      {
         let format = '';
         if (row.highlight) format += '<div class="lastScore">';
         format += cell;
         if (row.highlight) format += '</div>';
         return format;
      }
      
      function timeFormatter(cell, row)
      {
         let format = '';
         if (row.highlight) format += '<div class="lastScore">';
         format += cell;
         if (cell) format += 's';
         if (row.highlight) format += '</div>';
         return format;
      }
      
      let tabs = this.durations.map((duration) =>
      {
         return (
            <Tab key={duration.n} eventKey={duration.n} title={this.state.texts[duration.a]}>
               <div style={{marginTop: '16px'}}>
                  <BootstrapTable data={this.state.tables['n' + duration.n]} bordered={false} >
                     <TableHeaderColumn width="10%" dataField="rank" isKey={true} dataAlign="center" dataFormat={rankFormatter}></TableHeaderColumn>
                     <TableHeaderColumn width="30%" dataField="name" dataAlign="center" dataFormat={defaultFormatter}>{this.state.texts.name}</TableHeaderColumn>
                     <TableHeaderColumn width="30%" dataField="score" dataAlign="center" dataFormat={defaultFormatter}>{this.state.texts.score}</TableHeaderColumn>
                     <TableHeaderColumn width="30%" dataField="time" dataAlign="center" dataFormat={timeFormatter}>{this.state.texts.time}</TableHeaderColumn>
                 </BootstrapTable>
               </div>
            </Tab>);  
      });
      
      return (
         <div style={{display: (this.state.page === 'index' ? 'block' : 'none')}} className="home">
            <div className="container">
               <div id="content">
                  <header>
                     <div className="navbar">
                        <div className="col-md-10">     
                           {this.renderNavbarForm()}     
                        </div>
                        <div className="col-md-2 text-center">
                           <div className="visible-xs visible-sm"><br/></div>
                           <form className="navbar-form">
                              <select className="form-control input-sm" value={this.state.siteLanguageId}
                                      onChange={(e) => this.handleSiteLanguageChange(e.target.value)}>
                                 {siteLanguages}
                              </select>
                           </form>
                        </div>
                     </div>
                  </header>
                  <Modal show={this.state.showModal1} onHide={() => this.setState({ showModal1: false })}>               
                     <Modal.Body className="bg-danger">
                        <span>{this.state.texts.unknownUserOrWrongPassword}</span>
                        <Button className="close" onClick={() => this.setState({ showModal1: false })}>x</Button>
                     </Modal.Body>
                  </Modal>
                  <Modal show={this.state.showModal2} onHide={() => this.setState({ showModal2: false })}>               
                     <Modal.Body className="bg-danger">
                        <span>{this.state.texts.internalServerError}</span>
                        <Button className="close" onClick={() => this.setState({ showModal2: false })}>x</Button>
                     </Modal.Body>
                  </Modal>
                  <Modal show={this.state.showModal3} onHide={() => this.setState({ showModal3: false })}>               
                     <Modal.Body>
                        <p>{this.state.texts.alphaWarningLine1}</p>
                        <ul>
                           <li>{this.state.texts.alphaWarningLine2}</li>
                           <li>{this.state.texts.alphaWarningLine3}</li>
                           <li>{this.state.texts.alphaWarningLine4}</li>
                           <li>{this.state.texts.alphaWarningLine5}</li>
                        </ul>
                        <p>
                           {this.state.texts.alphaWarningLine6}
                           <span> </span>
                           <a target="_blank" href={this.state.issueUrl}>{this.state.issueUrl}</a>
                        </p>
                        
                     </Modal.Body>
                     <Modal.Footer>
                        <div className="row text-center">
                           <Button className="btn btn-primary input-sm" onClick={() => { this.setState({ showModal3: false }); }}>{this.state.texts.close}</Button>
                        </div>
                     </Modal.Footer>
                  </Modal>
                  <form>
                     <div className="row">
                        <div className="col-sm-4 col-sm-offset-4">
                           <label className="control-label" htmlFor="indexQuestionnaireSelect">{this.state.texts.questionnaire}:</label>
                        </div>
                     </div>
                     <div className="row">
                        <div className="col-sm-4 col-sm-offset-4">
                           <select className="form-control" id="indexQuestionnaireSelect" value={this.state.questionnaireId}
                                   onChange={(e) => this.handleQuestionnaireChange(e.target.value)}>
                              {questionnaires}
                           </select>
                        </div>
                        <div className="col-sm-1">
                           <img src={waitGif} className="waitImg" alt="Waiting for server..." style={{display: this.state.questionnaireWaitDisplay}}/>
                        </div>
                     </div>
                     <div className="row" style={{display: this.state.showQuestionnaireLanguageSelect ? 'block' : 'none'}}>
                        <div className="col-sm-4 col-sm-offset-4">
                           <label className="control-label" htmlFor="indexQuestionnaireLanguageSelect" style={{marginTop: '16px'}}>
                              {this.state.texts.language}:
                           </label>
                        </div>
                     </div>
                     <div className="row" style={{display: this.state.showQuestionnaireLanguageSelect ? 'block' : 'none'}}>
                        <div className="col-sm-4 col-sm-offset-4">
                           <select className="form-control" id="indexQuestionnaireLanguageSelect" value={this.state.questionnaireLanguageId}
                                   onChange={(e) => this.handleQuestionnaireLanguageChange(e.target.value)}>
                              {questionnaireLanguages}
                           </select>
                        </div>
                        <div className="col-sm-1">
                           <img src={waitGif} className="waitImg" alt="Waiting for server..." style={{display: this.state.questionnaireLanguageWaitDisplay}}/>
                        </div>
                     </div>
                     <div className="row">
                        <div className="col-sm-4 col-sm-offset-4">
                           <label className="control-label" htmlFor="indexLevelSelect" id="indexLevelLabel" style={{marginTop: '16px'}}>
                              {this.state.texts.level}:
                           </label>
                        </div>
                     </div>
                     <div className="row">
                        <div className="col-sm-4 col-sm-offset-4">
                           <select className="form-control" id="indexLevelSelect" value={this.state.levelId} onChange={(e) => this.handleLevelChange(e.target.value)}>
                              {levels}
                           </select>
                        </div>
                        <div className="col-sm-1">
                           <img src={waitGif} className="waitImg" alt="Waiting for server..." style={{display: this.state.levelWaitDisplay}}/>
                        </div>
                     </div>
                     <div className="row" style={{marginTop: '20px'}}>
                        <div className="col-sm-4 col-sm-offset-4">
                           <Button id="indexStartBtn" className="btn btn-success" onClick={(e) => this.handleStartBtnClick(e)}>{this.state.texts.start}</Button>
                        </div>
                        <div className="col-sm-1">
                           <img src={waitGif} className="waitImg" alt="Waiting for server..." style={{display: this.state.startWaitDisplay}}/>
                        </div>
                     </div>
                  </form>
                  <div className="row">
                     <div className="col-sm-10 col-sm-offset-1" style={{marginTop: '60px'}}>
                        <div className="row">
                           <div className="text-center col-sm-10 col-sm-offset-1" style={{marginBottom: '6px'}}>
                              <strong>{this.state.texts.highScores}</strong>
                           </div>
                           <div className="text-center col-sm-1">
                              <img src={waitGif} className="waitImg" alt="Waiting for server..." style={{display: this.state.tableWaitDisplay}}/>
                           </div>
                        </div>
                        <Tabs activeKey={this.state.scoreTab} onSelect={(key) => this.handleSelectTab(key)} id="tableTabs" animation={false}>
                           {tabs}
                        </Tabs>
                     </div>
                  </div>
               </div>
               <footer className={this.state.footerClass}>
                  <div className="row">
                     <div>
                        {this.state.texts.version}:
                        <span> </span>
                        {this.state.version}
                     </div>
                     <div>
                        <a onClick={() => this.setState({ showModal3: 'true' })} style={{cursor: 'pointer'}}>{this.state.texts.warning}</a>
                     </div>
                     <div>
                        {this.state.texts.sourceCode}:
                        <span> </span>
                        <a target="_blank" href={this.state.sourceUrl}>{this.state.sourceUrl}</a>
                     </div>
                     <div>
                        {this.state.texts.issues}:
                        <span> </span>
                        <a target="_blank" href={this.state.issueUrl}>{this.state.issueUrl}</a>
                     </div>
                  </div>
               </footer>
            </div>
         </div>);
   }
   
   renderNavbarForm()
   {
      if (this.state.userName)
      {
         return (
            <form className="navbar-form">
               <label>{this.state.userName}</label>
               <span> </span>
               <Button className="btn btn-primary btn-sm" onClick={(e) => this.handleSignOutBtnClick(e)}>{this.state.texts.signOut}</Button>
               <div className="col-sm-1 navbar-right"><img src={waitGif} className="waitImg" alt="Waiting for server..." style={{display: this.state.navBarWaitDisplay}}/></div>
            </form>);
      }
      else
      {
         return (
            <form className="navbar-form">
               <small>
                  <label htmlFor="indexNameInput">{this.state.texts.name}:</label>
                  <span> </span>
                  <input className="form-control input-sm" type="text" id="indexNameInput" value={this.state.tmpName ? this.state.tmpName : ''}
                         onChange={(e) => this.setState({ tmpName: e.target.value })}/>
                  <span> </span>
                  <label htmlFor="indexPassInput">{this.state.texts.password}:</label>
                  <span> </span>
                  <input className="form-control input-sm" type="password" id="indexPassInput" value={this.state.pass} onChange={(e) => this.setState({ pass: e.target.value })}/>
                  <span> </span>
                  <div className="visible-xs"><br/></div>
                  <Button className="btn btn-primary btn-sm" onClick={(e) => this.handleSignInBtnClick(e)}>{this.state.texts.signIn}</Button>
                  <span> </span>
                  <Button className="btn btn-info btn-sm" onClick={(e) => this.handleSignUpBtnClick(e)}>{this.state.texts.signUp}</Button>
                  <div className="col-sm-1 navbar-right"><img src={waitGif} className="waitImg" alt="Waiting for server..." style={{display: this.state.navBarWaitDisplay}}/></div>
               </small>
            </form>);
      }
   }
   
   // 1- User action handlers
   
   handleSiteLanguageChange(value)
   {
      this.setState({ siteLanguageId: value, navBarWaitDisplay: 'inline' });
      if (!this.state.userName) document.cookie = 'siteLanguageId=' + value + this.props.getCookieExpires(180);
      this.props.socket.emit('languageChoice', { page: 'index', languageId: value });
   }
   
   handleQuestionnaireChange(value)
   {
      this.setState({ questionnaireId: value, questionnaireWaitDisplay: 'inline' }, () => this.emitLevelChoice());
   }
   
   handleQuestionnaireLanguageChange(value)
   {
      this.setState({ questionnaireLanguageId: value, questionnaireLanguageWaitDisplay: 'inline' }, () => this.emitLevelChoice());
   }
   
   handleLevelChange(value)
   {
      this.setState({ levelId: value, levelWaitDisplay: 'inline' }, () => this.emitLevelChoice());
   }
   
   emitLevelChoice()
   {
      this.setState(
      {
         tables:
         {
            n1: [{ rank: '', name: '', score: '', time: '' }],
            n7: [{ rank: '', name: '', score: '', time: '' }],
            n30: [{ rank: '', name: '', score: '', time: '' }],
            n365: [{ rank: '', name: '', score: '', time: '' }]
         }
      });
         
      if (!this.state.userName)
      {
         var expires = this.props.getCookieExpires(180);
         document.cookie = 'questionnaireId=' + this.state.questionnaireId + expires;
         document.cookie = 'questionnaireLanguageId=' + this.state.questionnaireLanguageId + expires;
         document.cookie = 'levelId=' + this.state.levelId + expires;
      }

      this.props.socket.emit('levelChoice', { questionnaireId: this.state.questionnaireId, questionnaireLanguageId: this.state.questionnaireLanguageId, levelId: this.state.levelId });
   }

   handleSignInBtnClick(e)
   {
      e.preventDefault();
      this.props.socket.emit('signIn', { name: this.state.tmpName, pass: this.state.pass });
      this.setState({ navBarWaitDisplay: 'inline' });
   }

   handleSignOutBtnClick(e)
   {
      e.preventDefault();
      this.props.socket.emit('signOut', {});
      this.setState({ navBarWaitDisplay: 'inline' }); 
   }
   
   handleSignUpBtnClick(e)
   {
      e.preventDefault();
      this.props.socket.emit('signUp', { name: this.state.tmpName });
      this.setState({ navBarWaitDisplay: 'inline' });
   }
   
   handleStartBtnClick(e)
   {
      e.preventDefault();
      this.props.socket.emit('newGame', {});
      this.setState({ startWaitDisplay: 'inline' });
   }
   
   handleSelectTab(key)
   {
      this.setState({ scoreTab: key }, () => this.resize1());
      this.props.socket.emit('scoreTab', { n: key });
   }
   
   // 2- Server message handlers
   
   handleDisplayPage(data)
   {
      let state = {};
      Object.getOwnPropertyNames(this.stateReset).forEach((property) => { state[property] = this.stateReset[property]; });
      
      if (data.page === 'index')
      {
         Object.getOwnPropertyNames(data).forEach((property) => { state[property] = data[property]; });
         this.setState(state, () => { this.emitUpdateTables(); this.resize1(); });
         
         window.onresize = () => this.resize1();
      }
      else
      {
         state.page = data.page;
         this.setState(state);
      }
   }
   
   resize1()
   {
      clearTimeout(this.timeout);
      this.timeout = setTimeout(() => this.resize2(), 100);
   }
   
   resize2()
   {
      const h1 = document.getElementById('content').offsetHeight;
      const h2 = window.innerHeight;
      this.setState({ footerClass: h1 > h2 - 150 ? '' : 'fixedFooter' });
   }
   
   emitUpdateTables()
   {
      this.props.socket.emit('getTables', { questionnaireId: this.state.questionnaireId, levelId: this.state.levelId });
      this.setState({ tableWaitDisplay: 'inline' });
   }
   
   handleUpdateTable(tableData)
   {
      let stateTables = this.state.tables;
      
      if (tableData.questionnaireId === this.state.questionnaireId && tableData.levelId === this.state.levelId)
      {
         tableData.tables.forEach(function(table)
         {
            let t = [];
            table.rows.forEach(function(row, rank)
            {
               const r = { rank: rank + 1, name: row.name, score: row.score, time: (0.001 * row.time_ms).toFixed(3), highlight: Boolean(row.highlight) };
               t.push(r);
            });
            
            if (t.length === 0) t = [{ rank: '', name: '', score: '', time: '' }];
            stateTables['n' + table.d] = t;
         });
         
         this.setState({ tables: stateTables, tableWaitDisplay: 'none' }, () => this.resize1());
      }
      else
      {
         setTimeout(this.emitUpdateTables, 200);
      }
   }
   
   handleUpdateSiteLanguage(texts)
   {
      this.setState({ navBarWaitDisplay: 'none', texts: texts });
      this.emitLevelChoice();
   }
   
   handleUnknownName()
   {
      if (this.state.page === 'index') this.setState({ navBarWaitDisplay: 'none', showModal1: 'true' });
   }
   
   handleServerError()
   {
      if (this.state.page === 'index') this.setState({ navBarWaitDisplay: 'none', showModal2: 'true' });
   }
   
   handleUpdateSelects(data)
   {
      this.setState(
      {
         questionnaireWaitDisplay: 'none',
         questionnaireLanguageWaitDisplay: 'none',
         levelWaitDisplay: 'none',
         questionnaireList: data.questionnaireList,
         questionnaireId: data.questionnaireId,
         questionnaireLanguageList: data.questionnaireLanguageList,
         questionnaireLanguageId: data.questionnaireLanguageId,
         showQuestionnaireLanguageSelect: data.showQuestionnaireLanguageSelect,
         levelList: data.levelList,
         levelId: data.levelId
      },
         () => { this.emitUpdateTables(); this.resize1(); });
   }
}
