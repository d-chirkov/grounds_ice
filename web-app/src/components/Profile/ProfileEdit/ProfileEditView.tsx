import React from "react";
import { connect } from "react-redux";

import { IProfileInfo } from "../ProfileView";
import { IRootState } from "../../../store/contexts/model";

import { LoginEditView } from "./children/LoginEditView";
import { PasswordEditView } from "./children/PasswordEditView";
import { ProfileInfoEditView } from "./children/ProfileInfoEditView";
import { Link, withRouter, RouteComponentProps } from "react-router-dom";

import { Button } from "primereact/button";

export interface IProfileEditWindowProps {
	onChangesSaved: () => void
	setEditable: (status: boolean) => void
	isEditable: boolean
} 

export function saveButton(loading: boolean, enabled: boolean, callback: () => void) {
	return (<Button 
		style={{float:"right"}} 
		label="Сохранить"
		icon={loading ? "pi pi-spin pi-spinner" : "pi pi-save"}
		iconPos="left" 
		disabled={!enabled}
		onClick={() => callback()}/>);
}

interface IProfileEditViewMapProps {
}

interface IProfileEditViewMapDispatch {
}

interface IProfileEditViewProps extends IProfileEditViewMapProps, RouteComponentProps {
	userId: string,
	profileInfo: IProfileInfo
	updateLocalProfileInfo: (newValue: IProfileInfo) => void
}

interface IProfileEditViewState {
	isEditable: boolean
}

class ProfileEditView extends React.Component<IProfileEditViewProps, IProfileEditViewState> {
	constructor(props: IProfileEditViewProps) {
		super(props);
		this.state = {
			isEditable: true
		}
	}
	
	setEditable(status: boolean) {
		this.setState({isEditable: status});
	}
	
	onChangesSaved() {
		this.props.history.push(`/profile/id${this.props.userId}`);
	}
	
	render() {
		let {isEditable} = this.state;
		let {profileInfo, updateLocalProfileInfo, userId} = this.props;
		let profileWindowProps: IProfileEditWindowProps = {
			isEditable: this.state.isEditable,
			setEditable: v => this.setEditable(v),
			onChangesSaved: () => this.onChangesSaved()
		}
		return (<div>
			<Link to={`/profile/id${userId}`} style={{float:"right", position:"relative", bottom:7, right:7}}>
				Назад
			</Link>
			<LoginEditView {...profileWindowProps}/>
			<hr />
			<PasswordEditView {...profileWindowProps}/>
			<hr />
			<ProfileInfoEditView {...profileWindowProps} profileInfo={profileInfo} updateLocalProfileInfo={updateLocalProfileInfo}/>
		</div>);
	}
}


let mapStateToProps = (state: IRootState): IProfileEditViewMapProps => ({
	login: state.data.account != null ? state.data.account.login : ""
})

let mapDispatchToProps = (dispatch: any): IProfileEditViewMapDispatch => ({
})

let ProfileEditViewHOC = withRouter(connect(mapStateToProps, mapDispatchToProps)(ProfileEditView));

export { ProfileEditViewHOC as ProfileEditView };