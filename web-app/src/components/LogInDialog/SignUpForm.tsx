import React from "react";
import { Button } from "primereact/button";
import { InputText } from "primereact/inputtext";
import { Password } from "primereact/password";


interface ISignUpFormProps {
	loading: boolean,
	usernameError: string | null,
	passwordError: string | null,
	onSignUpClick: () => void,
	updateUsernameInput: (username: string) => void,
	updatePasswordInput: (password: string) => void,
	updatePasswordRepeatInput: (password: string) => void,
}

let SignUpForm = (props: ISignUpFormProps) => {
	return (<div className="w3-container w3-center">
		<h1 className="w3-opacity"><b>Регистрация</b></h1>
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
					tooltip={props.passwordError ? props.passwordError : undefined}
					size={30} 
					feedback={false}
					onChange={(e) => { props.updatePasswordInput(e.currentTarget.value); }} 
					placeholder="Пароль"/>
			</span>
		</div>
		
		<div className="w3-panel">
			<span>
				<Password 
					id="SignInForm.Password" 
					type="text" 
					className={props.passwordError ? "p-error" : undefined}
					size={30} 
					feedback={false}
					onChange={(e) => { props.updatePasswordRepeatInput(e.currentTarget.value); }} 
					placeholder="Повторите пароль"
					value={props.passwordError != null ? "" : undefined}/>
			</span>
		</div>
		<div className="w3-panel">
			{props.loading ? 
				<Button label="Регистрация..." icon="pi pi-spin pi-spinner" iconPos="left" disabled={true} /> :
				<Button label="Зарегистрироваться" icon="pi pi-sign-in" iconPos="left" onClick={props.onSignUpClick} />
			}
		</div>
	</div>)
}

export default SignUpForm;