import { store } from "../store";

export function selectToken() : string | null {
	let state = store.getState();
	return state.data.account == null ? null : state.data.account.token;
}