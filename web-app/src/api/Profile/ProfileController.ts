import { BaseController } from "../BaseController"
import { serverAddress } from "../urls";
import { Value } from "../DTO/Value";
import * as DTO from "./DTO";
import * as Model from "./Model";

let getProfileUrl = serverAddress + "api/profile/get";
let setProfileInfoUrl = serverAddress + "api/profile/set_profile_info";

export enum ValueType {
	Success = 1000,
	ProfileNotExists = 2000,
	BadData = 3000,
}

export class ProfileController extends BaseController {
	
	constructor(token: string | null = null) {
		super(token);
	}
	
	protected IsTypeIsPossible(type: number): boolean {
		return Object.values(ValueType).includes(type);
	}
	
	// Remote API functions
	
	public Get(dto: DTO.ProfileRequest): Promise<Value<Model.Profile>> {
		return this.Interact<Model.Profile>(getProfileUrl, dto).then(value => this.checkPayloadNotNull(value));;
	}
	
	public SetProfileInfo(dto: DTO.ProfileInfoModel): Promise<Value> {
		return this.Interact(setProfileInfoUrl, dto);
	}
	
	
}