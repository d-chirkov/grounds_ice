import { Action } from "redux";
import { RootAction } from "../../actions";

export enum CreateOrderFormActionType {
	CREATE_ORDER_FORM_SHOW = "CREATE_ORDER_FORM_SHOW",
}

export class CreateOrderFormShowAction implements Action<CreateOrderFormActionType.CREATE_ORDER_FORM_SHOW> {
	readonly type = CreateOrderFormActionType.CREATE_ORDER_FORM_SHOW;
	constructor(public visible: boolean) {}
}

export type CreateOrderFormAction = CreateOrderFormShowAction;

export function isCreateOrderFormAction(action: RootAction): action is CreateOrderFormAction {
	return Object.values(CreateOrderFormActionType).includes(action.type);
}