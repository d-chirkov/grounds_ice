import React from "react";
import { connect } from "react-redux";
import { IRootState } from "../../../../contexts/model";
import { IProfileInfo, IProfileInfoEntry } from "../../ProfileView";
import { InputText } from "primereact/inputtext";
import { InputTextarea } from "primereact/inputtextarea";
import { Checkbox } from "primereact/checkbox";
import { Button } from "primereact/button";

interface IProfileInfoEditViewMapProps {
}

interface IProfileInfoEditViewMapDispatch {
}

interface IProfileInfoEditViewProps extends IProfileInfoEditViewMapProps, IProfileInfoEditViewMapDispatch {
	profileInfo: IProfileInfo
	updateLocalProfileInfo: (newValue: IProfileInfo) => void
}

interface IProfileInfoEditViewState {
	profileInfo: IProfileInfo
}

class ProfileInfoEditView extends React.Component<IProfileInfoEditViewProps, IProfileInfoEditViewState> {
	constructor(props: IProfileInfoEditViewProps) {
		super(props);
		this.state = { profileInfo: props.profileInfo };
	}
	
	saveProfileInfo() {
		this.props.updateLocalProfileInfo(this.state.profileInfo);
	}
	
	render() {
		let {profileInfo} = this.state;
		return (<div>
			<h4>Изменить информацию профиля</h4>
			<div className="w3-container">
				{this.inputField("Фамилия", profileInfo.surname, (v, p) => this.setState({profileInfo: {...profileInfo, surname: {value:v, public:p}}}))}
				{this.inputField("Имя", profileInfo.firstname, (v, p) => this.setState({profileInfo: {...profileInfo, firstname: {value:v, public:p}}}))}
				{this.inputField("Отчество", profileInfo.middlename, (v, p) => this.setState({profileInfo: {...profileInfo, middlename: {value:v, public:p}}}))}
				{this.inputField("Местоположение", profileInfo.location, (v, p) => this.setState({profileInfo: {...profileInfo, location: {value:v, public:p}}}))}
				{this.descriptionField()}
				<Button style={{float:"right"}} label="Сохранить" icon="pi pi-save" iconPos="left" onClick={() => this.saveProfileInfo()} />
			</div>
		</div>);
	}
	
	private inputField(fieldName: string, entry: IProfileInfoEntry|null, update: (val: string, pub: boolean) => void) {
		let isPublic: boolean = entry ? entry.public : false;
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
		let description = this.state.profileInfo.description;
		let isPublic: boolean = description ? description.public : false;
		let fieldName = "Описание";
		return (
		<div style={{paddingBottom:"14px"}}>
			<InputTextarea rows={5} cols={30} autoResize={true}
				onChange={(e) => 
					this.setState({profileInfo: {...this.state.profileInfo, description: 
					{ public: isPublic, value: e.currentTarget.value}}})} 
				placeholder={fieldName}
				value={description ? description.value : undefined}
					/>
			<Checkbox 
				inputId={fieldName}
				onChange={e => { description && description.value != "" && 
					this.setState({profileInfo: {...this.state.profileInfo, description: 
					{ public: !e.checked, value: description.value }}}) }} 
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