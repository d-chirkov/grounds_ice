import "../ProfileEditView.css"

import React from "react";
import { connect } from "react-redux";
import { IRootState } from "../../../../store/contexts/model";
import { InputText } from "primereact/inputtext";
import { IProfileEditWindowProps, saveButton } from "../ProfileEditView";
import * as ChangeLogin from "../../../../api/Account/interface/changeLogin";
import { filter } from "../../../../inputFilters/loginFilter"
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
	loginMaxLength: number
}

class LoginEditView extends React.Component<ILoginEditViewProps, ILoginEditViewState> {
	constructor(props: ILoginEditViewProps) {
		super(props);
		this.state = {
			loading: false,
			newLoginInput: "",
			isLoginInputIsInvalid: false,
			loginMaxLength: 20
		}
	}
	
	render() {
		return (<div>
			<h4>Изменить логин</h4>
			<div className="w3-container">
				<div className="gi-profile-edit-row">
					<InputText 
						className={"gi-input-text " + (this.state.isLoginInputIsInvalid ? "p-error" : "")}
						disabled={!this.props.isEditable}
						onChange={(e) => this.updateLoginInput(e.currentTarget.value)}
						placeholder="Новый логин"
						type="text" 
						value={this.state.newLoginInput} />
					{saveButton(this.state.loading, this.props.isEditable, () => this.updateLogin())}
				</div>
			</div>
		</div>);
	}
	
	updateLogin() {
		let {newLoginInput} = this.state;
		let warnings: string[] = [];
		let isInvalidInput = false;
		if (newLoginInput.length === 0) {
			warnings = [...warnings, "Логин не может быть пустым"];
			isInvalidInput = true;
		}
		if (isInvalidInput) {
			Messager.showManyErrors(warnings.map(v => ({message: v})))
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
					switch(error) {
						case ChangeLogin.Error.LoginAlreadyExists: 
							Messager.showError("Логин уже используется"); 
							isInvalidInput = true; 
							break;
						case ChangeLogin.Error.LoginNotValid: 
							Messager.showError("Этот логин не может быть использован");
							isInvalidInput = true; 
							break;
						case ChangeLogin.Error.Unexpected:
							Messager.showError("Неизвестная ошибка");
							break;
					}
					this.setState({loading: false, isLoginInputIsInvalid: true});
					this.props.setEditable(true);
				});
		}
	}
	
	updateLoginInput(newValue: string) {
		this.setState({newLoginInput: filter(newValue), isLoginInputIsInvalid: false})
	}
}


let mapStateToProps = (state: IRootState): ILoginEditViewMapProps => ({
})

let mapDispatchToProps = (dispatch: any): ILoginEditViewMapDispatch => ({
	setLogin: (v: string) => dispatch(new SetLoginAction(v))
})

let LoginEditViewHOC = connect(mapStateToProps, mapDispatchToProps)(LoginEditView);

export { LoginEditViewHOC as LoginEditView };