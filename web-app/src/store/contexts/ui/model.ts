import { ILogInForm, initialLogInForm } from "./logInForm/model";
import { ICreateOrderForm, initialCreateOrderForm } from "./createOrderForm/model";

export interface IUIState {
	logInForm: ILogInForm
	createOrderForm: ICreateOrderForm
}

export let initialUIState: IUIState = {
	logInForm: initialLogInForm,
	createOrderForm: initialCreateOrderForm
}
