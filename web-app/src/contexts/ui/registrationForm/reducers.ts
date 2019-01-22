import { IRegistrationForm, initialRegistrationForm } from "./model"
import { RegistrationFromActionType, RegistrationFromAction } from "./actions";

let updateRegistrationForm = (state: IRegistrationForm = initialRegistrationForm, action: RegistrationFromAction): IRegistrationForm => {
	if (action.type == RegistrationFromActionType.REGISTRATION_FORM_SET_VISIBLE) {
		console.log("updating registration form");
		return {
			...state,
			visible: action.visible
		};
	}
	return {...state};
}

export default updateRegistrationForm;