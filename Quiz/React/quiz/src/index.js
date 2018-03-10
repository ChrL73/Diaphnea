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
      
      this.socket = window.io.connect('192.168.50.184:3002');
      this.socket.on('displayPage', (data) => this.setState({ page: data.page }));
      
      this.state = { page: 'none' };
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
            <Home socket={this.socket} getCookieExpires={(days) => this.getCookieExpires(days)}/>
            <SignUp socket={this.socket} getCookieExpires={(days) => this.getCookieExpires(days)}/>
            <Game socket={this.socket} />
         </div>);
   }
}

ReactDOM.render(<UserInterface/>, document.getElementById('root'));
