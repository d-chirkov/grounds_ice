import { IRegistrationForm, initialRegistrationForm} from "./registrationForm/model";

export interface IUIState {
	registrationForm: IRegistrationForm
}

export let initialUIState: IUIState = {
	registrationForm: initialRegistrationForm
}
