const saver = (store: { getState: () => void; }) => (next: (arg0: any) => void) => (action: any) => {
	let result = next(action);
	localStorage["redux_store"] = JSON.stringify(store.getState());
	return result; 
}

export { saver };