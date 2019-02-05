import React from "react";
import { connect } from "react-redux";
import { IRootState } from "../../../../store/contexts/model";
import * as Model from "../../../../api/Profile/Model"
import * as SetProfileInfo from "../../../../api/Profile/interface/setProfileInfo"
import { InputText } from "primereact/inputtext";
import { InputTextarea } from "primereact/inputtextarea";
import { Checkbox } from "primereact/checkbox";
import { IProfileEditWindowProps, saveButton } from "../ProfileEditView";
import { isUndefined } from "util";
import { Messager } from "../../../../Messager";


interface IProfileInfoEditViewMapProps {
}

interface IProfileInfoEditViewMapDispatch {
}

interface IProfileInfoEditViewProps extends IProfileInfoEditViewMapProps, IProfileInfoEditViewMapDispatch, IProfileEditWindowProps {
	profileInfo: Array<Model.ProfileInfoEntry>
	updateLocalProfileInfo: (newValue: Array<Model.ProfileInfoEntry>) => void
}

interface IProfileInfoEditViewState {
	loading: boolean
	profileInfoInput: Array<Model.ProfileInfoEntry>
}

class ProfileInfoEditView extends React.Component<IProfileInfoEditViewProps, IProfileInfoEditViewState> {
	constructor(props: IProfileInfoEditViewProps) {
		super(props);
		this.state = { 
			loading: false,
			profileInfoInput: props.profileInfo 
		};
	}
	
	updateProfileInfo() {
		this.setState({loading: true});
		this.props.setEditable(false);
		let profileInfo: Model.ProfileInfoEntry[] = [];
		let {profileInfoInput} = this.state;
		console.log(profileInfoInput);
		for (let i = 0; i < profileInfoInput.length; ++i) {
			console.log(profileInfoInput[i].Value.length);
			if (profileInfoInput[i].Value.length > 0) {
				profileInfo.push(profileInfoInput[i]);
			}
		}
		let cpy = [...profileInfo];
		//let profileInfo = [...this.state.profileInfoInput.filter(v => v.Value.length > 0)];
		console.log(profileInfo);
		SetProfileInfo.perform(profileInfo,
		() => {
			this.setState({loading: false});
			this.props.setEditable(true);
			this.props.updateLocalProfileInfo(cpy);
			this.props.onChangesSaved(); 
		},
		(error) => {
			this.setState({loading: false});
			this.props.setEditable(true);
			let showWarning = (message: string) =>  Messager.showError("Ошибка редактирования профиля", message);
			switch(error) {
				case SetProfileInfo.Error.BadData: 
					showWarning("Введены недопустимые данные");
					break;
				case SetProfileInfo.Error.Unexpected:
					showWarning("Неизвестная ошибка");
					break;
			}
		});
	}
	
	render() {
		let {profileInfoInput} = this.state;
		return (<div>
			<h4>Изменить информацию профиля</h4>
			<div className="w3-container">
				{}
				{this.inputField( 
					"Фамилия", 
					this.getFieldByType(Model.ProfileInfoType.LastName, profileInfoInput),
					(v, p) => this.setState({}))}
				{this.inputField( 
					"Имя", 
					this.getFieldByType(Model.ProfileInfoType.FirstName, profileInfoInput),
					(v, p) => this.setState({}))}
				{this.inputField( 
					"Отчество", 
					this.getFieldByType(Model.ProfileInfoType.MiddleName, profileInfoInput),
					(v, p) => this.setState({}))}
				{this.inputField( 
					"Местоположение", 
					this.getFieldByType(Model.ProfileInfoType.Location, profileInfoInput),
					(v, p) => this.setState({}))}
				{this.descriptionField(this.getFieldByType(Model.ProfileInfoType.Description, profileInfoInput))}
				{saveButton(this.state.loading, this.props.isEditable, () => this.updateProfileInfo())}
			</div>
		</div>);
	}
	
	private getFieldByType(fieldType: Model.ProfileInfoType, profileInfo: Array<Model.ProfileInfoEntry>): Model.ProfileInfoEntry {
		let found = profileInfo.find((v) => v.Type == fieldType);
		if (isUndefined(found)) {
			found = {Type: fieldType, Value: "", IsPublic: false};
			profileInfo.push(found);
		} 
		return found;
	}
	
	private inputField(fieldName: string, entry: Model.ProfileInfoEntry, update: (val: string, pub: boolean) => void) {
		let isPublic: boolean = entry ? entry.IsPublic : false;
		return (
		<div style={{paddingBottom:"14px"}}>
			<InputText 
				type="text" 
				size={30} 
				onChange={(e) => { entry.Value = e.currentTarget.value; this.forceUpdate() }} 
				placeholder={fieldName}
				value={entry.Value}
					/>
			<Checkbox 
				inputId={fieldName}
				onChange={e => { if (entry && entry.Value != "") entry.IsPublic = !e.checked; this.forceUpdate() } } 
				checked={ !isPublic } 
				disabled={ entry.Value == ""}
				style={{marginLeft:"20px"}}/>
			<label htmlFor={fieldName} className="p-checkbox-label w3-text-blue-grey">Скрыть</label>
		</div>
		);
	}
	
	private descriptionField(entry: Model.ProfileInfoEntry) {
		let fieldName = "Описание";
		return (
		<div style={{paddingBottom:"14px"}}>
			<InputTextarea rows={5} cols={30} autoResize={true}
				onChange={(e) => { entry.Value = e.currentTarget.value; this.forceUpdate(); }}
				placeholder={fieldName}
				value={entry.Value}
					/>
			<Checkbox 
				inputId={fieldName}
				onChange={e => { entry.IsPublic = !e.checked; this.forceUpdate() }} 
				checked={ !entry.IsPublic } 
				disabled={ entry.Value == ""}
				style={{marginLeft:"20px"}}/>
			<label htmlFor={fieldName} className="p-checkbox-label w3-text-blue-grey">Скрыть</label>
		</div>);
	}
}

let mapStateToProps = (state: IRootState): IProfileInfoEditViewMapProps => ({
})

let mapDispatchToProps = (dispatch: any): IProfileInfoEditViewMapDispatch => ({
})

let ProfileInfoEditViewHOC = connect(mapStateToProps, mapDispatchToProps)(ProfileInfoEditView);

export { ProfileInfoEditViewHOC as ProfileInfoEditView };