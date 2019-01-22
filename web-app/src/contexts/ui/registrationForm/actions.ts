import { Action } from "redux";
import { RootAction } from "../../actions";

export enum RegistrationFromActionType {
	REGISTRATION_FORM_SET_VISIBLE = "REGISTRATION_FORM_SET_VISIBLE",
}

export class SetRegistrationFormVisibleAction implements Action<RegistrationFromActionType.REGISTRATION_FORM_SET_VISIBLE> {
	readonly type = RegistrationFromActionType.REGISTRATION_FORM_SET_VISIBLE;
	constructor(public visible: boolean) {}
}
	
export type RegistrationFromAction = SetRegistrationFormVisibleAction;

export function isRegistrationFromAction(action: RootAction): action is RegistrationFromAction {
	return Object.values(RegistrationFromActionType).includes(action.type);
}