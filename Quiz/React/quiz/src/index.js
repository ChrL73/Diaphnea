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
import ReactDOM from 'react-dom';
import { Home } from './home.js';
import { SignUp } from './signUp.js';
import { Game } from './game.js';
import waitGif from './wait.gif'
import './styles.css';
import 'bootstrap/dist/css/bootstrap.css';

class UserInterface extends React.Component
{
   constructor(props)
   {
      super(props);
      
      this.state = { page: 'none' };
   }
   
   componentDidMount()
   {
      this.socket = window.io.connect('albertine:3002');
      
      this.socket.on('displayPage', (data) =>
      {
         this.setState({ page: data.page });
         this.home.handleDisplayPage(data);
         this.game.handleDisplayPage(data);
         this.signUp.handleDisplayPage(data);
      });
      
      this.socket.on('unknownName', () =>
      {
         this.home.handleUnknownName();
         this.signUp.handleUnknownName();
      });
      
      this.socket.on('indexError', () =>
      {
         this.home.handleServerError();
         this.signUp.handleServerError();
      });
      
      this.socket.on('updateIndex', (texts) => this.home.handleUpdateSiteLanguage(texts));
      this.socket.on('updateSelects', (data) => this.home.handleUpdateSelects(data));
      this.socket.on('tables', (tableData) => this.home.handleUpdateTable(tableData));  
      
      this.socket.on('updateQuestions', (data) => this.game.handleUpdateQuestions(data));
      this.socket.on('time', (time) => this.game.initTime(time, Date.now())); 
      
      this.socket.on('updateSignUp', (texts) => this.signUp.handleUpdateSiteLanguage(texts));
      this.socket.on('signUpError', (data) => this.signUp.handleServerError(data));
   }
   
   emit(message, data)
   {
      if (this.socket) this.socket.emit(message, data);
   }
   
   getCookieExpires(days)
   {
      let date = new Date();
      date.setTime(date.getTime() + days * 86400000);
      return "; expires=" + date.toGMTString();
   }
   
   render()
   {
      return (
         <div className="userInterface">
            <img src={waitGif} className="waitImg" alt="Waiting for server..." style={{display: this.state.page === 'none' ? 'inline' : 'none'}}/>
            <Home ref={(x) => { this.home = x; }} getCookieExpires={(days) => this.getCookieExpires(days)} emit={(message, data) => this.emit(message, data)}/>
            <SignUp ref={(x) => { this.signUp = x; }} getCookieExpires={(days) => this.getCookieExpires(days)} emit={(message, data) => this.emit(message, data)}/>
            <Game ref={(x) => { this.game = x; }} emit={(message, data) => this.emit(message, data)}/>
         </div>);
   }
}

ReactDOM.render(<UserInterface/>, document.getElementById('root'));
