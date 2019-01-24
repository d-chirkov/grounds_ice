import { IUIState, initialUIState } from "./model";
import { RootAction } from "../actions";

import { isLogInFormAction } from "./logInForm/actions";
import updateLogInForm from "./logInForm/reducers";

let updateUIState = (state: IUIState = initialUIState, action: RootAction): IUIState => {
	if (isLogInFormAction(action)) {
		return {
			...state,
			logInForm: updateLogInForm(state.logInForm, action)
		};
	}
	return state;
}

export default updateUIState;