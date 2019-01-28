import { Action } from "redux";
import { RootAction } from "../../actions";

export enum LogInFormActionType {
	LOGIN_FORM_SHOW = "LOGIN_FORM_SHOW",
}

export class LogInFormShowAction implements Action<LogInFormActionType.LOGIN_FORM_SHOW> {
	readonly type = LogInFormActionType.LOGIN_FORM_SHOW;
	constructor(public visible: boolean) {}
}

export type LogInFormAction = LogInFormShowAction;

export function isLogInFormAction(action: RootAction): action is LogInFormAction {
	return Object.values(LogInFormActionType).includes(action.type);
}