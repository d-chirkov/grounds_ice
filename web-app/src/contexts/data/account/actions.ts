import { Action } from "redux";
import { RootAction } from "../../actions";

export enum AccountActionType {
	ACCOUNT_SET = "ACCOUNT_SET",
	ACCOUNT_SET_TOKEN = "ACCOUNT_SET_TOKEN",
	ACCOUNT_SET_USER_ID = "ACCOUNT_SET_USER_ID",
	ACCOUNT_SET_USERNAME = "ACCOUNT_SET_USERNAME",
	ACCOUNT_UNSET = "ACCOUNT_UNSET",
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

export class UnsetAccountAction implements Action<AccountActionType.ACCOUNT_UNSET> {
	readonly type = AccountActionType.ACCOUNT_UNSET;
}

export type AccountAction = SetTokenAction | SetUserIdAction | SetUserNameAction | SetAccountAction | UnsetAccountAction;

export function isAccountAction(action: RootAction): action is AccountAction {
	return Object.values(AccountActionType).includes(action.type);
}