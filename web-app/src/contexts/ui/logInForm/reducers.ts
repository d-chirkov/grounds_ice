import { ILogInForm, initialLogInForm } from "./model"
import { LogInFormActionType, LogInFormAction } from "./actions";

let updateLogInForm = (state: ILogInForm = initialLogInForm, action: LogInFormAction): ILogInForm => {
	if (action.type == LogInFormActionType.LOGIN_FORM_SHOW) {
		return {
			...state,
			visible: action.visible
		};
	}
	if (action.type == LogInFormActionType.LOGIN_SET_SERVER_ERROR) {
		return {
			...state,
			serverError: action.serverError
		};
	}
	return {...state};
}

export default updateLogInForm;