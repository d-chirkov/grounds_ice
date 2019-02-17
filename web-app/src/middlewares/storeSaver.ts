import { IRootState } from "../store/contexts/model";

const saver = (store: { getState: () => IRootState; }) => (next: (arg0: any) => void) => (action: any) => {
	let result = next(action);
	localStorage["account"] = JSON.stringify((store.getState()).data.account)
	return result; 
}

export { saver };