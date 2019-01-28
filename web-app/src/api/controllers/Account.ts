import fetch from "isomorphic-fetch";
import { serverAddress } from "../urls";

export let fetTokenUrl = serverAddress + "token";
export let registerUrl = serverAddress + "api/account/register";
export let getAccountUrl = serverAddress + "api/account/get_account";

export enum ValueType {
	Success = 1000,
	LoginAlreadyExists = 2000,
	LoginNotValid = 4001,
	PasswordNotValid = 4002,
	AccountNotExists = 5000,
}

interface AccountException {
	isUnauthorized?: boolean,
	isUnexpected?: boolean,
	valueType?: ValueType
}

interface LoginAndPassword {
	Login: string | null,
	Password: string | null
}

interface Account {
	UserId: string,
	Login: string
}

interface Value {
	Type: ValueType, 
	Payload: Account | null
}

let initialValue: Value = {
	Type: 0,
	Payload: null
}

export class AccountController {
	
	public token: string | null = null;
	
	// Remote API functions
	
	public Register(dto: LoginAndPassword): Promise<Value> {
		let request: RequestInit = {
			method: "Post",
			headers: {
				"Accept": "application/json",
				"Content-Type": "application/json"
			},
			body: JSON.stringify(dto)
		}
		return fetch(registerUrl, request)
			.then(res => this.getValueFrom(res))
			.then(value => this.checkValueSuccess(value));
	}
	
	public GetAccount(): Promise<Value> {
		if (this.token == null) {
			throw <AccountException>{isUnexpected: true};
		}
		let request: RequestInit = {
			method: "Post",
			headers: {
				"Accept": "application/json",
				"Authorization": "Bearer " + this.token
			}
		}; 
		return fetch(getAccountUrl, request)
			.then(res => this.getValueFrom(res))
			.then(value => this.checkValueSuccess(value))
			.then(value => this.checkPayloadNotNull(value));
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
	
	private checkValueSuccess(value: Value): Value {
		if (value.Type != ValueType.Success) {
			throw <AccountException>{valueType: value.Type};
		}
		return value;
	}
	
	private checkPayloadNotNull(value: Value): Value {
		if (value.Payload == null) {
			throw <AccountException>{isUnexpected: true};
		}
		return value;
	}
	
	private getValueFrom(res: Response): Promise<Value> {
		return this.checkHttpStatus(res).json()
			.then((res:any) => {
				let value: Value = initialValue;
				if (res.hasOwnProperty("Type") && Object.values(ValueType).includes(res.Type)) {
					value.Type = res.Type;
				} else {
					throw <AccountException>{isUnexpected: true};
				}
				if (res.hasOwnProperty("Payload")) {
					value.Payload = res.Payload;
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



