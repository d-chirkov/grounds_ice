import { IDataState, initialDataState } from "./data/model";
import { IUIState, initialUIState } from "./ui/model";

export interface IRootState {
	data: IDataState,
	ui: IUIState
}

export let initialRootState: IRootState = {
	data: initialDataState,
	ui: initialUIState
}
