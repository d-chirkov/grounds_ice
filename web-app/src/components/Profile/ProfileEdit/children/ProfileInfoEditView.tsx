import React from "react";
import { connect } from "react-redux";
import { IRootState } from "../../../../store/contexts/model";
import { IProfileInfo, IProfileInfoEntry } from "../../ProfileView";
import { InputText } from "primereact/inputtext";
import { InputTextarea } from "primereact/inputtextarea";
import { Checkbox } from "primereact/checkbox";
import { Button } from "primereact/button";
import { IProfileEditWindowProps, saveButton } from "../ProfileEditView";


interface IProfileInfoEditViewMapProps {
}

interface IProfileInfoEditViewMapDispatch {
}

interface IProfileInfoEditViewProps extends IProfileInfoEditViewMapProps, IProfileInfoEditViewMapDispatch, IProfileEditWindowProps {
	profileInfo: IProfileInfo
	updateLocalProfileInfo: (newValue: IProfileInfo) => void
}

interface IProfileInfoEditViewState {
	loading: boolean
	newProfileInfoInput: IProfileInfo
}

class ProfileInfoEditView extends React.Component<IProfileInfoEditViewProps, IProfileInfoEditViewState> {
	constructor(props: IProfileInfoEditViewProps) {
		super(props);
		this.state = { 
			loading: false,
			newProfileInfoInput: props.profileInfo 
		};
	}
	
	updateProfileInfo() {
		this.setState({loading: true});
		this.props.setEditable(false);
		// TODO: call api
		setTimeout(() => {
			this.props.updateLocalProfileInfo(this.state.newProfileInfoInput);
			this.setState({loading: false});
			this.props.setEditable(true);
			this.props.onChangesSaved(); 
		}, 500);
	}
	
	render() {
		let {newProfileInfoInput: profileInfo} = this.state;
		return (<div>
			<h4>Изменить информацию профиля</h4>
			<div className="w3-container">
				{this.inputField("Фамилия", profileInfo.surname, (v, p) => this.setState({newProfileInfoInput: {...profileInfo, surname: {value:v, isPublic:p}}}))}
				{this.inputField("Имя", profileInfo.firstname, (v, p) => this.setState({newProfileInfoInput: {...profileInfo, firstname: {value:v, isPublic:p}}}))}
				{this.inputField("Отчество", profileInfo.middlename, (v, p) => this.setState({newProfileInfoInput: {...profileInfo, middlename: {value:v, isPublic:p}}}))}
				{this.inputField("Местоположение", profileInfo.location, (v, p) => this.setState({newProfileInfoInput: {...profileInfo, location: {value:v, isPublic:p}}}))}
				{this.descriptionField()}
				{saveButton(this.state.loading, this.props.isEditable, () => this.updateProfileInfo())}
			</div>
		</div>);
	}
	
	private inputField(fieldName: string, entry: IProfileInfoEntry|null, update: (val: string, pub: boolean) => void) {
		let isPublic: boolean = entry ? entry.isPublic : false;
		return (
		<div style={{paddingBottom:"14px"}}>
			<InputText 
				type="text" 
				size={30} 
				onChange={(e) => update(e.currentTarget.value, isPublic)} 
				placeholder={fieldName}
				value={entry ? entry.value : undefined}
					/>
			<Checkbox 
				inputId={fieldName}
				onChange={e => { entry && entry.value != "" && update(entry.value, !e.checked) } } 
				checked={ !isPublic } 
				disabled={ entry == null || entry.value == ""}
				style={{marginLeft:"20px"}}/>
			<label htmlFor={fieldName} className="p-checkbox-label w3-text-blue-grey">Скрыть</label>
		</div>
		);
	}
	
	private descriptionField() {
		let description = this.state.newProfileInfoInput.description;
		let isPublic: boolean = description ? description.isPublic : false;
		let fieldName = "Описание";
		return (
		<div style={{paddingBottom:"14px"}}>
			<InputTextarea rows={5} cols={30} autoResize={true}
				onChange={(e) => 
					this.setState({newProfileInfoInput: {...this.state.newProfileInfoInput, description: 
					{ isPublic: isPublic, value: e.currentTarget.value}}})} 
				placeholder={fieldName}
				value={description ? description.value : undefined}
					/>
			<Checkbox 
				inputId={fieldName}
				onChange={e => { description && description.value != "" && 
					this.setState({newProfileInfoInput: {...this.state.newProfileInfoInput, description: 
					{ isPublic: !e.checked, value: description.value }}}) }} 
				checked={ !isPublic } 
				disabled={ description == null || description.value == ""}
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