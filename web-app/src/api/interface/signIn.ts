import { AccountController } from "../controllers/Account/AccountController";

type onSuccess = (token: string, userId: string, login: string) => void;

export enum Error {
	Unexpected,
	Unauthorized,
}

type onFail = (error: Error) => void;

export function perform(login: string, password: string, onSuccess: onSuccess, onFail: onFail) {
	let account = new AccountController();
	account.GetToken(login, password)
		.then(() => account.GetAccount())
		.then(value => onSuccess(account.token!, value.Payload!.UserId, value.Payload!.Login))
		.catch(ex => {
			if ("isUnauthorized" in ex) {
				onFail(Error.Unauthorized);
			} else {
				onFail(Error.Unexpected);
			}
		});
}
