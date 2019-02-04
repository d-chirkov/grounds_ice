import { AccountController, ValueType } from "../AccountController";
import { selectToken } from "../../../store/selectors/authToken";

type onSuccess =  () => void;;

export enum Error {
	Unexpected,
	LoginAlreadyExists,
	LoginNotValid,
}

type onFail = (error: Error) => void;

export function perform(newLogin: string, onSuccess: onSuccess, onFail: onFail) {
	let account = new AccountController(selectToken());
	account.ChangeLogin({Login: newLogin})
		.then(() => onSuccess())
		.catch(ex => {
			if ("valueType" in ex) {
				switch(ex.valueType) {
					case ValueType.LoginAlreadyExists: onFail(Error.LoginAlreadyExists); return;
					case ValueType.LoginNotValid: onFail(Error.LoginNotValid); return;
				}
			}
			onFail(Error.Unexpected);
		});
}
