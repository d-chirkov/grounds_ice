import { IDataState, initialDataState } from "./model";
import { RootAction } from "../actions";

import { isAccountAction } from "./account/actions";
import updateAccount from "./account/reducers";

let updateDataState = (state: IDataState = initialDataState, action: RootAction): IDataState => {
	if (isAccountAction(action)) {
		return {
			...state,
			account: updateAccount(state.account, action)
		};
	}
	return { ...state };
}

export default updateDataState;