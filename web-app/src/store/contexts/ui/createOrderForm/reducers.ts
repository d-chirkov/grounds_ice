import { ICreateOrderForm, initialCreateOrderForm } from "./model"
import { CreateOrderFormActionType, CreateOrderFormAction } from "./actions";

let updateCreateOrderForm = (state: ICreateOrderForm = initialCreateOrderForm, action: CreateOrderFormAction): ICreateOrderForm => {
	if (action.type == CreateOrderFormActionType.CREATE_ORDER_FORM_SHOW) {
		return {
			...state,
			visible: action.visible
		};
	}
	return {...state};
}

export { updateCreateOrderForm };