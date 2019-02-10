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
	let isEmptyProfileInfo = profileInfo.length == 0;
	return (
		<div>
			{isOwnProfileInfo && 
				<Link to={`/profile/id${userId}/edit`} style={{float:"right", position:"relative", top:2, right:7}} >
					Изменить
				</Link>
			}
			{isEmptyProfileInfo && <div className="w3-text-grey" style={{textAlign:"center", width:"100%", float:"left", height:"30px"}}>
				Здесь пока ничего нет
			</div>}
			<ProfileInfoEntryView fieldName="Фамилия" value={getValueByType(profileInfo, Model.ProfileInfoType.LastName)} />
			<ProfileInfoEntryView fieldName="Имя" value={getValueByType(profileInfo, Model.ProfileInfoType.FirstName)} />
			<ProfileInfoEntryView fieldName="Отчество" value={getValueByType(profileInfo, Model.ProfileInfoType.MiddleName)} />
			<ProfileInfoEntryView fieldName="Город" value={getValueByType(profileInfo, Model.ProfileInfoType.City)} />
			<ProfileInfoEntryView fieldName="Регион" value={getValueByType(profileInfo, Model.ProfileInfoType.Region)} />
			<ProfileInfoEntryView fieldName="Описание" value={getValueByType(profileInfo, Model.ProfileInfoType.Description)} autoHeight={true} />
			<div className="w3-text-blue-grey" style={{width:"250px", float:"left", height:"30px"}} />
		</div>
	);
}

function getValueByType(entries: Model.ProfileInfoEntry[], type: Model.ProfileInfoType): string | null {
	let entry = entries.find(v => v.Type == type);
	return isUndefined(entry) ? null : entry.Value
}

interface IProfileInfoEntryViewProps {
	fieldName: string
	value: string | null
	autoHeight?: boolean
}

let ProfileInfoEntryView = (props: IProfileInfoEntryViewProps, ) => {
	let {fieldName, value, autoHeight} = props;
	if (value == null) {
		return null;
	}
	if (isUndefined(autoHeight)) {
		autoHeight = false;
	}
	// TODO: убрать костыль
	let heightParam = autoHeight ? "auto" : "30px";
	return (<div>
		<div className="w3-text-blue-grey" style={{width:"120px", float:"left", height:"30px"}}>
			{`${fieldName}:`}
		</div>
		<div style={{float: "left", height:heightParam, width:"280px", wordWrap:"break-word"}}>
			{value}
		</div>
	</div>);
}