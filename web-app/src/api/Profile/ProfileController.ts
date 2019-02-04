import fetch from "isomorphic-fetch";
import { serverAddress } from "../urls";
import { Value, getInitialValue } from "../DTO/Value";
import * as DTO from "./DTO";
import * as Model from "./Model";

let getProfileUrl = serverAddress + "api/profile/get";
let setProfileInfoUrl = serverAddress + "api/profile/set_profile_info";

export enum ValueType {
	Success = 1000,
	ProfileNotExists = 2000,
	BadData = 3000,
}

interface ProfileException {
	isUnauthorized?: boolean
	isUnexpected?: boolean
	valueType?: ValueType
}

export class ProfileController {
	
	constructor(token: string | null = null) {
		this.token = token;
	}
	
	public token: string | null = null;
	
	// Remote API functions
	
	public Get(dto: DTO.ProfileRequest): Promise<Value<Model.Profile>> {
		return this.Interact<Model.Profile>(getProfileUrl, dto).then(value => this.checkPayloadNotNull(value));;
	}
	
	public SetProfileInfo(dto: DTO.ProfileInfoModel): Promise<Value> {
		return this.Interact(setProfileInfoUrl, dto);
	}
	
	private Interact<T = null>(url: string, dto: any = null): Promise<Value<T>> {
		let request: RequestInit = {
			method: "Post",
			headers: {
				"Accept": "application/json",
				"Content-Type": "application/json; charset=utf-8",
			},
		}; 
		if (this.token != null) {
			request.headers = {...request.headers, "Authorization": "Bearer " + this.token};
		}
		if (dto != null) {
			request.body = JSON.stringify(dto);
		}
		console.log(dto);
		console.log(request.body);
		return fetch(url, request)
			.then(res => this.getValueFrom<T>(res))
			.then(value => this.checkValueSuccess(value));
	}
	
	// Helpers
	
	private checkValueSuccess<T = null>(value: Value<T>): Value<T> {
		if (value.Type != ValueType.Success) {
			throw <ProfileException>{valueType: value.Type};
		}
		return value;
	}
	
	private checkPayloadNotNull<T = null>(value: Value<T>): Value<T> {
		if (value.Payload == null) {
			throw <ProfileException>{isUnexpected: true};
		}
		return value;
	}
	
	private getValueFrom<T = null>(res: Response): Promise<Value<T>> {
		return this.checkHttpStatus(res).json()
			.then((res:any) => {
				let value = getInitialValue<T>();
				if (res.hasOwnProperty("Type") && Object.values(ValueType).includes(res.Type)) {
					value.Type = res.Type;
				} else {
					throw <ProfileException>{isUnexpected: true};
				}
				if (res.hasOwnProperty("Payload")) {
					value.Payload = res.Payload as T;
				}
				return value;
			})
	}
	
	private checkHttpStatus (response: Response): any  {
		let status = response.status;
		if (status == 200) {
			return response;
		}
		if (status == 401) {
			throw <ProfileException>{isUnauthorized: true};
		}
		throw <ProfileException>{isUnexpected: true};
	}
}