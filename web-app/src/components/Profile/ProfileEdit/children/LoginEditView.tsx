import React from "react";
import { connect } from "react-redux";
import { IRootState } from "../../../../store/contexts/model";
import { InputText } from "primereact/inputtext";
import { IProfileEditWindowProps, saveButton } from "../ProfileEditView";
import * as ChangeLogin from "../../../../api/Account/interface/changeLogin";
import { Messager } from "../../../../Messager";
import { SetLoginAction } from "../../../../store/contexts/data/account/actions";

interface ILoginEditViewMapProps {
}

interface ILoginEditViewMapDispatch {
	setLogin: (v: string) => void
}

interface ILoginEditViewProps extends ILoginEditViewMapProps, ILoginEditViewMapDispatch, IProfileEditWindowProps {
}

interface ILoginEditViewState {
	loading: boolean
	newLoginInput: string
	isLoginInputIsInvalid: boolean
}

class LoginEditView extends React.Component<ILoginEditViewProps, ILoginEditViewState> {
	constructor(props: ILoginEditViewProps) {
		super(props);
		this.state = {
			loading: false,
			newLoginInput: "",
			isLoginInputIsInvalid: false
		}
	}
	
	updateLogin() {
		let {newLoginInput} = this.state;
		let errorMessageHeader = "Ошибка смены логина";
		let warnings: string[] = [];
		let isInvalidInput = false;
		if (newLoginInput.length === 0) {
			warnings = [...warnings, "Логин не может быть пустым"];
			isInvalidInput = true;
		}
		if (isInvalidInput) {
			Messager.showErrors(warnings.map(v => ({header: errorMessageHeader, message: v})))
			this.setState({isLoginInputIsInvalid: true});
			
		} else {
			this.setState({loading: true});
			this.props.setEditable(false);
			ChangeLogin.perform(newLoginInput, 
				() => {
					this.props.setLogin(newLoginInput);
					this.props.setEditable(true);
					this.props.onChangesSaved(); 
				},
				(error: ChangeLogin.Error) => {
					let showWarning = (message: string) =>  Messager.showError(errorMessageHeader, message);
					switch(error) {
						case ChangeLogin.Error.LoginAlreadyExists: 
							showWarning("Логин уже используется"); 
							isInvalidInput = true; 
							break;
						case ChangeLogin.Error.LoginNotValid: 
							showWarning("Этот логин не может быть использован");
							isInvalidInput = true; 
							break;
						case ChangeLogin.Error.Unexpected:
							showWarning("Неизвестная ошибка");
							break;
					}
					this.setState({loading: false, isLoginInputIsInvalid: true});
					this.props.setEditable(true);
				});
		}
	}
	
	updateLoginInput(newValue: string) {
		this.setState({newLoginInput: newValue, isLoginInputIsInvalid: false})
	}
	
	render() {
		return (<div>
			<h4>Изменить логин</h4>
			<div className="w3-container">
				<InputText 
					type="text" 
					size={30} 
					placeholder="Новый логин"
					className={this.state.isLoginInputIsInvalid ? "p-error" : undefined}
					disabled={!this.props.isEditable}
					onChange={(e) => { this.setState({newLoginInput: e.currentTarget.value}) }} 
					/>
				{saveButton(this.state.loading, this.props.isEditable, () => this.updateLogin())}
			</div>
		</div>);
	}
}


let mapStateToProps = (state: IRootState): ILoginEditViewMapProps => ({
})

let mapDispatchToProps = (dispatch: any): ILoginEditViewMapDispatch => ({
	setLogin: (v: string) => dispatch(new SetLoginAction(v))
})

let LoginEditViewHOC = connect(mapStateToProps, mapDispatchToProps)(LoginEditView);

export { LoginEditViewHOC as LoginEditView };