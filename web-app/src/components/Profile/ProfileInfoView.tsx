import React from "react";
import { Link } from "react-router-dom";
import * as Model from "../../api/Profile/Model"
import { isUndefined } from "util";


interface IProfileInfoViewProps {
	userId: string
	isOwnProfileInfo: boolean
	profileInfo: Model.ProfileInfoEntry[]
}

export let ProfileInfoView = (props: IProfileInfoViewProps) => {
	let {userId, isOwnProfileInfo, profileInfo} = props;
	console.log("ProfileInfoView");
	console.log(profileInfo);
	return (
		<div>
			{isOwnProfileInfo && 
				<Link to={`/profile/id${userId}/edit`} style={{float:"right", position:"relative", top:2, right:7}} >
					Изменить
				</Link>
			}
			<ProfileInfoEntryView fieldName="Фамилия" entry={profileInfo.find(v => v.Type == Model.ProfileInfoType.LastName)} />
			<ProfileInfoEntryView fieldName="Имя" entry={profileInfo.find(v => v.Type == Model.ProfileInfoType.FirstName)} />
			<ProfileInfoEntryView fieldName="Отчество" entry={profileInfo.find(v => v.Type == Model.ProfileInfoType.MiddleName)} />
			<ProfileInfoEntryView fieldName="Местоположение" entry={profileInfo.find(v => v.Type == Model.ProfileInfoType.Location)} />
			<ProfileInfoEntryView fieldName="Описание" entry={profileInfo.find(v => v.Type == Model.ProfileInfoType.Description)} />
		</div>
	);
}

interface IProfileInfoEntryViewProps {
	fieldName: string
	entry?: Model.ProfileInfoEntry
}

let ProfileInfoEntryView = (props: IProfileInfoEntryViewProps) => {
	let {fieldName, entry} = props;
	if (isUndefined(entry)) {
		return null;
	}
	return (<div>
		<div className="w3-text-blue-grey" style={{width:"150px", float:"left", height:"30px"}}>
			{`${fieldName}:`}
		</div>
		<div style={{float: "left", height:"30px", width:"250px", wordWrap:"break-word"}}>
			{entry.Value}
		</div>
	</div>);
}