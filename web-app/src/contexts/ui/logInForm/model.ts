export interface ILogInForm {
	visible: boolean
	serverError: string | null
}

export let initialLogInForm = {
	visible: false,
	serverError: null
}