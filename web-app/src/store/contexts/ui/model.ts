import { ILogInForm, initialLogInForm } from "./logInForm/model";

export interface IUIState {
	logInForm: ILogInForm
}

export let initialUIState: IUIState = {
	logInForm: initialLogInForm
}
