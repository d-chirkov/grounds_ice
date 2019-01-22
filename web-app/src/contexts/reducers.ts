import { IRootState, initialRootState } from "./model";
import { RootAction } from "./actions";
import updateDataState from "./data/reducers";
import updateUIState from "./ui/reducers";

let updateRootState = (state: IRootState = initialRootState, action: RootAction): IRootState => {
	return {
		...state,
		data: updateDataState(state.data, action),
		ui: updateUIState(state.ui, action)
	};
}

export default updateRootState;