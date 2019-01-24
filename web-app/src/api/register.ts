import fetch from "isomorphic-fetch";
import { accountGetInfoUrl, accountRegisterUrl, tokenUrl } from "./urls";

let askForAccountInfo = (token: string): Promise<Response> => {
	let request: RequestInit = {
		method: "Post",
		headers: {
			"Accept": "application/json",
			"Authorization": "Bearer " + token
		}
	}; 
	return fetch(accountGetInfoUrl, request);
}

let askForToken = (username: string, password: string): Promise<Response> => {
	interface TokenRequest {
		grant_type: string,
		username: string,
		password: string,
		client_id: string
	}
	let toketRequest: TokenRequest = {
		grant_type: "password",
		username: username,
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
	return fetch(tokenUrl, request);
}
	
let errorCodesMap = {
	[1000]: "Success",
	[2000]: "Unspecified",
	[3000]: "CredentialsNotValid",
	[4000]: "UserAlreadyExists",
}

export enum ErrorCode {
	Success = 1000,
	Unspecified = 2000,
	CredentialsNotValid = 3000, 
	UserAlreadyExists = 4000
}
	
export interface IAccountInfo {
	token: string,
	userId: string,
	username: string
}

export let registerAccount = (
	username: string, 
	password: string, 
	onSuccess: (accountInfo: IAccountInfo) => void, 
	onFail: (errorCode: ErrorCode) => void) => 
	{
	let registerRequest: RequestInit = {
		method: "Post",
		headers: {
			"Accept": "application/json",
			"Content-Type": "application/json"
		},
		body: JSON.stringify({username: username, password: password})
	}
	let token: string = "";
	fetch(accountRegisterUrl, registerRequest)
		.then((res) => res.json())
		.then((res) => askForToken(username, password))
		.then((res) => res.json())
		.then((res) => {token = res.access_token; return askForAccountInfo(token) })
		.then((res) => res.json())
		.then((res) => {console.log(res); onSuccess({
			token: token,
			userId: res.Id,
			username: res.Name})})
		.catch((reason) => {
			let errorCode: ErrorCode = ErrorCode.Unspecified;
			console.log(reason);
			if (reason.json().ErrorCode != undefined) {
				console.log("WTF!!!");
				errorCode = ErrorCode.Unspecified;
			}
			console.log(errorCode);
			onFail(errorCode);
		}).catch(() => onFail(ErrorCode.Unspecified));
}





