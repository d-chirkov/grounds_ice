import { AccountController, ValueType } from "../AccountController";
import { selectToken } from "../../../store/selectors/authToken";

type onSuccess =  () => void;;

export enum Error {
	Unexpected,
	PasswordNotValid,
	OldPasswordNotValid,
}

type onFail = (error: Error) => void;

export function perform(oldPassword: string, newPassword: string, onSuccess: onSuccess, onFail: onFail) {
	let account = new AccountController(selectToken());
	account.ChangePassword({OldPassword: oldPassword, NewPassword: newPassword})
		.then(() => onSuccess())
		.catch(ex => {
			if ("valueType" in ex) {
				switch(ex.valueType) {
					case ValueType.PasswordNotValid: onFail(Error.PasswordNotValid); return;
					case ValueType.OldPasswordNotValid: onFail(Error.OldPasswordNotValid); return;
				}
			}
			onFail(Error.Unexpected);
		});
}
