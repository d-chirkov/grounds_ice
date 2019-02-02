import { createStore, applyMiddleware } from "redux";
import thunk from "redux-thunk";
import { actionToPlainObject } from "./middlewares/actionToPlainObject";
import { saver } from "./middlewares/storeSaver";
import updateRootState from "./contexts/reducers";
import { initialRootState } from "./contexts/model";

let storeFactory = () => {
	return applyMiddleware(thunk, actionToPlainObject, saver)(createStore)(
		updateRootState, 
		localStorage["redux_store"] ? JSON.parse(localStorage["redux_store"]) : initialRootState);
}

export { storeFactory };
