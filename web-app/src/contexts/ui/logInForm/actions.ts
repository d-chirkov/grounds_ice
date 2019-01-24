import { Action } from "redux";
import { RootAction } from "../../actions";

export enum LogInFormActionType {
	LOGIN_FORM_SHOW = "LOGIN_FORM_SHOW",
	LOGIN_SET_SERVER_ERROR = "LOGIN_FORM_SET_SERVER_ERROR",
}

export class LogInFormShowAction implements Action<LogInFormActionType.LOGIN_FORM_SHOW> {
	readonly type = LogInFormActionType.LOGIN_FORM_SHOW;
	constructor(public visible: boolean) {}
}

export class LogInFormSetServerErrorAction implements Action<LogInFormActionType.LOGIN_SET_SERVER_ERROR> {
	readonly type = LogInFormActionType.LOGIN_SET_SERVER_ERROR;
	constructor(public serverError: string | null) {}
}
	
export type LogInFormAction = LogInFormShowAction | LogInFormSetServerErrorAction;

export function isLogInFormAction(action: RootAction): action is LogInFormAction {
	return Object.values(LogInFormActionType).includes(action.type);
}