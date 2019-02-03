import { IDataState, initialDataState } from "../contexts/data/model";
import { IUIState, initialUIState } from "../contexts/ui/model";

export interface IRootState {
	data: IDataState,
	ui: IUIState
}

export let initialRootState: IRootState = {
	data: initialDataState,
	ui: initialUIState
}
