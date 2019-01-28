import React from "react";
import { Button } from "primereact/button";
import { InputText } from "primereact/inputtext";
import { Password } from "primereact/password";


interface ISignUpFormProps {
	loading: boolean,
	isLoginIsInvalid: boolean,
	isPasswordIsInvalid: boolean,
	onSignUpClick: () => void,
	updateLoginInput: (login: string) => void,
	updatePasswordInput: (password: string) => void,
	updatePasswordRepeatInput: (passwordRepeat: string) => void,
}

let SignUpForm = (props: ISignUpFormProps) => {
	return (<div className="w3-container w3-center">
		<h1 className="w3-opacity"><b>Регистрация</b></h1>
		<div className="w3-panel">
			<span>
				<InputText 
					type="text" 
					className={props.isLoginIsInvalid ? "p-error" : undefined}
					size={30} 
					onChange={(e) => { props.updateLoginInput(e.currentTarget.value); }} 
					placeholder="Логин"/>
			</span>
		</div>
		<div className="w3-panel">
			<span>
				<Password 
					type="text" 
					className={props.isPasswordIsInvalid ? "p-error" : undefined}
					size={30} 
					feedback={false}
					onChange={(e) => { props.updatePasswordInput(e.currentTarget.value); }} 
					placeholder="Пароль"/>
			</span>
		</div>
		
		<div className="w3-panel">
			<span>
				<Password 
					type="text" 
					className={props.isPasswordIsInvalid ? "p-error" : undefined}
					size={30} 
					feedback={false}
					onChange={(e) => { props.updatePasswordRepeatInput(e.currentTarget.value); }} 
					placeholder="Повторите пароль"/>
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