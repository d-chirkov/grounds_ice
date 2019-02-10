import React from "react";
import { connect } from "react-redux";
import { IRootState } from "../../../../store/contexts/model";
import { Password } from "primereact/password";
import { IProfileEditWindowProps, saveButton } from "../ProfileEditView";
import { Messager } from "../../../../Messager";
import { filter } from "../../../../inputFilters/passwordFilter"
import * as ChangePassword from "../../../../api/Account/interface/changePassword";

interface IPasswordEditViewMapProps {
}

interface IPasswordEditViewMapDispatch {
}

interface IPasswordEditViewProps extends IPasswordEditViewMapProps, IPasswordEditViewMapDispatch, IProfileEditWindowProps {
}

interface IPasswordEditViewState {
	loading: boolean
	isOldPasswordInputIsInvalid: boolean
	isNewPasswordsInputIsInvalid: boolean
	oldPasswordInput: string,
	newPasswordInput: string,
	repeatNewPasswordInput: string
}

class PasswordEditView extends React.Component<IPasswordEditViewProps, IPasswordEditViewState> {
	constructor(props: IPasswordEditViewProps) {
		super(props);
		this.state = {
			loading: false,
			isOldPasswordInputIsInvalid: false,
			isNewPasswordsInputIsInvalid: false,
			oldPasswordInput: "",
			newPasswordInput: "",
			repeatNewPasswordInput: ""
		}
	}
	
	updatePassword() {
		let {oldPasswordInput, newPasswordInput, repeatNewPasswordInput} = this.state;
		let warnings: string[] = [];
		let isOldPasswordInputIsInvalid = false;
		let isNewPasswordsInputIsInvalid = false;
		if (oldPasswordInput.length === 0) {
			warnings = [...warnings, "Введите старый пароль"];
			isOldPasswordInputIsInvalid = true;
		}
		else if (newPasswordInput.length === 0) {
			warnings = [...warnings, "Введите новый пароль"];
			isNewPasswordsInputIsInvalid = true;
		}
		else if (repeatNewPasswordInput.length === 0) {
			warnings = [...warnings, "Повторите новый пароль"];
			isNewPasswordsInputIsInvalid = true;
		}
		else if (newPasswordInput != repeatNewPasswordInput) {
			warnings = [...warnings, "Пароли не совпадают"];
			isNewPasswordsInputIsInvalid = true;
		}
		if (isOldPasswordInputIsInvalid || isNewPasswordsInputIsInvalid) {
			Messager.showManyErrors(warnings.map(v => ({message: v})))
			this.setState({isOldPasswordInputIsInvalid, isNewPasswordsInputIsInvalid});
		} else {
			this.setState({loading: true});
			this.props.setEditable(false);
			ChangePassword.perform(oldPasswordInput, newPasswordInput, 
				() => {
					this.props.setEditable(true);
					this.props.onChangesSaved(); 
				},
				(error: ChangePassword.Error) => {
					switch(error) {
						case ChangePassword.Error.OldPasswordNotValid: 
							Messager.showError("Старый пароль неверен"); 
							isOldPasswordInputIsInvalid = true; 
							break;
						case ChangePassword.Error.PasswordNotValid: 
							Messager.showError("Новый пароль не может быть использован");
							isNewPasswordsInputIsInvalid = true; 
							break;
						case ChangePassword.Error.Unexpected:
							Messager.showError("Неизвестная ошибка");
							break;
					}
					this.setState({loading: false, isOldPasswordInputIsInvalid, isNewPasswordsInputIsInvalid});
					this.props.setEditable(true);
				});
		}
	}
	
	updateOldPasswordInput(newValue: string) {
		this.setState({ isOldPasswordInputIsInvalid: false, oldPasswordInput: filter(newValue) });
	}
	
	updateNewPasswordInput(newValue: string) {
		this.setState({ isNewPasswordsInputIsInvalid: false, newPasswordInput: filter(newValue) });
	}
	
	updateRepeatNewPasswordInput(newValue: string) {
		this.setState({ isNewPasswordsInputIsInvalid: false, repeatNewPasswordInput: filter(newValue) });
	}

	render() {
		return (<div>
			<h4>Изменить пароль</h4>
			<div className="w3-container" style={{paddingBottom:"14px"}}>
				<Password
					type="text"
					size={30}
					className={this.state.isOldPasswordInputIsInvalid ? "p-error" : undefined}
					feedback={false}
					disabled={!this.props.isEditable}
					onChange={(e) => this.updateOldPasswordInput(e.currentTarget.value)}
					value={this.state.oldPasswordInput}
					placeholder="Старый пароль" />
			</div>
			<div className="w3-container" style={{paddingBottom:"14px"}}>
				<Password
					type="text"
					size={30}
					className={this.state.isNewPasswordsInputIsInvalid ? "p-error" : undefined}
					feedback={false}
					disabled={!this.props.isEditable}
					onChange={(e) => this.updateNewPasswordInput(e.currentTarget.value) }
					value={this.state.newPasswordInput}
					placeholder="Новый пароль" />
			</div>
			<div className="w3-container">
				<Password
					type="text"
					size={30}
					className={this.state.isNewPasswordsInputIsInvalid ? "p-error" : undefined}
					feedback={false}
					disabled={!this.props.isEditable}
					onChange={(e) => this.updateRepeatNewPasswordInput(e.currentTarget.value) }
					value={this.state.repeatNewPasswordInput}
					placeholder="Повторите новый пароль" />
				{saveButton(this.state.loading, this.props.isEditable, () => this.updatePassword())}
			</div>
		</div>);
	}
}

let mapStateToProps = (state: IRootState): IPasswordEditViewMapProps => ({
})

let mapDispatchToProps = (dispatch: any): IPasswordEditViewMapDispatch => ({
})

let PasswordEditViewHOC = connect(mapStateToProps, mapDispatchToProps)(PasswordEditView);

export { PasswordEditViewHOC as PasswordEditView };