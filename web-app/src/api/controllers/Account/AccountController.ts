import fetch from "isomorphic-fetch";
import { serverAddress } from "../../urls";
import { Value, getInitialValue } from "../../DTO/Value";
import * as DTO from "./DTO";

let fetTokenUrl = serverAddress + "token";
let registerUrl = serverAddress + "api/account/register";
let getAccountUrl = serverAddress + "api/account/get_account";
let changeLoginUrl = serverAddress + "api/account/change_login";
let changePasswordUrl = serverAddress + "api/account/change_password";

export enum ValueType {
	Success = 1000,
	LoginAlreadyExists = 2000,
	LoginNotValid = 4001,
	PasswordNotValid = 4002,
	AccountNotExists = 5000,
	OldPasswordNotValid = 6000,
}

interface AccountException {
	isUnauthorized?: boolean
	isUnexpected?: boolean
	valueType?: ValueType
}

interface Account {
	UserId: string,
	Login: string
}

export class AccountController {
	
	constructor(token: string | null = null) {
		this.token = token;
		console.log(token);
	}
	
	public token: string | null = null;
	
	// Remote API functions
	
	public Register(dto: DTO.LoginAndPassword): Promise<Value> {
		return this.Interact(registerUrl, false, dto);
	}
	
	public GetAccount(): Promise<Value<Account>> {
		return this.Interact<Account>(getAccountUrl, true).then(value => this.checkPayloadNotNull(value));
	}
	
	public ChangeLogin(dto: DTO.NewLogin): Promise<Value> {
		return this.Interact(changeLoginUrl, true, dto);
	}
	
	public ChangePassword(dto: DTO.OldAndNewPasswords): Promise<Value> {
		return this.Interact(changePasswordUrl, true, dto);
	}
	
	private Interact<T = null>(url: string, authorized: boolean, dto: any = null): Promise<Value<T>> {
		if (authorized && this.token == null) {
			throw <AccountException>{isUnexpected: true};
		}
		let request: RequestInit = {
			method: "Post",
			headers: {
				"Accept": "application/json",
				"Content-Type": "application/json",
			},
			body: JSON.stringify(dto)
		}; 
		if (authorized) {
			request.headers = {...request.headers, "Authorization": "Bearer " + this.token};
		}
		if (dto != null) {
			request.body = JSON.stringify(dto);
		}
		return fetch(url, request)
			.then(res => this.getValueFrom<T>(res))
			.then(value => this.checkValueSuccess(value));
	}
	
	// GetToken not provided by account controller, but by ASP.NET OAuth provider with bearer tokens
	public GetToken(login: string, password: string): Promise<string> {
		interface TokenRequest {
			grant_type: string,
			username: string,
			password: string,
			client_id: string
		}
		let toketRequest: TokenRequest = {
			grant_type: "password",
			username: login,
			password: password,
			client_id: "self"
		}
		let requestKeys: Array<string> = new Array<string>();
		for (let i in toketRequest) {
			requestKeys.push(encodeURIComponent(i) + "=" + encodeURIComponent(toketRequest[i as keyof TokenRequest]));
		}
		let request: RequestInit = {
			method: "Post",
			headers: {
				"Accept": "application/json",
				"Content-Type": "application/x-www-form-urlencoded"
			},
			body: requestKeys.join("&")
		};
		return fetch(fetTokenUrl, request)
			.then(res => {
				if (res.status == 400) {
					throw <AccountException>{isUnauthorized: true};
				}
				return res;})
			.then(res => this.checkHttpStatus(res).json())
			.then(res => {
				if (!res.hasOwnProperty("access_token")) {
					throw <AccountException>{isUnauthorized: true};
				}
				this.token = res.access_token;
				return res.access_token;
			});
	}
	
	// Helpers
	
	private checkValueSuccess<T = null>(value: Value<T>): Value<T> {
		if (value.Type != ValueType.Success) {
			throw <AccountException>{valueType: value.Type};
		}
		return value;
	}
	
	private checkPayloadNotNull<T = null>(value: Value<T>): Value<T> {
		if (value.Payload == null) {
			throw <AccountException>{isUnexpected: true};
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
					throw <AccountException>{isUnexpected: true};
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
			throw <AccountException>{isUnauthorized: true};
		}
		throw <AccountException>{isUnexpected: true};
	}
}



