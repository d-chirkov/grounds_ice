import { IUIState, initialUIState } from "./model";
import { RootAction } from "../actions";

import { isRegistrationFromAction } from "./registrationForm/actions";
import updateRegistrationForm from "./registrationForm/reducers";

let updateUIState = (state: IUIState = initialUIState, action: RootAction): IUIState => {
	if (isRegistrationFromAction(action)) {
		return {
			...state,
			registrationForm: updateRegistrationForm(state.registrationForm, action)
		};
	}
	return { ...state };
}

export default updateUIState;