import IAccount from "./account/model";

export interface IDataState {
	account: IAccount | null
}

export let initialDataState: IDataState = {
	account: null
}
