import React from 'react';
import { Button } from 'react-bootstrap';
import waitGif from './wait.gif'

export class SignUp extends React.Component
{
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
      
      return (
         <div style={{display: (data.page === 'signUp' ? 'block' : 'none')}} className="signUp">
            <div className="container">
               <div className="col-md-4 col-md-offset-4 col-sm-offset-3 col-sm-6">  
                  <header>
                     <div className="row">
                        <div className="col-sm-8 col-sm-offset-2">
                           <div className="navbar text-center">
                              <form className="navbar-form">
                                 <select className="form-control input-sm" >
                                    {siteLanguages}
                                 </select>
                              </form>
                           </div>
                        </div>
                        <div className="col-sm-1 col-xs-1">
                           <img src={waitGif} className="waitImg"/>
                        </div>
                     </div>
                  </header>                 
                  <div className="modal fade" id="signUpErrorMessage">
                     <div className="modal-dialog">
                        <div className="modal-content">
                           <div className="modal-body bg-danger">
                              <span id="signUpErrorSpan">{data.texts.internalServerError}</span>
                              <button type="button" className="close" data-dismiss="modal">x</button>
                           </div>
                        </div>
                     </div>
                  </div>
                  <form>
                     <div className="form-group" id="nameGroup">
                        <label htmlFor="signUpNameInput">{data.texts.name}</label>
                        <input className="form-control" type="text" id="signUpNameInput" value={data.name}/>
                        <div className="errorMessage text-danger" >{data.texts.nameMustBeBetween2And16Chars}</div>
                        <div className="errorMessage text-danger" >{data.texts.theNameIsAlreadyUsed}</div>
                     </div>
                     <div className="form-group" id="pass1Group">
                        <label htmlFor="signUpPassInput1">{data.texts.password}</label>
                        <input className="form-control" type="password" id="signUpPassInput1"/>
                        <div className="errorMessage text-danger">{data.texts.passwordMustContainAtLeast8Chars}</div>
                        <div className="errorMessage text-danger">{data.texts.passwordMustContainOnlyLettersNumbersEtc}</div>
                     </div>
                     <div className="form-group" id="pass2Group">
                        <label htmlFor="signUpPassInput2">{data.texts.confirmPassword}</label>
                        <input className="form-control" type="password" id="signUpPassInput2"/>
                        <div className="errorMessage text-danger">{data.texts.twoPasswordsAreNotIdentical}</div>
                     </div>
                     <div className="form-group">
                        <div className="row">
                           <div className="col-sm-10 col-xs-10">
                              <button className="btn btn-info" id="submitSignUp">{data.texts.signUp}</button>
                              <span> </span>
                              <Button className="btn btn-warning" onClick={(e) => this.handleCancelBtnClick(e)}>{data.texts.cancel}</Button>
                           </div>
                           <div className="col-sm-1 col-xs-1">
                              <img src={waitGif} className="waitImg"/>
                           </div>
                        </div>
                     </div>
                  </form>   
               </div>
            </div>
         </div>);
   }
   
   handleCancelBtnClick(e)
   {
      e.preventDefault();
      this.props.socket.emit('cancelSignUp', {});
   }
}
