import { selectToken } from "../../../store/selectors/authToken"
import { ProfileController, ValueType } from "../ProfileController";
import { ProfileInfoEntry } from "../Model";

type onSuccess =  () => void;;

export enum Error {
	Unexpected,
	BadData
}

type onFail = (error: Error) => void;

export function perform(profileInfo: ProfileInfoEntry[], onSuccess: onSuccess, onFail: onFail) {
	let account = new ProfileController(selectToken());
	account.SetProfileInfo({ProfileInfo: profileInfo as []})
		.then(() => onSuccess())
		.catch(ex => {
			if ("valueType" in ex) {
				switch(ex.valueType) {
					case ValueType.BadData: onFail(Error.BadData); return;
				}
			}
			onFail(Error.Unexpected);
		});
}
