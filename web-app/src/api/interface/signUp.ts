import { AccountController, ValueType } from "../controllers/Account";

type onSignUpSuccessCallback =  (token: string, userId: string, login: string) => void;;

export enum SignUpError {
	Unexpected,
	LoginAlreadyExists,
	LoginNotValid,
	PasswordNotValid,
}

type onSignUpFailCallback = (error: SignUpError) => void;


export function signUp(login: string, password: string, onSuccess: onSignUpSuccessCallback, onFail: onSignUpFailCallback) {
	let account = new AccountController();
	account.Register({Login: login, Password: password})
		.then(() => account.GetToken(login, password))
		.then(() => account.GetAccount())
		.then(value => onSuccess(account.token!, value.Payload!.UserId, value.Payload!.Login))
		.catch(ex => {
			if ("valueType" in ex) {
				switch(ex.valueType) {
					case ValueType.LoginAlreadyExists: onFail(SignUpError.LoginAlreadyExists); return;
					case ValueType.LoginNotValid: onFail(SignUpError.LoginNotValid); return;
					case ValueType.PasswordNotValid: onFail(SignUpError.PasswordNotValid); return;
				}
			}
			onFail(SignUpError.Unexpected);
		});
}
