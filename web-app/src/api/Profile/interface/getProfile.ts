import { selectToken } from "../../../store/selectors/authToken"
import { ProfileController, ValueType } from "../ProfileController";
import { Profile } from "../Model";

type onSuccess =  (profile: Profile) => void;;

export enum Error {
	Unexpected,
	ProfileNotExists
}

type onFail = (error: Error) => void;

export function perform(userId: string, onSuccess: onSuccess, onFail: onFail) {
	let account = new ProfileController(selectToken());
	account.Get({UserId: userId})
		.then(v => onSuccess(v.Payload!))
		.catch(ex => {
			if ("valueType" in ex) {
				switch(ex.valueType) {
					case ValueType.ProfileNotExists: onFail(Error.ProfileNotExists); return;
				}
			}
			onFail(Error.Unexpected);
		});
}
