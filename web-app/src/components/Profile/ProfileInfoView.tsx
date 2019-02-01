import React from "react";
import { IProfileInfo } from "./ProfileView";
import { Link } from "react-router-dom";


interface IProfileInfoViewProps {
	userId: string
	isOwnProfileInfo: boolean
	profileInfo: IProfileInfo
}

export let ProfileInfoView = (props: IProfileInfoViewProps) => {
	let {userId, isOwnProfileInfo, profileInfo} = props;
	return (
		<div>
			{isOwnProfileInfo && 
				<Link to={`/profile/id${userId}/edit`} style={{position:"absolute", top:5, right:15}}>
					Изменить
				</Link>
			}
			{profileInfo.surname && <ProfileInfoEntryView field="Фамилия" value={profileInfo.surname} />}
			{profileInfo.firstname && <ProfileInfoEntryView field="Имя" value={profileInfo.firstname} />}
			{profileInfo.middlename && <ProfileInfoEntryView field="Отчество" value={profileInfo.middlename} />}
			{profileInfo.location && <ProfileInfoEntryView field="Местоположение" value={profileInfo.location} />}
		</div>
	);
}

interface IProfileInfoEntryViewProps {
	field: string
	value: string
}

let ProfileInfoEntryView = (props: IProfileInfoEntryViewProps) => {
	let {field, value} = props;
	return (<div>
		<div style={{width:"150px", float:"left", height:"30px"}}>
			{`${field}:`}
		</div>
		<div style={{height:"30px"}}>
			{value}
		</div>
	</div>);
}