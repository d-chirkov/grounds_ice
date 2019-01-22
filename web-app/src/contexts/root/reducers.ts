import { IRootState, initialRootState } from "./model";
import { RootAction } from "./actions";
import { isAccountContextAction } from "../account/actions";

import updateAccount from "../account/reducers";

let updateRootState = (state: IRootState = initialRootState, action: RootAction): IRootState => {
	if (isAccountContextAction(action)) {
		return {
			...state,
			account: updateAccount(state.account, action)
		};
	}
	return state;
}

export default updateRootState;