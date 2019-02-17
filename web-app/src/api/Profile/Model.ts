export enum ProfileInfoType {
	FirstName = "FirstName",
	LastName = "LastName",
	MiddleName = "MiddleName",
	City = "City",
	Region = "Region",
	Description = "Description"
}

export interface ProfileInfoEntry {
	Type: ProfileInfoType
	Value: string
	IsPublic: boolean
}

export interface Profile {
	Login: string
	Avatar: string | null
	ProfileInfo: ProfileInfoEntry[]
}