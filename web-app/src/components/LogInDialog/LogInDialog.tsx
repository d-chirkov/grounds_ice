import React from "react";
import { connect } from "react-redux";
import { Dialog } from "primereact/dialog";
import SignInForm from "./SignInForm";
import UserAgreementForm from "./UserAgreementForm"
import SignUpForm from "./SignUpForm"
import Messager from "../../Messager";

interface ILogInDialogStateProps {
	isLoggedIn: boolean,
	serverError: string | null
}

interface ILogInDialogDispatchProps {
	signUp: (username: string, password: string) => void
	signIn: (username: string, password: string) => void
	closeDialog: () => void,
	clearServerError: () => void
}

interface ILogInDialogProps extends ILogInDialogStateProps, ILogInDialogDispatchProps {
}

enum LogInVariant {
	SignIn,
	UserAgreement,
	SignUp
}

interface ILogInDialogState {
	variant: LogInVariant,
	username: string,
	password: string,
	usernameError: string | null,
	passwordError: string | null,
	loading: boolean
}


class LogInDialog extends React.Component<ILogInDialogProps, ILogInDialogState> {
	constructor(props: ILogInDialogProps) {
		super(props);
		this.state = {
			loading: false,
			variant: LogInVariant.SignIn,
			username: "",
			password: "",
			usernameError: null,
			passwordError: null,
		};
		this.getHeader = this.getHeader.bind(this);
		this.getWelcome = this.getWelcome.bind(this);
		this.signIn = this.signIn.bind(this);
		this.signUp = this.signUp.bind(this);
		this.updateUsernameInput = this.updateUsernameInput.bind(this);
		this.updatePasswordInput = this.updatePasswordInput.bind(this);
	}
	
	getHeader(): string {
		switch (this.state.variant) {
			case LogInVariant.SignIn: return "Вход";
			case LogInVariant.UserAgreement: return "Соглашение";
			case LogInVariant.SignUp: return "Регистрация";
		}
	}
	
	getWelcome(): string {
		switch (this.state.variant) {
			case LogInVariant.SignIn: return "Здравствуйте";
			case LogInVariant.SignUp: return "Добро пожаловать";
			default: return "Добро пожаловать";
		}
	}
	
	signIn() {
		let {username, password} = this.state;
		let isError: boolean = false;
		if (username.length === 0) {
			this.setState({usernameError: "Введите логин"});
			isError = true;
		}
		if (password.length === 0) {
			this.setState({passwordError: "Введите пароль"});
			isError = true;
		}
		if (!isError) {
			this.setState({loading: true});
			this.props.signIn(username, password);
		}
	}
	
	signUp() {
		let {username, password} = this.state;
		let isError: boolean = false;
		if (username.length === 0) {
			this.setState({usernameError: "Введите логин"});
			isError = true;
		}
		if (password.length === 0) {
			this.setState({passwordError: "Введите пароль"});
			isError = true;
		}
		if (!isError) {
			this.setState({loading: true});
			this.props.signUp(username, password);
		}
	}
	
	updateUsernameInput(username: string) {
		this.setState({username, usernameError: null});
	}
 	
	updatePasswordInput(password: string) {
		this.setState({password, passwordError: null});
	}
	
	componentWillUpdate() {
		if (this.props.isLoggedIn) {
			this.props.closeDialog();
		}
	}
	
	componentDidUpdate() {
		if (this.props.serverError != null) {
			this.props.clearServerError();
			this.setState({loading: false});
		}
		if (this.props.isLoggedIn) {
			this.props.closeDialog();
		}
	}
	
	render() { 
		let {variant} = this.state;
		return (<div>
			<Dialog
				visible={ true }
				style={{ width: '40vw', height: "23vw" }}
				contentStyle={{ border: "0px", height: "23vw" }}
				closeOnEscape={ true }
				showHeader={ false }
				modal={ true }
				blockScroll={ true }
				onHide={ () => { this.props.closeDialog() } }
				>
				{
					this.props.isLoggedIn ? 
						<div className="w3-container w3-center">
							<h1 className="w3-opacity"><b>{this.getWelcome()}</b></h1>
						</div>:
					variant == LogInVariant.SignIn ? 
						<SignInForm 
							loading={this.state.loading} 
							usernameError={this.state.usernameError}
							passwordError={this.state.passwordError}
							onSignInClick={() => this.signIn()}
							switchToRegistration={ () => this.setState({ variant: LogInVariant.UserAgreement }) }
							updateUsernameInput={ this.updateUsernameInput }
							updatePasswordInput={ this.updatePasswordInput }
							/> :
					variant == LogInVariant.UserAgreement ? 
						<UserAgreementForm switchToSignUp={ () => this.setState({ variant: LogInVariant.SignUp }) }/> :
					variant == LogInVariant.SignUp ? 
						<SignUpForm 
							loading={this.state.loading} 
							usernameError={this.state.usernameError}
							passwordError={this.state.passwordError}
							onSignUpClick={() => this.signUp()}
							updateUsernameInput={ this.updateUsernameInput }
							updatePasswordInput={ this.updatePasswordInput }
							/> : ""
				}
				
			</Dialog>
		</div>
		);
	}
}

import { LogInFormShowAction, LogInFormSetServerErrorAction } from "../../contexts/ui/logInForm/actions";
import { SignUpAction, SignInAction } from "../../contexts/data/account/actions";

let mapStateToProps = (state: any): ILogInDialogStateProps => ({
	isLoggedIn: state.data.account != null,
	serverError: state.ui.logInForm.serverError
})

let mapDispatchToProps = (dispatch: any): ILogInDialogDispatchProps => ({
	signUp: (username: string, password: string) => dispatch(SignUpAction(username, password)),
	signIn: (username: string, password: string) => dispatch(SignInAction(username, password)),
	closeDialog: () => dispatch(new LogInFormShowAction(false)),
	clearServerError: () => dispatch(new LogInFormSetServerErrorAction(null))
})

export default connect<ILogInDialogStateProps, ILogInDialogDispatchProps>(mapStateToProps, mapDispatchToProps)(LogInDialog);