import "./LogInDialog.css"

import React from "react";
import { connect } from "react-redux";

import { Dialog } from "primereact/dialog";
import { Button } from "primereact/button";

import { Messager, IMessage } from "../../Messager";
import { signUp, SignUpError} from "../../api/interface/signUp";
import { signIn, SignInError } from "../../api/interface/signIn";

import SignInForm from "./SignInForm";
import UserAgreementForm from "./UserAgreementForm"
import SignUpForm from "./SignUpForm"

interface ILogInDialogStateProps {
	isLoggedIn: boolean,
}

interface ILogInDialogDispatchProps {
	setAccount: (token:string, userId: string, login: string) => void,
	closeDialog: () => void
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
	login: string,
	password: string,
	passwordRepeat: string,
	isLoginIsInvalid: boolean,
	isPasswordIsInvalid: boolean,
	isUserAgreementChecked: boolean,
	loading: boolean
}


class LogInDialog extends React.Component<ILogInDialogProps, ILogInDialogState> {
	constructor(props: ILogInDialogProps) {
		super(props);
		this.state = {
			loading: false,
			variant: LogInVariant.SignIn,
			login: "",
			password: "",
			passwordRepeat: "",
			isLoginIsInvalid: false,
			isPasswordIsInvalid: false,
			isUserAgreementChecked: false
		};
	}
	
	signIn() {
		let {login, password: password} = this.state;
		let errorMessageHeader = "Ошибка авторизации";
		let warnings: string[] = [];
		let isUsernameError = false;
		let isPasswordError = false;
		if (login.length === 0) {
			warnings = [...warnings, "Введите логин"];
			isUsernameError = true;
		}
		if (password.length === 0) {
			warnings = [...warnings, "Введите пароль"];
			isPasswordError = true;
		}
		if (isUsernameError || isPasswordError) {
			Messager.showErrors(warnings.map(v => ({header: errorMessageHeader, message: v})))
			this.setState({isLoginIsInvalid: isUsernameError, isPasswordIsInvalid: isPasswordError});
		} else {
			this.setState({loading: true});
			signIn(login, password, 
				(token: string, userId: string, login: string) => {
					this.props.setAccount(token, userId, login);
				},
				(error: SignInError) => {
					let showWarning = (message: string) =>  Messager.showError(errorMessageHeader, message);
					switch(error) {
						case SignInError.Unauthorized: 
							showWarning("Неверный логин или пароль");
							isUsernameError = true;
							isPasswordError = true;
							break;
						case SignInError.Unexpected: 
							showWarning("Неизвестная ошибка"); 
							break;
					}
					this.setState({loading: false, isLoginIsInvalid: isUsernameError, isPasswordIsInvalid: isPasswordError});
				});
		}
	}
	
	signUp() {
		let {login, password, passwordRepeat} = this.state;
		let errorMessageHeader = "Ошибка регистрации";
		let warnings: string[] = [];
		let isUsernameError = false;
		let isPasswordError = false;
		if (login.length === 0) {
			warnings = [...warnings, "Введите логин"];
			isUsernameError = true;
		}
		if (password.length === 0) {
			warnings = [...warnings, "Введите пароль"];
			isPasswordError = true;
		}
		if (password !== passwordRepeat) {
			warnings = [...warnings, "Пароли не совпадают"];
			isPasswordError = true;
		}
		if (isUsernameError || isPasswordError) {
			Messager.showErrors(warnings.map(v => ({header: errorMessageHeader, message: v})))
			this.setState({isLoginIsInvalid: isUsernameError, isPasswordIsInvalid: isPasswordError});
		} else {
			this.setState({loading: true});
			signUp(login, password, 
				(token: string, userId: string, login: string) => {
					this.props.setAccount(token, userId, login);
				},
				(error: SignUpError) => {
					let showWarning = (message: string) =>  Messager.showError(errorMessageHeader, message);
					switch(error) {
						case SignUpError.LoginAlreadyExists: 
							showWarning("Логин уже используется"); 
							isUsernameError = true; 
							break;
						case SignUpError.LoginNotValid: 
							showWarning("Этот логин не может быть использован");
							isUsernameError = true; 
							break;
						case SignUpError.PasswordNotValid:
							showWarning("Этот пароль не может быть использован");
							isPasswordError = true; 
							break;
						case SignUpError.Unexpected:
							showWarning("Неизвестная ошибка");
							break;
					}
					this.setState({loading: false, isLoginIsInvalid: isUsernameError, isPasswordIsInvalid: isPasswordError});
				});
		}
	}
	
	updateUsernameInput(login: string) {
		this.setState({login, isLoginIsInvalid: false});
	}
 	
	updatePasswordInput(password: string) {
		this.setState({password, isPasswordIsInvalid: false});
	}
	
	updatePasswordRepeatInput(passwordRepeat: string) {
		this.setState({passwordRepeat, isPasswordIsInvalid: false});
	}
	
	swicthToUserAgreement() {
		this.setState({ variant: LogInVariant.UserAgreement, isLoginIsInvalid: false, isPasswordIsInvalid: false })
	}
	
	swicthToSignUp() {
		this.setState({isUserAgreementChecked: true}); setTimeout(() => this.setState({ variant: LogInVariant.SignUp }), 500);
	}
	
	componentDidUpdate() {
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
				showHeader={ false }
				modal={ true }
				blockScroll={ true }
				onHide={ () => this.props.closeDialog() }
				>
				<Button 
					id="LogInDialog_CloseButton"
					icon="pi pi-times" 
					className="p-button-rounded p-button-secondary" 
					style={{position:"absolute", top:0, right:0}} 
					onClick={() => this.props.closeDialog() } />
				{ 
				variant == LogInVariant.SignIn ? 
					<SignInForm 
						loading={this.state.loading} 
						isLoginIsInvalid={this.state.isLoginIsInvalid}
						isPasswordIsInvalid={this.state.isPasswordIsInvalid}
						onSignInClick={() => this.signIn()}
						switchToRegistration={ () => this.swicthToUserAgreement() }
						updateLoginInput={ v => this.updateUsernameInput(v) }
						updatePasswordInput={ v => this.updatePasswordInput(v) }
						/> 
					:
				variant == LogInVariant.UserAgreement ? 
					<UserAgreementForm 
						isUserAgreementChecked={ this.state.isUserAgreementChecked }
						onUserAgreementChecked={ () => this.swicthToSignUp() }/> 
					:
				variant == LogInVariant.SignUp ? 
					<SignUpForm 
						loading={this.state.loading} 
						isLoginIsInvalid={this.state.isLoginIsInvalid}
						isPasswordIsInvalid={this.state.isPasswordIsInvalid}
						onSignUpClick={() => this.signUp()}
						updateLoginInput={ v => this.updateUsernameInput(v) }
						updatePasswordInput={ v => this.updatePasswordInput(v) }
						updatePasswordRepeatInput={ v => this.updatePasswordRepeatInput(v) }
						/> 
					: ""
				}
			</Dialog>
		</div>
		);
	}
}

import { LogInFormShowAction } from "../../contexts/ui/logInForm/actions";
import { SetAccountAction } from "../../contexts/data/account/actions";

let mapStateToProps = (state: any): ILogInDialogStateProps => ({
	isLoggedIn: state.data.account != null
})

let mapDispatchToProps = (dispatch: any): ILogInDialogDispatchProps => ({
	setAccount: (token:string, userId: string, login: string) => dispatch(new SetAccountAction(token, userId, login)),
	closeDialog: () => dispatch(new LogInFormShowAction(false))
})

let LogInDialogHOC = connect<ILogInDialogStateProps, ILogInDialogDispatchProps>(mapStateToProps, mapDispatchToProps)(LogInDialog);

export { LogInDialogHOC as LogInDialog };