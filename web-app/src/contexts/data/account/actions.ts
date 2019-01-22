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

export let RegisterAccountAction = (username: string, password: string) => 
	(dispatch: Function): void => {
		return;
		//TODO: send register request to api, recieve response, dispatch actions to update state
	}
	
export let LogInAction = (username: string, password: string) => 
	(dispatch: Function): void => {
		return;
		//TODO: send token request to api, recieve response, dispatch actions to update state
	}
	
export type AccountAction = SetTokenAction | SetUserIdAction | SetUserNameAction | SetAccountAction;

export function isAccountAction(action: RootAction): action is AccountAction {
	return Object.values(AccountActionType).includes(action.type);
}