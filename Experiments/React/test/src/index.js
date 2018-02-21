import React from 'react';
import ReactDOM from 'react-dom';
import { Home } from './home.js';
import { SignUp } from './signUp.js';
import './styles.css';
import 'bootstrap/dist/css/bootstrap.css';
//import { Navbar, Button } from 'react-bootstrap';

let debugCounter = 0;

class UserInterface extends React.Component
{
   constructor(props)
   {
      super(props);
      
      ++debugCounter;
      if (debugCounter !== 1) throw(String('Error: UserInterface constructor should be called only once'));
      
      this.socket = window.io.connect('albertine:3002');
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
   
   render()
   {
      return (
         <div className="userInterface">
            <Home userInterfaceState={this.state} socket={this.socket} setUserInterfaceState={this.setState} changeData={(data) => this.setState({ data: data })}/>
            <SignUp userInterfaceState={this.state} socket={this.socket} setUserInterfaceState={this.setState}/>
         </div>);
   }
}

ReactDOM.render(<UserInterface/>, document.getElementById('root'));
