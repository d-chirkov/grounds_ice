import IAccount from "./model"
import { AccountActionType, AccountAction } from "./actions";

let updateAccount = (state: IAccount | null | undefined, action: AccountAction): IAccount | null => {
	if (action.type == AccountActionType.ACCOUNT_SET) {
		return {
			token: action.token,
			userId: action.userId,
			login: action.login
		};
	}
	if (state === null || state === undefined) {
		return null;
	}
	switch (action.type) {
		case AccountActionType.ACCOUNT_SET_TOKEN: {
			return {
				...state,
				token: action.token
			};
		}
		case AccountActionType.ACCOUNT_SET_USER_ID: {
			return {
				...state,
				token: action.userId
			};
		}
		case AccountActionType.ACCOUNT_SET_LOGIN: {
			return {
				...state,
				token: action.login
			};
		}
		case AccountActionType.ACCOUNT_UNSET: {
			return null;
		}
		default: {
			return { ...state };
		}
	}
}

export default updateAccount;