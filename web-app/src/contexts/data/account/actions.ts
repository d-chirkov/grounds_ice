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

import { LogInFormSetServerErrorAction } from "../../ui/logInForm/actions";
import Messager from "../../../Messager";
import { processSignUpRequest, IAccountInfo } from "../../../api/register";

export let SignUpAction = (username: string, password: string) => 
	(dispatch: Function): void => {
		let onSuccess = (accountInfo: IAccountInfo) => {
			dispatch(new SetAccountAction(accountInfo.token, accountInfo.userId, accountInfo.username));
		}
		let onFail = (err: string) => {
			dispatch(new LogInFormSetServerErrorAction(err));
			Messager.showWarning("Ошибка", err);
		}
		processSignUpRequest(username, password, onSuccess, onFail);
	}
	
export let SignInAction = () => 
	(dispatch: Function): void => {
		setTimeout(() => {
			dispatch(new LogInFormSetServerErrorAction("Неверный логин или пароль"));
			Messager.showWarning("Ошибка", "Неверный логин или пароль");
		}, 1500);
		return;
		//TODO: send token request to api, recieve response, dispatch actions to update state
	}
	
export type AccountAction = SetTokenAction | SetUserIdAction | SetUserNameAction | SetAccountAction;

export function isAccountAction(action: RootAction): action is AccountAction {
	return Object.values(AccountActionType).includes(action.type);
}