import fetch from "isomorphic-fetch";
import { BaseController, ApiException } from "../BaseController"
import { Account } from "./Model"
import { serverAddress } from "../urls";
import { Value } from "../DTO/Value";
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

export class AccountController extends BaseController {
	constructor(token: string | null = null) {
		super(token);
	}
	
	protected IsTypeIsPossible(type: number): boolean {
		return Object.values(ValueType).includes(type);
	}
	
	// Remote API functions
	
	public Register(dto: DTO.LoginAndPassword): Promise<Value> {
		return this.Interact(registerUrl, dto);
	}
	
	public GetAccount(): Promise<Value<Account>> {
		return this.Interact<Account>(getAccountUrl).then(value => this.checkPayloadNotNull(value));
	}
	
	public ChangeLogin(dto: DTO.NewLogin): Promise<Value> {
		return this.Interact(changeLoginUrl, dto);
	}
	
	public ChangePassword(dto: DTO.OldAndNewPasswords): Promise<Value> {
		return this.Interact(changePasswordUrl, dto);
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
					throw <ApiException>{isUnauthorized: true};
				}
				return res;})
			.then(res => this.checkHttpStatus(res).json())
			.then(res => {
				if (!res.hasOwnProperty("access_token")) {
					throw <ApiException>{isUnauthorized: true};
				}
				this.token = res.access_token;
				return res.access_token;
			});
	}
}



