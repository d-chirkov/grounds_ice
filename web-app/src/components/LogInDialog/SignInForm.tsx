import React from "react";
import { Button } from "primereact/button";
import { InputText } from "primereact/inputtext";
import { Password } from "primereact/password";


interface ISignInFormProps {
	loading: boolean,
	usernameError: string | null,
	passwordError: string | null,
	onSignInClick: () => void,
	switchToRegistration: () => void,
	updateUsernameInput: (username: string) => void,
	updatePasswordInput: (password: string) => void,
}

let SignInForm = (props: ISignInFormProps) => {
	return (<div className="w3-container w3-center">
		<h1 className="w3-opacity"><b>Вход</b></h1>
		<div className="w3-panel">
			<span>
				<InputText 
					id="SignInForm.Username" 
					type="text" 
					className={props.usernameError != null ? "p-error" : undefined}
					tooltip={props.usernameError != null ? props.usernameError : undefined}
					size={30} 
					onChange={(e) => { props.updateUsernameInput(e.currentTarget.value); }} 
					placeholder="Логин"/>
			</span>
		</div>
		<div className="w3-panel">
			<span>
				<Password 
					id="SignInForm.Password" 
					type="text" 
					className={props.passwordError != null ? "p-error" : undefined}
					tooltip={props.passwordError != null ? props.passwordError : undefined}
					size={30} 
					feedback={false}
					onChange={(e) => { props.updatePasswordInput(e.currentTarget.value); }} 
					placeholder="Пароль"/>
			</span>
		</div>
		<div className="w3-panel">
			{props.loading ? 
				<Button label="Вход..." icon="pi pi-spin pi-spinner" iconPos="left" disabled={true} /> :
				<Button label="Войти" icon="pi pi-sign-in" iconPos="left" onClick={props.onSignInClick} />
			}
		</div>
		<div className="w3-panel">
			<Button 
				label="Регистрация" className="p-button-secondary" 
				icon="pi pi-plus" iconPos="left" onClick={props.switchToRegistration} 
				disabled={props.loading}/>
		</div>
	</div>)
}

export default SignInForm;