import { IUIState, initialUIState } from "./model";
import { RootAction } from "../actions";

import { isLogInFormAction } from "./logInForm/actions";
import { updateLogInForm } from "./logInForm/reducers";

import { isCreateOrderFormAction } from "./createOrderForm/actions";
import { updateCreateOrderForm } from "./createOrderForm/reducers";

let updateUIState = (state: IUIState = initialUIState, action: RootAction): IUIState => {
	if (isLogInFormAction(action)) {
		return {
			...state,
			logInForm: updateLogInForm(state.logInForm, action)
		};
	}
	if (isCreateOrderFormAction(action)) {
		return {
			...state,
			createOrderForm: updateCreateOrderForm(state.createOrderForm, action)
		};
	}
	return state;
}

export default updateUIState;