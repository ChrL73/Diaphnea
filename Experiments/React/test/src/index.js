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
      
      this.socket = window.io.connect('gilberte:3002');
      this.socket.on('displayPage', (data) => this.handleDisplayPage(data));
      
      this.state =
      {
         data:
         {
            page: 'none',
            texts: {}
         }
      };
   }
      
   handleDisplayPage(data)
   {
      this.setState({ data: data });
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
            <img src={waitGif} className="waitImg" style={{display: this.state.data.page === 'none' ? 'inline' : 'none'}}/>
            <Home userInterfaceState={this.state} socket={this.socket} changeData={(data) => this.setState({ data: data })}
                  getCookieExpires={(days) => this.getCookieExpires(days)}/>
            <SignUp userInterfaceState={this.state} socket={this.socket}/>
            <Game userInterfaceState={this.state} socket={this.socket}/>
         </div>);
   }
}

ReactDOM.render(<UserInterface/>, document.getElementById('root'));
