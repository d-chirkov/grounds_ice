import fetch from "isomorphic-fetch";
import { accountGetInfoUrl, accountRegisterUrl, tokenUrl } from "./urls";

let fetchAccountInfo = (token: string): Promise<Response> => {
	let request: RequestInit = {
		method: "Post",
		headers: {
			"Accept": "application/json",
			"Authorization": "Bearer " + token
		}
	}; 
	return fetch(accountGetInfoUrl, request);
}

let fetchToken = (username: string, password: string): Promise<Response> => {
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

interface FetchingException {
	isUnauthorized?: boolean,
	isUnexpected?: boolean,
	errorCode?: ErrorCode
}

let checkHttpStatus = (response: Response): any => {
	let status = response.status;
	if (status == 200) {
		return response;
	}
	if (status == 401) {
		throw <FetchingException>{isUnauthorized: true};
	}
	throw <FetchingException>{isUnexpected: true};
}

let checkErrorCode = <T>(response: T): T => {
	if (!response.hasOwnProperty("ErrorCode")) {
		throw <FetchingException>{isUnexpected: true};
	}
	let errorCode = (<any>response).ErrorCode;
	if(!Object.values(ErrorCode).includes(errorCode)) {
		throw <FetchingException>{isUnexpected: true};
	}
	if (errorCode != ErrorCode.Success) {
		throw <FetchingException>{errorCode};
	}
	return response;
}

let selectJsonFrom = (response: Response): any => {
	return checkHttpStatus(response).json().then((res:any) => checkErrorCode(res))
}

let selectTokenFrom = (response: Response): any => {
	return checkHttpStatus(response).json()
		.then((res:any) => {
			if (!res.hasOwnProperty("access_token")) {
				throw <FetchingException>{isUnauthorized: true};
			}
			return res.access_token;
		})
}

export let registerAccount = (
	username: string, 
	password: string, 
	onSuccess: (accountInfo: IAccountInfo) => void, 
	onFail: (errorDescription: string) => void) => 
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
		.then(res => selectJsonFrom(res))
		.then(res => fetchToken(username, password))
		.then(res => selectTokenFrom(res))
		.then(res => {token = res; return fetchAccountInfo(token) })
		.then(res => selectJsonFrom(res))
		.then(res => {console.log(res); onSuccess({
			token: token,
			userId: res.Id,
			username: res.Name})})
		.catch((reason) => {
			if ("isUnauthorized" in reason && reason.isUnauthorized) {
				onFail("Не удалось авторизоваться на сервере, попробуйте позже");
			}
			else if ("isUnexpected" in reason && reason.isUnexpected) {
				onFail("Что-то пошло не так, попробуйте повторить попытку позже");
			}
			else if ("errorCode" in reason) {
				let errorCode:ErrorCode = reason.errorCode;
				let err: string = "";
				switch(errorCode) {
					case ErrorCode.CredentialsNotValid: err = "Логин или пароль не прошли валидацию, попробуйте другие учётные данные"; break;
					case ErrorCode.UserAlreadyExists: err = "Пользователь с таким логином уже существует, попробуйте другой"; break;
					default: err = "Что-то пошло не так, попробуйте повторить попытку позже";
				}
				onFail(err);
			}
			else {
				onFail("Что-то пошло не так, попробуйте повторить попытку позже");
			}
		});
}





