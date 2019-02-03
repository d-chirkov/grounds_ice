import React from "react";
import { connect } from "react-redux";
import { Route, Switch, RouteComponentProps } from "react-router-dom";
import { IRootState } from "../../store/contexts/model";

import { ProfileInfoView } from "./ProfileInfoView";
import { ProfileEditView } from "./ProfileEdit/ProfileEditView";

import { Card } from 'primereact/card';

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
	profile: IProfile | null
}

interface IProfile {
	login: string
	avatar: string | null
	profileInfo: IProfileInfo
}

export interface IProfileInfo {
	firstname: IProfileInfoEntry | null,
	surname: IProfileInfoEntry | null,
	middlename: IProfileInfoEntry | null,
	location: IProfileInfoEntry | null,
	description: IProfileInfoEntry | null,
}

export interface IProfileInfoEntry {
	value: string,
	public: boolean,
}

let initialProfileInfo: IProfileInfo = {
	firstname: null,
	surname: null,
	middlename: null,
	location:  null,
	description: null,
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
		setTimeout(() => {
			//TODO: loading profile from api service
			let profileInfo: IProfileInfo = {
				...initialProfileInfo,
				firstname: {value: "Иван", public: true},
				middlename: {value: "Иваныч", public: true},
				location: {value: "Светлый Путь Ленина", public: false}
			}
			this.setState({ loading: false, profile: {login: "ivan", avatar: null, profileInfo} });
		}, 1000);
	}
	
	private updateLocalProfileInfo(newValue: IProfileInfo) {
		if (this.state.profile != null) {
			for (var k in newValue) {
				if (newValue.hasOwnProperty(k) && (newValue as any)[k] != null && (newValue as any)[k].value == "") {
					(newValue as any)[k] = null;
				}
			}
			let newProfile = {...this.state.profile, profileInfo: newValue};
			this.setState({profile: newProfile});
		}
	}
	
	render() {
		let {profile} = this.state;
		let {userId} = this.props;
		return (<div style={{margin: "auto", width:"850px", bottom:0, top:0}}>{
			this.state.loading ? <p>LOADING</p> :
			profile == null ? <p>ERROR</p> :
			
			<div>
				<h3 className="w3-text-blue-grey" style={{marginLeft:"25px"}}>{this.props.isOwnProfile ? 
					"МОЙ ПРОФИЛЬ" : 
					`ПРОФИЛЬ ${profile.login}(id${userId})`}</h3>
				<Card style={{margin:"25px", marginTop:0, float:"left", width: "250px", height: "250px", left: 0}}>
					АВАТАРКА
				</Card>
				<Card style={{margin:"25px", marginTop:0, float:"left", width: "500px", right: 0}}>
					<Switch>
						{this.props.isOwnProfile && <Route 
							path={`/profile/id${userId}/edit`} 
							component={(p: any) => <ProfileEditView 
								userId={userId}
								profileInfo={profile!.profileInfo}
								updateLocalProfileInfo={(v: IProfileInfo) => this.updateLocalProfileInfo(v)} />} 
						/>}
						<Route 
							path={`/profile/id${userId}`} 
							component={(p: any) => <ProfileInfoView {...p} 
								userId={userId}  
								profileInfo={profile!.profileInfo} 
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