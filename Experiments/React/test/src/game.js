import React from 'react';
import { Button } from 'react-bootstrap';
import waitGif from './wait.gif'

export class Game extends React.Component
{
   constructor(props)
   {
      super(props);
      
      props.socket.on('displayPage', (data) => this.handleDisplayPage(data));
      
      this.stateReset =
      {
         timeWaitDisplay: 'none',
         questionWaitVisible: {},
         texts: {}
      };
      
      this.state = this.stateReset;
   }
   
   render()
   {
      let questions;
      if (this.state.questions)
      {
         questions = this.state.questions.map((question, iQuestion) =>
         {
            let disabled = this.state.questionStates[iQuestion].answered;
            
            const choices = question.choices.map((choice, iChoice) =>
            {
               let checked = Boolean(this.state.questionStates[iQuestion].choiceStates[iChoice] & 1);
               let rightChoice = Boolean(this.state.questionStates[iQuestion].choiceStates[iChoice] & 2);
               let classStr;
               if (disabled && rightChoice) classStr = 'boldChoice';
               
               const _this = this;
               function renderInput()
               {
                  if (question.isMultiple)
                  { 
                     return (<input id={'input' + iQuestion + '_' + iChoice} type="checkbox" checked={checked} disabled={disabled} style={{marginLeft: '12px'}}
                                    onChange={(e) => _this.handleCheckBoxChange(e.target)}/>);
                  }
                  else
                  {
                     return (<input id={'input' + iQuestion + '_' + iChoice} type="radio" name={'radio' + iQuestion} checked={checked} disabled={disabled} style={{marginLeft: '12px'}}
                                    onChange={(e) => _this.handleRadioChange(e.target)}/>);
                  }
               }
               
               if (question.isMultiple)
               {
                  if (disabled)
                  {
                     if (checked === rightChoice) classStr = (classStr ? classStr + ' greenChoice' : 'greenChoice');
                     else classStr = (classStr ? classStr + ' redChoice' : 'redChoice');
                  }
               }
               else
               {
                  if (disabled && checked)
                  {
                     if (rightChoice) classStr = (classStr ? classStr + ' greenChoice' : 'greenChoice');
                     else classStr = (classStr ? classStr + ' redChoice' : 'redChoice');
                  }
               }
               
               return (
                  <div key={'input' + iQuestion + '_' + iChoice} >
                     {renderInput()}
                     <span> </span>
                     <span className={classStr}>{choice.text}</span>
                     <span> </span>
                     <span>{(disabled && choice.comment.length) ? (' (' + choice.comment + ')') : ''}</span><br/>
                  </div>);
               
            });
            
            return (
               <div className="question panel panel-info" key={'question' + iQuestion} style={{display: iQuestion === this.state.displayedQuestion ? 'block' : 'none'}}>
                  <div className="panel-heading">
                     {this.state.texts.question}
                     <span> </span>
                     {iQuestion + 1}/{this.state.questions.length}
                     <img src={waitGif} className="waitImg" alt="Waiting for server..."
                          style={{marginTop: '-6px', marginBottom: '-6px', display: (this.state.questionWaitVisible[iQuestion] ? 'inline' : 'none')}}/>
                  </div>
                  <div className="panel-body">
                     <p>{question.question}</p>
                     {choices}
                  </div>
               </div>);
         });
      }
      
      return (
         <div style={{display: (this.state.page === 'game' ? 'block' : 'none')}} className="game">
            <div className="container-fluid">
               <header className="row bg-success" id="gameHeader" style={{paddingLeft: '30px', borderBottom: '1px solid #bbd8a2'}}>
                  <span className="gameHeader">
                     {this.state.texts.questionnaire}:&nbsp;
                     <strong>{this.state.questionnaireName}</strong>
                  </span>
                  <span className="gameHeader">
                     {this.state.texts.level}:&nbsp;
                     <strong>{this.state.levelName}</strong>
                  </span>
                  {this.state.texts.score}:&nbsp;
                  <span className="gameHeader">
                     {this.state.rightAnswerCount}/{this.state.answerCount}
                  </span>
                  {this.state.texts.time}:&nbsp;
                  {this.state.finalTime ? this.state.finalTime : Math.floor(0.001 * this.state.time)}s
                  <img src={waitGif} alt="Waiting for server..." className="waitImg" style={{display: this.state.timeWaitDisplay}}/>
               </header>
               <div className="row">
                  <div className="col-lg-3 col-md-4 col-sm-5">
                     <div id="questionDiv">
                        {questions}
                        <Button className="btn btn-warning" onClick={(e) => this.handleStopBtnClick(e)}>{this.state.texts.stop}</Button>
                     </div>
                  </div>
                  <div className="col-lg-9 col-md-8 col-sm-7" id="canvasColumn">
                     <canvas>
                     </canvas>
                  </div>
               
               </div>
            </div>
         </div>);
   }
   
   /*
      
      
      html += '<div style="margin-top:20px;"><button class="btn btn-primary input-sm" id="previousButton"';
      if (displayedQuestion == 0) html += ' disabled';       
      html += '>' + pageData.texts.previous
         + '</button> <button class="btn btn-success input-sm" id="submitButton"';
      if (questionStates[displayedQuestion].answered) html += ' disabled';
      html += '>' + pageData.texts.submit
         + '</button> <button class="btn btn-primary input-sm" id="nextButton"';
      if (displayedQuestion == questions.length - 1 || !questionStates[displayedQuestion].answered) html += ' disabled';
      html += '>' + pageData.texts.next + '</button></div>';
      
      html += '<form><div style="margin-top:20px; margin-bottom:20px;"><button class="btn btn-warning input-sm" id="gameStopBtn">'
         + pageData.texts.stop
         + '</button><img src="wait.gif" class="waitImg" id="stopGameWaitImg"/></div></form>'
      
      html += '</div>
   */
   
   // 1- Handlers for user actions
   
   handleStopBtnClick(e)
   {
      e.preventDefault();
      this.props.socket.emit('stopGame', {});
   }
   
   handleCheckBoxChange(target)
   {
      const x = target.id.match(/[0-9]+/g);
      const iQuestion = x[0];
      const iChoice = x[1];
      
      let questionStates = this.state.questionStates;
      if (target.checked) questionStates[iQuestion].choiceStates[iChoice] |= 1;
      else questionStates[iQuestion].choiceStates[iChoice] &= 2;
      this.setState({ questionStates: questionStates });
   }
   
   handleRadioChange(target)
   {
      const x = target.id.match(/[0-9]+/g);
      const iQuestion = x[0];
      const iChoice = x[1];
      
      
   }
   
   // 2- Handlers for server messages
   
   handleDisplayPage(data)
   {
      let state = {};
      Object.getOwnPropertyNames(this.stateReset).forEach((property) => { state[property] = this.stateReset[property]; });
      
      if (data.page === 'game') Object.getOwnPropertyNames(data).forEach((property) => { state[property] = data[property]; });
      else state.page = data.page;
      
      this.setState(state);
   }
}
