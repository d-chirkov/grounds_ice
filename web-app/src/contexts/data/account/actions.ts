import { Action } from "redux";
import { RootAction } from "../../actions";

export enum AccountActionType {
	ACCOUNT_SET = "ACCOUNT_SET",
	ACCOUNT_SET_TOKEN = "ACCOUNT_SET_TOKEN",
	ACCOUNT_SET_USER_ID = "ACCOUNT_SET_USER_ID",
	ACCOUNT_SET_USERNAME = "ACCOUNT_SET_USERNAME"
}

export class SetTokenAction implements Action<AccountActionType.ACCOUNT_SET_TOKEN> {
	readonly type = AccountActionType.ACCOUNT_SET_TOKEN;
	constructor(public token: string) {}
}

export class SetUserIdAction implements Action<AccountActionType.ACCOUNT_SET_USER_ID> {
	readonly type = AccountActionType.ACCOUNT_SET_USER_ID;
	constructor(public userId: string) {}
}

export class SetUserNameAction implements Action<AccountActionType.ACCOUNT_SET_USERNAME> {
	readonly type = AccountActionType.ACCOUNT_SET_USERNAME;
	constructor(public userName: string) {}
}

export class SetAccountAction implements Action<AccountActionType.ACCOUNT_SET> {
	readonly type = AccountActionType.ACCOUNT_SET;
	constructor(public token: string, public userId: string, public userName: string) {}
}

import { LogInFormShowAction, LogInFormSetServerErrorAction } from "../../ui/logInForm/actions";
import Messager from "../../../Messager";
import { processSignUpRequest, processSignInRequest, IAccountInfo } from "../../../api/register";

function processSignInUpRequest (
	username: string, 
	password: string, 
	handler: (username:string, 
			  password: string, 
			  onSuccess: (accountInfo: IAccountInfo) => void, 
			  onFail: (err: string) => void) => void) 
	{
	return (dispatch: Function): void => {
		let onSuccess = (accountInfo: IAccountInfo) => {
			dispatch(new SetAccountAction(accountInfo.token, accountInfo.userId, accountInfo.username));
		}
		let onFail = (err: string) => {
			dispatch(new LogInFormSetServerErrorAction(err));
			Messager.showWarning("Ошибка", err);
		}
		handler(username, password, onSuccess, onFail);
	}		  
}
	

export let SignUpAction = (username: string, password: string) => processSignInUpRequest(username, password, processSignUpRequest);
	
export let SignInAction = (username: string, password: string) => processSignInUpRequest(username, password, processSignInRequest);

export type AccountAction = SetTokenAction | SetUserIdAction | SetUserNameAction | SetAccountAction;

export function isAccountAction(action: RootAction): action is AccountAction {
	return Object.values(AccountActionType).includes(action.type);
}