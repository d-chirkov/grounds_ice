import * as Model from "../api/Profile/Model"

let entriesMaxLengths = {
	[Model.ProfileInfoType.FirstName]: 30,
	[Model.ProfileInfoType.LastName]: 30,
	[Model.ProfileInfoType.MiddleName]: 30,
	[Model.ProfileInfoType.City]: 35,
	[Model.ProfileInfoType.Region]: 35,
	[Model.ProfileInfoType.Description]: 300,
}

export function filter(value: string, type: Model.ProfileInfoType): string {
	if (!/\S/.test(value)) {
		return "";
	}
	if (value.length > entriesMaxLengths[type]) {
		return value.substr(0, entriesMaxLengths[type]);	
	}
	return value;
}