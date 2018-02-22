import React from 'react';
import ReactDOM from 'react-dom';
import { Home } from './home.js';
import { SignUp } from './signUp.js';
import './styles.css';
import 'bootstrap/dist/css/bootstrap.css';

let userInterfaceDebugCounter = 0;

class UserInterface extends React.Component
{
   constructor(props)
   {
      super(props);
      
      ++userInterfaceDebugCounter;
      if (userInterfaceDebugCounter !== 1) throw(String('Error: UserInterface constructor should be called only once'));
      
      this.socket = window.io.connect('192.168.50.31:3002');
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
      this.setState(
      {
         data: data
      });
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
            <Home userInterfaceState={this.state} socket={this.socket} setUserInterfaceState={this.setState} changeData={(data) => this.setState({ data: data })}
                  getCookieExpires={(days) => this.getCookieExpires(days)}/>
            <SignUp userInterfaceState={this.state} socket={this.socket} setUserInterfaceState={this.setState}/>
         </div>);
   }
}

ReactDOM.render(<UserInterface/>, document.getElementById('root'));
