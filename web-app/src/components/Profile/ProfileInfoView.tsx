import React from "react";
import { IProfileInfo, IProfileInfoEntry } from "./ProfileView";
import { Link } from "react-router-dom";


interface IProfileInfoViewProps {
	userId: string
	isOwnProfileInfo: boolean
	profileInfo: IProfileInfo
}

export let ProfileInfoView = (props: IProfileInfoViewProps) => {
	let {userId, isOwnProfileInfo, profileInfo} = props;
	console.log(profileInfo);
	return (
		<div>
			{isOwnProfileInfo && 
				<Link to={`/profile/id${userId}/edit`} style={{float:"right", position:"relative", top:2, right:7}} >
					Изменить
				</Link>
			}
			{profileInfo.surname && <ProfileInfoEntryView fieldName="Фамилия" entry={profileInfo.surname} />}
			{profileInfo.firstname && <ProfileInfoEntryView fieldName="Имя" entry={profileInfo.firstname} />}
			{profileInfo.middlename && <ProfileInfoEntryView fieldName="Отчество" entry={profileInfo.middlename} />}
			{profileInfo.location && <ProfileInfoEntryView fieldName="Местоположение" entry={profileInfo.location} />}
			{profileInfo.description && <ProfileInfoEntryView fieldName="Описание" entry={profileInfo.description} />}
		</div>
	);
}

interface IProfileInfoEntryViewProps {
	fieldName: string
	entry: IProfileInfoEntry
}

let ProfileInfoEntryView = (props: IProfileInfoEntryViewProps) => {
	let {fieldName, entry} = props;
	return (<div>
		<div className="w3-text-blue-grey" style={{width:"150px", float:"left", height:"30px"}}>
			{`${fieldName}:`}
		</div>
		<div style={{float: "left", height:"30px", width:"250px", wordWrap:"break-word"}}>
			{entry.value}
		</div>
	</div>);
}