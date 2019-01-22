import { Action } from "redux";

const actionToPlainObject = (store: any) => (next: any) => (action: Action) => {

	if (isObjectLike(action)) {
		console.log(action);
		return next({ ...action })
	}
}

function isObjectLike(val: any): val is {} {
	return isPresent(val) && typeof val === 'object'
}
function isPresent(obj: any) {
	return obj !== undefined && obj !== null
}

export default actionToPlainObject;