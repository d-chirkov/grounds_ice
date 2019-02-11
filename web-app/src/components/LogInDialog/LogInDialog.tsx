import "../../styles/PopupDialog.css"

import React from "react";
import { connect } from "react-redux";

import { Dialog } from "primereact/dialog";
import { Button } from "primereact/button";

import { Messager } from "../../Messager";
import * as SignUp from "../../api/Account/interface/signUp";
import * as SignIn from "../../api/Account/interface/signIn";

import SignInForm from "./SignInForm";
import UserAgreementForm from "./UserAgreementForm"
import SignUpForm from "./SignUpForm"

interface ILogInDialogMapProps {
	isLoggedIn: boolean,
	isDialogVisible: boolean,
}

interface ILogInDialogDispatchMapProps {
	setAccount: (token:string, userId: string, login: string) => void,
	closeDialog: () => void
}

interface ILogInDialogProps extends ILogInDialogMapProps, ILogInDialogDispatchMapProps {
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

let initialLogInDialogState: ILogInDialogState = {
	loading: false,
	variant: LogInVariant.SignIn,
	login: "",
	password: "",
	passwordRepeat: "",
	isLoginIsInvalid: false,
	isPasswordIsInvalid: false,
	isUserAgreementChecked: false
}

class LogInDialog extends React.Component<ILogInDialogProps, ILogInDialogState> {
	constructor(props: ILogInDialogProps) {
		super(props);
		this.state = initialLogInDialogState;
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
			Messager.showManyErrors(warnings.map(v => ({header: errorMessageHeader, message: v})))
			this.setState({isLoginIsInvalid: isUsernameError, isPasswordIsInvalid: isPasswordError});
		} else {
			this.setState({loading: true});
			SignIn.perform(login, password, 
				(token: string, userId: string, login: string) => {
					this.props.setAccount(token, userId, login);
				},
				(error: SignIn.Error) => {
					let showWarning = (message: string) =>  Messager.showError(message);
					switch(error) {
						case SignIn.Error.Unauthorized: 
							showWarning("Неверный логин или пароль");
							isUsernameError = true;
							isPasswordError = true;
							break;
						case SignIn.Error.Unexpected: 
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
			Messager.showManyErrors(warnings.map(v => ({header: errorMessageHeader, message: v})))
			this.setState({isLoginIsInvalid: isUsernameError, isPasswordIsInvalid: isPasswordError});
		} else {
			this.setState({loading: true});
			SignUp.perform(login, password, 
				(token: string, userId: string, login: string) => {
					Messager.showSuccess("Добро пожаловать, " + login + "!");
					this.props.setAccount(token, userId, login);
				},
				(error: SignUp.Error) => {
					let showWarning = (message: string) =>  Messager.showError(message);
					switch(error) {
						case SignUp.Error.LoginAlreadyExists: 
							showWarning("Логин уже используется"); 
							isUsernameError = true; 
							break;
						case SignUp.Error.LoginNotValid: 
							showWarning("Этот логин не может быть использован");
							isUsernameError = true; 
							break;
						case SignUp.Error.PasswordNotValid:
							showWarning("Этот пароль не может быть использован");
							isPasswordError = true; 
							break;
						case SignUp.Error.Unexpected:
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
	
	shouldComponentUpdate(nextProps: ILogInDialogProps): boolean {
		if (!this.props.isDialogVisible && !nextProps.isDialogVisible) {
			return false;
		}
		if (this.props.isDialogVisible != nextProps.isDialogVisible) {
			this.setState(initialLogInDialogState);
		}
		return true;
	}
	
	componentDidUpdate() {
		if (this.props.isLoggedIn) {
			this.props.closeDialog();
		}
	}
	
	render() { 
		if (!this.props.isDialogVisible) {
			return null;
		}
		let {variant} = this.state;
		let dialogHeight = "23vw";
		return (<div>
			<Dialog
				visible={ true }
				className="popup-dialog"
				style={{ height: dialogHeight }}
				contentStyle={{ border: "0px", height: dialogHeight }}
				showHeader={ false }
				modal={ true }
				blockScroll={ true }
				onHide={ () => this.props.closeDialog() }
				>
				<Button 
					id="popup-dialog-close-button"
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

import { LogInFormShowAction } from "../../store/contexts/ui/logInForm/actions";
import { SetAccountAction } from "../../store/contexts/data/account/actions";

let mapStateToProps = (state: any): ILogInDialogMapProps => ({
	isLoggedIn: state.data.account != null,
	isDialogVisible: state.ui.logInForm.visible
})

let mapDispatchToProps = (dispatch: any): ILogInDialogDispatchMapProps => ({
	setAccount: (token:string, userId: string, login: string) => dispatch(new SetAccountAction(token, userId, login)),
	closeDialog: () => dispatch(new LogInFormShowAction(false))
})

let LogInDialogHOC = connect<ILogInDialogMapProps, ILogInDialogDispatchMapProps>(mapStateToProps, mapDispatchToProps)(LogInDialog);

export { LogInDialogHOC as LogInDialog };