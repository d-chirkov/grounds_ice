import React from "react";
import { connect } from "react-redux";
import { Route, Switch, RouteComponentProps } from "react-router-dom";
import { IRootState } from "../../contexts/model";

import { ProfileInfoView } from "./ProfileInfoView";
import { ProfileEditView } from "./ProfileEditView";

import { Card } from 'primereact/card';

interface IProfileViewRouteProps {
	userId: string
}

interface IProfileViewMapProps {
	isOwnProfile: boolean
}

interface IProfileViewMapDispatch {
}

interface IProfileViewProps extends IProfileViewMapProps, IProfileViewMapDispatch, RouteComponentProps<IProfileViewRouteProps> {
}

interface IProfileViewState {
	isOwnProfile: boolean
	userId: string
	loading: boolean
	profile: IProfile | null
}

interface IProfile {
	login: string
	avatar: string | null
	profileInfo: IProfileInfo
}

export interface IProfileInfo {
	firstname: string | null,
	surname: string | null,
	middlename: string | null,
	location: string | null,
	description: string | null,
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
			isOwnProfile: props.isOwnProfile,
			userId: props.match.params.userId.toString(),
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
				firstname: "Иван",
				middlename: "Иваныч",
				location: "Светлый Путь Ленина"
			}
			this.setState({ loading: false, profile: {login: "ivan", avatar: null, profileInfo} });
		}, 1000);
	}
	
	render() {
		let {profile} = this.state;
		return (<div className="w3-container" style={{margin: "auto", width:"800px", height:"100vh"}}>{
			this.state.loading ? <p>LOADING</p> :
			profile == null ? <p>ERROR</p> :
			<div style={{position: "relative"}}>
				<h3 className="w3-text-blue-grey">{this.props.isOwnProfile ? 
					"МОЙ ПРОФИЛЬ" : 
					`ПРОФИЛЬ ${profile.login}(id${this.state.userId})`}</h3>
				<Card style={{position: "absolute", width: "250px", height: "250px", left: 0}}>
					АВАТАРКА
				</Card>
				<Card style={{position: "absolute", width: "500px", right: 0}}>
					<Switch>
						<Route 
							exact path={`/profile/id${this.state.userId}`} 
							component={(p: any) => <ProfileInfoView {...p} 
								profileInfo={profile!.profileInfo} 
								isOwnProfileInfo={this.props.isOwnProfile} 
								userId={this.state.userId} />} 
						/>
						<Route 
							exact path={`/profile/id${this.state.userId}/edit`} 
							component={(p: any) => <ProfileEditView/>} 
						/>
					</Switch>
				</Card>
			</div>
		}</div>);
	}
}

//<Route exact path={`/profile/${this.state.userId}/edit`} component={} />
// to={`/profile/id${this.state.userId}/edit`}

let mapStateToProps = (state: IRootState, ownProps: IProfileViewProps): IProfileViewMapProps => ({
	isOwnProfile: state.data.account !== null && state.data.account.userId == ownProps.match.params.userId.toString(),
})

let mapDispatchToProps = (dispatch: any): IProfileViewMapDispatch => ({
})

let ProfileViewHOC = connect(mapStateToProps, mapDispatchToProps)(ProfileView);

export { ProfileViewHOC as ProfileView };