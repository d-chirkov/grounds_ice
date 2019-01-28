import { AccountController } from "../controllers/Account";

type onSignInSuccessCallback = (token: string, userId: string, login: string) => void;

export enum SignInError {
	Unexpected,
	Unauthorized,
}

type onSignInFailCallback = (error: SignInError) => void;

export function signIn(login: string, password: string, onSuccess: onSignInSuccessCallback, onFail: onSignInFailCallback) {
	let account = new AccountController();
	account.GetToken(login, password)
		.then(() => account.GetAccount())
		.then(value => onSuccess(account.token!, value.Payload!.UserId, value.Payload!.Login))
		.catch(ex => {
			if ("isUnauthorized" in ex) {
				onFail(SignInError.Unauthorized);
			} else {
				onFail(SignInError.Unexpected);
			}
		});
}
