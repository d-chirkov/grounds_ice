import "./LogInDialog.css"

import React from "react";
import { connect } from "react-redux";
import { Dialog } from "primereact/dialog";
import SignInForm from "./SignInForm";
import UserAgreementForm from "./UserAgreementForm"
import SignUpForm from "./SignUpForm"

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
	passwordRepeat: string,
	usernameError: string | null,
	passwordError: string | null,
	userAgreementChecked: boolean,
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
			passwordRepeat: "",
			usernameError: null,
			passwordError: null,
			userAgreementChecked: false
		};
		this.getHeader = this.getHeader.bind(this);
		this.getWelcome = this.getWelcome.bind(this);
		this.signIn = this.signIn.bind(this);
		this.signUp = this.signUp.bind(this);
		this.updateUsernameInput = this.updateUsernameInput.bind(this);
		this.updatePasswordInput = this.updatePasswordInput.bind(this);
		this.updatePasswordRepeatInput = this.updatePasswordRepeatInput.bind(this);
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
		let {username, password, passwordRepeat} = this.state;
		let isError: boolean = false;
		if (username.length === 0) {
			this.setState({usernameError: "Введите логин"});
			isError = true;
		}
		if (password.length === 0) {
			this.setState({passwordError: "Введите пароль"});
			isError = true;
		}
		if (password !== passwordRepeat) {
			this.setState({passwordError: "Пароли не совпадают"});
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
	
	updatePasswordRepeatInput(passwordRepeat: string) {
		this.setState({passwordRepeat, passwordError: null});
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
					<Button 
						id="LogInDialog_CloseButton"
						icon="pi pi-times" 
						className="p-button-rounded p-button-secondary" 
						style={{position:"absolute", top:0, right:0}} 
						onClick={() => { this.props.closeDialog() }} />
					{variant == LogInVariant.SignIn ? 
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
						<UserAgreementForm 
							userAgreementChecked={ this.state.userAgreementChecked }
							onUserAgreementChecked={ 
							() => { this.setState({userAgreementChecked: true}); setTimeout(() => this.setState({ variant: LogInVariant.SignUp }), 1000) } }/> :
					variant == LogInVariant.SignUp ? 
						<SignUpForm 
							loading={this.state.loading} 
							usernameError={this.state.usernameError}
							passwordError={this.state.passwordError}
							onSignUpClick={() => this.signUp()}
							updateUsernameInput={ this.updateUsernameInput }
							updatePasswordInput={ this.updatePasswordInput }
							updatePasswordRepeatInput={ this.updatePasswordRepeatInput }
							/> : ""
					}
			</Dialog>
		</div>
		);
	}
}

import { LogInFormShowAction, LogInFormSetServerErrorAction } from "../../contexts/ui/logInForm/actions";
import { SignUpAction, SignInAction } from "../../contexts/data/account/actions";
import { Button } from "primereact/button";

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