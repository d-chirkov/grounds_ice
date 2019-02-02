import React from "react";
import { connect } from "react-redux";

import { IProfileInfo } from "../ProfileView";
import { IRootState } from "../../../contexts/model";

import { LoginEditView } from "./children/LoginEditView";
import { PasswordEditView } from "./children/PasswordEditView";
import { ProfileInfoEditView } from "./children/ProfileInfoEditView";
import { Link } from "react-router-dom";

interface IProfileEditViewMapProps {
}

interface IProfileEditViewMapDispatch {
}

interface IProfileEditViewProps extends IProfileEditViewMapProps {
	userId: string,
	profileInfo: IProfileInfo
	updateLocalProfileInfo: (newValue: IProfileInfo) => void
}

interface IProfileEditViewState {
	editableProfileInfo: IProfileInfo
}

class ProfileEditView extends React.Component<IProfileEditViewProps, IProfileEditViewState> {
	constructor(props: IProfileEditViewProps) {
		super(props);
	}
	
	render() {
		let {profileInfo, updateLocalProfileInfo, userId} = this.props;
		return (<div>
			<Link to={`/profile/id${userId}`} style={{float:"right", position:"relative", bottom:7, right:7}}>
				Назад
			</Link>
			<LoginEditView />
			<hr />
			<PasswordEditView />
			<hr />
			<ProfileInfoEditView profileInfo={profileInfo} updateLocalProfileInfo={updateLocalProfileInfo} />
		</div>);
	}
}


let mapStateToProps = (state: IRootState): IProfileEditViewMapProps => ({
	login: state.data.account != null ? state.data.account.login : ""
})

let mapDispatchToProps = (dispatch: any): IProfileEditViewMapDispatch => ({
})

let ProfileEditViewHOC = connect(mapStateToProps, mapDispatchToProps)(ProfileEditView);

export { ProfileEditViewHOC as ProfileEditView };