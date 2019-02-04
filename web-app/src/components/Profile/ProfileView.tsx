import React from "react";
import { connect } from "react-redux";
import { Route, Switch, RouteComponentProps } from "react-router-dom";
import { IRootState } from "../../store/contexts/model";

import { ProfileInfoView } from "./ProfileInfoView";
import { ProfileEditView } from "./ProfileEdit/ProfileEditView";

import { Card } from "primereact/card";

import * as Model from "../../api/Profile/Model";
import * as GetProfile from "../../api/Profile/interface/getProfile";
import { loadavg } from "os";
import { ValueType } from "../../api/Profile/ProfileController";
import { Messager } from "../../Messager";

interface IProfileViewRouteProps {
	userId: string
}

interface IProfileViewMapProps {
	userId: string
	isOwnProfile: boolean
}

interface IProfileViewMapDispatch {
}

interface IProfileViewProps extends IProfileViewMapProps, IProfileViewMapDispatch, RouteComponentProps<IProfileViewRouteProps> {
}

interface IProfileViewState {
	loading: boolean
	profile: Model.Profile | null
}

class ProfileView extends React.Component<IProfileViewProps, IProfileViewState> {
	
	constructor(props: IProfileViewProps) {
		super(props);
		this.state = {
			loading: false,
			profile: null
		}
	}
	
	componentWillMount() {
		this.loadProfile();
	}
	
	private loadProfile() {
		this.setState({ loading: true });
		let { userId } = this.props;
		GetProfile.perform(userId, 
		(profile) => {
			console.log("loaded profile");
			console.log(profile);
			this.setState({ loading: false, profile });
		},
		(error) => {
			let showWarning = (message: string) =>  Messager.showError("Ошибка", message);
			switch(error) {
				case GetProfile.Error.ProfileNotExists: 
					showWarning("Профиль не найден");
					break;
				case GetProfile.Error.Unexpected: 
					showWarning("Неизвестная ошибка"); 
					break;
			}
			this.setState({ loading: false });
		})
	}
	
	private updateLocalProfileInfo(newValue: Model.ProfileInfoEntry[]) {
		let { profile } = this.state;
		console.log("updateLocalProfileInfo this state");
		console.log(profile);
		if (profile != null) {
			profile.ProfileInfo = newValue;
			console.log("updateLocalProfileInfo");
			console.log(newValue);
			//this.state.profile!.ProfileInfo.
			this.setState({ profile: {ProfileInfo: newValue, Login: profile.Login, Avatar: profile.Avatar} });
		}
	}
	
	render() {
		let { profile } = this.state;
		let { userId } = this.props;
		console.log("render");
		if (profile != null) {
			console.log(profile.ProfileInfo);
		}
		return (<div style={{margin: "auto", width:"850px", bottom:0, top:0}}>{
			this.state.loading ? <p>LOADING</p> :
			profile == null ? <p>ERROR</p> :
			
			<div>
				<h3 className="w3-text-blue-grey" style={{marginLeft:"25px"}}>{this.props.isOwnProfile ? 
					"МОЙ ПРОФИЛЬ" : 
					`ПРОФИЛЬ ${profile.Login}(id${userId})`}</h3>
				<Card style={{margin:"25px", marginTop:0, float:"left", width: "250px", height: "250px", left: 0}}>
					АВАТАРКА
				</Card>
				<Card style={{margin:"25px", marginTop:0, float:"left", width: "500px", right: 0}}>
					<Switch>
						{this.props.isOwnProfile && <Route 
							path={`/profile/id${userId}/edit`} 
							component={(p: any) => <ProfileEditView 
								userId={userId}
								profileInfo={[...profile!.ProfileInfo]}
								updateLocalProfileInfo={v => this.updateLocalProfileInfo(v)} />} 
						/>}
						<Route 
							path={`/profile/id${userId}`} 
							component={(p: any) => <ProfileInfoView {...p} 
								userId={userId}  
								profileInfo={[...profile!.ProfileInfo]} 
								isOwnProfileInfo={this.props.isOwnProfile} />} 
						/>
					</Switch>
				</Card>
			</div>
		}</div>);
	}
}

let mapStateToProps = (state: IRootState, ownProps: IProfileViewProps): IProfileViewMapProps => ({
	userId: ownProps.match.params.userId.toString(),
	isOwnProfile: state.data.account !== null && state.data.account.userId == ownProps.match.params.userId.toString(),
})

let mapDispatchToProps = (dispatch: any): IProfileViewMapDispatch => ({
})

let ProfileViewHOC = connect(mapStateToProps, mapDispatchToProps)(ProfileView);

export { ProfileViewHOC as ProfileView };