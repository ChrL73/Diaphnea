import React from 'react';
import ReactDOM from 'react-dom';
import { Home } from './home.js'
import { SignUp } from './signUp.js'
import 'bootstrap/dist/css/bootstrap.css';
//import { Navbar, Button } from 'react-bootstrap';

let debugCounter = 0;

class UserInterface extends React.Component
{
   constructor(props)
   {
      super(props);
      
      ++debugCounter;
      if (debugCounter != 1) throw('Error: UserInterface constructor should be called only once');
      
      this.socket = window.io.connect('192.168.50.31:3002');
      this.socket.on('displayPage', (data) => this.handleDisplayPage(data));
      
      this.state = { currentPage: 'none' };
   }
      
   handleDisplayPage(data)
   {
      this.setState({ currentPage: data.page });
   }
   
   render()
   {
      return (
         <div className="userInterface">
            <Home userInterfaceState={this.state} socket={this.socket}/>
            <SignUp userInterfaceState={this.state} socket={this.socket}/>
         </div>);
   }
}

ReactDOM.render(<UserInterface/>, document.getElementById('root'));
