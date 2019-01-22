import OIAccount from "../account/model";

export interface IRootState {
	account: OIAccount
}

export let initialRootState: IRootState = {
	account: null
}
