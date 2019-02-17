import { createStore, applyMiddleware } from "redux";
import thunk from "redux-thunk";
import { actionToPlainObject } from "../middlewares/actionToPlainObject";
import { saver } from "../middlewares/storeSaver";
import updateRootState from "./contexts/reducers";
import { initialRootState } from "./contexts/model";

let storeFactory = () => {
	let state = initialRootState;
	state.data.account = localStorage["account"] ? JSON.parse(localStorage["account"]) : null;
	return applyMiddleware(thunk, actionToPlainObject, saver)(createStore)(
		updateRootState, state);//, 
}

export { storeFactory };
