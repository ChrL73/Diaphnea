/* --------------------------------------------------------------------
 *
 * Copyright (C) 2018
 *
 * This file is part of the Diaphnea project.
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Affero General Public License
 * version 3 as published by the Free Software Foundation
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 *
 * -------------------------------------------------------------------- */

import React from 'react';
import { Button, Modal } from 'react-bootstrap';
import waitGif from './wait.gif'
import { Map } from './map.js';

export class Game extends React.Component
{
   constructor(props)
   {
      super(props);
      
      this.stateReset =
      {
         showModal: false,
         timeWaitDisplay: 'none',
         questionWaitVisible: {},
         stopGameWaitDisplay: 'none',
         texts: {},
         finalTime: undefined
      };
      
      this.state = this.stateReset;
   }
   
   render()
   {
      const ok = Boolean(this.state.questions);
      
      let questions;
      if (ok)
      {
         questions = this.state.questions.map((question, iQuestion) =>
         {
            const choices = question.choices.map((choice, iChoice) =>
            {
               const renderInput = () =>
               {
                  if (question.isMultiple)
                  { 
                     return (<input id={'input' + iQuestion + '_' + iChoice} type="checkbox" checked={choice.checked} disabled={question.disabled} style={{marginLeft: '12px'}}
                                    onChange={(e) => this.handleCheckBoxChange(e.target)}/>);
                  }
                  else
                  {
                     return (<input id={'input' + iQuestion + '_' + iChoice} type="radio" name={'radio' + iQuestion} value={'_' + iChoice} checked={question.selection === '_' + iChoice}
                                    disabled={question.disabled} style={{marginLeft: '12px'}} onChange={(e) => this.handleRadioChange(e.target)}/>);
                  }
               }
               
               return (
                  <div key={'input' + iQuestion + '_' + iChoice} >
                     {renderInput()}
                     <span> </span>
                     <span className={choice.classStr}>{choice.text}</span>
                     <span> </span>
                     <span>{(question.disabled && choice.comment) ? (' (' + choice.comment + ')') : ''}</span><br/>
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
               <Modal show={this.state.showModal} onHide={() => this.setState({ showModal: false })}>               
                  <Modal.Body>{this.state.texts.areYouSureStopGame}</Modal.Body>
                  <Modal.Footer>
                     <div className="row text-center">
                        <Button style={{marginRight: '4px'}} className="btn btn-primary input-sm" onClick={() => { this.setState({ showModal: false }); this.stopGame(); }}>{this.state.texts.yes}</Button>
                        <Button style={{marginLeft: '4px'}}  className="btn btn-warning input-sm" onClick={() => this.setState({ showModal: false })}>{this.state.texts.no}</Button>
                     </div>
                  </Modal.Footer>
               </Modal>
               <div className="row">
                  <div className="col-lg-3 col-md-4 col-sm-5">
                     {questions}
                     <div style={{marginTop: '20px'}}>
                        <button className="btn btn-primary input-sm" disabled={this.state.displayedQuestion === 0} onClick={(e) => this.handlePreviousBtnClick(e)}>
                           {this.state.texts.previous}
                        </button>
                        <span> </span>
                        <Button className="btn btn-success input-sm"  onClick={(e) => this.handleSubmitBtnClick(e)}
                                disabled={ok ? this.state.questionStates[this.state.displayedQuestion].answered : true}>
                           {this.state.texts.submit}
                        </Button>
                        <span> </span>
                        <Button className="btn btn-primary input-sm" onClick={(e) => this.handleNextBtnClick(e)}
                                disabled={ok ? this.state.displayedQuestion === this.state.questions.length - 1 || !this.state.questionStates[this.state.displayedQuestion].answered : true}>
                           {this.state.texts.next}
                        </Button>
                     </div>
                     <div style={{marginTop: '20px', marginBottom: '20px'}}>
                        <Button className="btn btn-warning input-sm" onClick={(e) => this.handleStopBtnClick(e)}>
                           {ok && this.state.questionStates[this.state.questions.length - 1].answered ? this.state.texts.finish : this.state.texts.stop}
                        </Button>
                        <img src={waitGif} className="waitImg" alt="Waiting for server..." style={{display: this.state.stopGameWaitDisplay}}/>
                     </div>
                  </div>
                  <div className="col-lg-9 col-md-8 col-sm-7" id="mapColumn">
                     <Map gameState={this.state} ref={(x) => { this.map = x; }}/>
                  </div>
               </div>
            </div>
         </div>);
   }
   
   // 1- User action handlers
   
   handleStopBtnClick(e)
   {
      e.preventDefault();
      
      if (!this.state.questionStates[this.state.questions.length - 1].answered) this.setState({ showModal: true });
      else this.stopGame();
   }
   
   stopGame()
   {
      this.setState({ stopGameWaitDisplay: 'inline' });
      this.props.emit('stopGame', {});
   }
   
   handleCheckBoxChange(target)
   {
      const x = target.id.match(/[0-9]+/g);
      const iQuestion = x[0];
      const iChoice = x[1];
      
      let questions = this.state.questions;
      questions[iQuestion].choices[iChoice].checked = target.checked;
      this.setState({ questions: questions });
   }
   
   handleRadioChange(target)
   {
      if (target.checked)
      {
         const x = target.id.match(/[0-9]+/g);
         const iQuestion = x[0];
         const iChoice = x[1];
         
         let questions = this.state.questions;
         questions[iQuestion].selection = '_' + iChoice;
         this.setState({ questions: questions });
      }
   }
   
   handlePreviousBtnClick(e)
   {
      e.preventDefault();
      
      const oldDisplayedQuestion = this.state.displayedQuestion;
      if (oldDisplayedQuestion > 0)
      {
         const newDisplayedQuestion = oldDisplayedQuestion - 1;
         this.setState({ displayedQuestion: newDisplayedQuestion });
         this.props.emit('changeQuestion', { quizId: this.state.quizId, displayedQuestion: newDisplayedQuestion });
         
         this.map.update(newDisplayedQuestion, oldDisplayedQuestion);
      }
   }
   
   handleNextBtnClick(e)
   {
      e.preventDefault();
      
      const oldDisplayedQuestion = this.state.displayedQuestion;
      if (oldDisplayedQuestion < this.state.questions.length - 1)   
      {
         const newDisplayedQuestion = oldDisplayedQuestion + 1;
         this.setState({ displayedQuestion: newDisplayedQuestion });
         this.props.emit('changeQuestion', { quizId: this.state.quizId, displayedQuestion: newDisplayedQuestion });
         
         this.map.update(newDisplayedQuestion, oldDisplayedQuestion);
      }
   }
   
   handleSubmitBtnClick(e)
   {
      e.preventDefault();
      const displayedQuestion = this.state.displayedQuestion;
      
      let questionStates = this.state.questionStates;
      questionStates[displayedQuestion].answered = true;
      
      let questions = this.state.questions;
      questions[displayedQuestion].disabled = true;
      
      let questionWaitVisible = this.state.questionWaitVisible;
      questionWaitVisible[displayedQuestion] = true;
      
      this.setState({ questionStates: questionStates, questions: questions, questionWaitVisible: questionWaitVisible });

      let data = 
      {
         quizId: this.state.quizId,
         question: displayedQuestion,
         checks: []
      };
      
      const question = questions[displayedQuestion];
      question.choices.forEach((choice, iChoice) =>
      {
         if (question.isMultiple) data.checks.push(choice.checked);
         else data.checks.push(question.selection === '_' + iChoice);
      });

      this.props.emit('submit', data); 
   }
   
   windowResize()
   {
      const w = document.getElementById('mapColumn').offsetWidth;
      if (w === 0)
      {
         setTimeout(() => this.windowResize(), 5);
      }
      else
      {
         document.getElementById('mapColumn').style.height = (window.innerHeight - 72).toString() + 'px';
         this.setState(
         {
            mapWidth: w - 30,
            mapHeight: window.innerHeight - 72
         },
            () => this.map.redraw());
      }
   }
   
   // 2- Server message handlers
   
   handleDisplayPage(data)
   {
      const date0 = Date.now();
      
      let state = {};
      Object.getOwnPropertyNames(this.stateReset).forEach((property) => { state[property] = this.stateReset[property]; });
      state.questionWaitVisible = {};
      
      if (data.page === 'game')
      {
         data.questions.forEach((question, iQuestion) =>
         {
            question.disabled = data.questionStates[iQuestion].answered;
            if (question.isMultiple) question.selection = '_0';

            question.choices.forEach((choice, iChoice) =>
            {
               let checked = Boolean(data.questionStates[iQuestion].choiceStates[iChoice] & 1);

               if (question.isMultiple) choice.checked = checked;
               else if (checked) question.selection = '_' + iChoice;

               let rightChoice = Boolean(data.questionStates[iQuestion].choiceStates[iChoice] & 2);
               let classStr;
               if (question.disabled && rightChoice) classStr = 'boldChoice';

               if (question.isMultiple)
               {
                  if (question.disabled)
                  {
                     if (checked === rightChoice) classStr = (classStr ? classStr + ' greenChoice' : 'greenChoice');
                     else classStr = (classStr ? classStr + ' redChoice' : 'redChoice');
                  }
               }
               else
               {
                  if (question.disabled && checked)
                  {
                     if (rightChoice) classStr = (classStr ? classStr + ' greenChoice' : 'greenChoice');
                     else classStr = (classStr ? classStr + ' redChoice' : 'redChoice');
                  }
               }

               choice.classStr = classStr;
            });
         });
         
         Object.getOwnPropertyNames(data).forEach((property) => { state[property] = data[property]; });
   
         this.windowResize();
         window.onresize = () => this.windowResize();
      }
      else
      {
         state.page = data.page;
      }
      
      this.setState(state, () =>
      {
         this.initTime(data.time, date0);
         if (data.page === 'game') this.map.init();
      });
   }
   
   initTime(t0, date0)
   {
      this.setState({ timeWaitDisplay: 'none' });
      clearInterval(this.timeout);
      
      if (!this.state.finalTime && this.state.page === 'game')
      {
         let lastDisplayedTime;

         const updateTime = () =>
         {
            let t = (Date.now() - date0) + t0;
            let displayedTime = Math.floor(0.001 * t);
            if (lastDisplayedTime && (displayedTime < lastDisplayedTime || displayedTime > lastDisplayedTime + 2))
            {
               this.props.emit('timeRequest');
               clearInterval(this.timeout);
               this.setState({ timeWaitDisplay: 'inline' });
            }
            else
            {
               this.setState({ time: t });
               lastDisplayedTime = displayedTime;
               if (!this.state.finalTime && this.state.page === 'game')
               {
                  this.timeout = setTimeout(updateTime, 1000 * (1 + displayedTime) - t);
               }
            }
         }

         this.timeout = setTimeout(updateTime, 1000 * (1 + Math.floor(0.001 * t0)) - t0);
      }
   }
   
   handleUpdateQuestions(data)
   {
      if (data.quizId !== this.state.quizId)
      {
         window.location.replace('/');
      }
      else
      {
         let questionWaitVisible = this.state.questionWaitVisible;
         let questionStates = this.state.questionStates;
         let questions = this.state.questions;
         
         data.questionStates.forEach((state) =>
         {
            var i = state.index;
            
            let question = questions[i];
            questionWaitVisible[i] = false;
            questionStates[i].answered = true;
            question.disabled = true;
            
            state.choiceStates.forEach((choiceState, j) =>
            {
               let choice = question.choices[j];
               
               const checked = Boolean(choiceState.state & 1);  
               if (question.isMultiple) choice.checked = checked;
               else if (checked) question.selection = '_' + j;

               const isRight = Boolean(choiceState.state & 2);
               let classStr;
               if (isRight) classStr = 'boldChoice';

               if (question.isMultiple)
               {       
                  if (checked === isRight) classStr = (classStr ? classStr + ' greenChoice' : 'greenChoice');
                  else classStr = (classStr ? classStr + ' redChoice' : 'redChoice');
               }
               else if (checked)
               {
                  if (isRight) classStr = (classStr ? classStr + ' greenChoice' : 'greenChoice');
                  else classStr = (classStr ? classStr + ' redChoice' : 'redChoice');
               }

               choice.classStr = classStr;

               if (choiceState.comment) choice.comment = choiceState.comment;
            });
         });
         
         let state = 
         {
            rightAnswerCount: data.rightAnswerCount,
            answerCount: data.answerCount,
            questionWaitVisible: questionWaitVisible,
            questionStates: questionStates,
            questions: questions,
            mapInfo: data.mapInfo
         };
         
         if (data.finalTime || this.state.page !== 'game')
         {
            clearInterval(this.timeout);
            state.finalTime = data.finalTime;
         }
         
         this.setState(state, () => this.map.update(this.state.displayedQuestion));
      }
   }
}
