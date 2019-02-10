import React from "react";
import { connect } from "react-redux";
import { IRootState } from "../../../../store/contexts/model";
import * as Model from "../../../../api/Profile/Model"
import * as SetProfileInfo from "../../../../api/Profile/interface/setProfileInfo"
import { filter } from "../../../../inputFilters/profileInfoInputFilter"
import { InputText } from "primereact/inputtext";
import { InputTextarea } from "primereact/inputtextarea";
import { Checkbox } from "primereact/checkbox";
import { IProfileEditWindowProps, saveButton } from "../ProfileEditView";
import { isUndefined } from "util";
import { Messager } from "../../../../Messager";
import { LocationEdit } from "../../../Shared/LocationEdit"

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
			profileInfoInput: props.profileInfo,
		};
		
	}
	
	updateProfileInfo() {
		this.setState({loading: true});
		this.props.setEditable(false);
		let profileInfo: Model.ProfileInfoEntry[] = [];
		console.log(this.state.profileInfoInput);
		let profileInfoInput = this.state.profileInfoInput.map((v:Model.ProfileInfoEntry) => ({...v, Value: v.Value.trim()}));
		for (let i = 0; i < profileInfoInput.length; ++i) {
			if (profileInfoInput[i].Value.length > 0) {
				profileInfoInput[i].Value = profileInfoInput[i].Value.trim();
				profileInfo.push(profileInfoInput[i]);
			}
		}
		SetProfileInfo.perform(profileInfo,
		() => {
			this.setState({loading: false});
			this.props.setEditable(true);
			this.props.updateLocalProfileInfo(profileInfo);
			this.props.onChangesSaved(); 
		},
		(error) => {
			this.setState({loading: false});
			this.props.setEditable(true);
			switch(error) {
				case SetProfileInfo.Error.BadData: 
					 Messager.showError("Введены недопустимые данные");
					break;
				case SetProfileInfo.Error.Unexpected:
					 Messager.showError("Неизвестная ошибка");
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
					this.getFieldByType(Model.ProfileInfoType.LastName, profileInfoInput))}
				{this.inputField( 
					"Имя", 
					this.getFieldByType(Model.ProfileInfoType.FirstName, profileInfoInput))}
				{this.inputField( 
					"Отчество", 
					this.getFieldByType(Model.ProfileInfoType.MiddleName, profileInfoInput))}
				{this.locationField(
					this.getFieldByType(Model.ProfileInfoType.City, profileInfoInput),
					this.getFieldByType(Model.ProfileInfoType.Region, profileInfoInput))}
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
	
	private inputField(fieldName: string, entry: Model.ProfileInfoEntry) {
		let isPublic: boolean = entry ? entry.IsPublic : false;
		return (
		<div style={{paddingBottom:"14px"}}>
			<InputText 
				type="text" 
				size={30} 
				onChange={(e) => { entry.Value = filter(e.currentTarget.value, entry.Type); this.forceUpdate() }} 
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
				onChange={(e) => { entry.Value = filter(e.currentTarget.value, entry.Type); this.forceUpdate(); }}
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
	
	private locationField(cityEntry: Model.ProfileInfoEntry, regionEntry: Model.ProfileInfoEntry) {
		let fieldName = "Местоположение";
		let isCityIsPublic: boolean = (cityEntry.Value == "") || cityEntry.IsPublic;
		let isRegionIsPublic: boolean = (regionEntry.Value == "") || regionEntry.IsPublic;
		let isPublic = isCityIsPublic && isRegionIsPublic;
		if (cityEntry.Value == "" && regionEntry.Value == "") {
			isPublic = false;
		}
		let isEmpty = cityEntry.Value == "" && regionEntry.Value == "";
		let setPublic = (flag: boolean) => {cityEntry.IsPublic = flag; regionEntry.IsPublic = flag;}
		return (
		<div style={{paddingBottom:"14px"}}>
			<LocationEdit
				size={30} 
				placeholder={fieldName}
				intialCity={cityEntry.Value}
				intialRegion={regionEntry.Value}
				onChangeLocation={this.updateLocation.bind(this)}
			/>
			<Checkbox 
				inputId={fieldName}
				onChange={e => { if (!isEmpty) setPublic(!e.checked); this.forceUpdate() } } 
				checked={ !isPublic } 
				disabled={ isEmpty }
				style={{marginLeft:"20px"}}/>
			<label htmlFor={fieldName} className="p-checkbox-label w3-text-blue-grey">Скрыть</label>
		</div>
		);
	}
	
	private updateLocation(city: string | null, region: string | null) {
		this.getFieldByType(Model.ProfileInfoType.City, this.state.profileInfoInput).Value = city || "";
		this.getFieldByType(Model.ProfileInfoType.Region, this.state.profileInfoInput).Value = region || "";
		this.forceUpdate();
	}
}

let mapStateToProps = (state: IRootState): IProfileInfoEditViewMapProps => ({
})

let mapDispatchToProps = (dispatch: any): IProfileInfoEditViewMapDispatch => ({
})

let ProfileInfoEditViewHOC = connect(mapStateToProps, mapDispatchToProps)(ProfileInfoEditView);

export { ProfileInfoEditViewHOC as ProfileInfoEditView };