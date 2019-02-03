import { AccountController, ValueType } from "../controllers/Account/AccountController";

type onSuccess =  (token: string, userId: string, login: string) => void;;

export enum Error {
	Unexpected,
	LoginAlreadyExists,
	LoginNotValid,
	PasswordNotValid,
}

type onFail = (error: Error) => void;

export function perform(login: string, password: string, onSuccess: onSuccess, onFail: onFail) {
	let account = new AccountController();
	account.Register({Login: login, Password: password})
		.then(() => account.GetToken(login, password))
		.then(() => account.GetAccount())
		.then(value => onSuccess(account.token!, value.Payload!.UserId, value.Payload!.Login))
		.catch(ex => {
			if ("valueType" in ex) {
				switch(ex.valueType) {
					case ValueType.LoginAlreadyExists: onFail(Error.LoginAlreadyExists); return;
					case ValueType.LoginNotValid: onFail(Error.LoginNotValid); return;
					case ValueType.PasswordNotValid: onFail(Error.PasswordNotValid); return;
				}
			}
			onFail(Error.Unexpected);
		});
}
