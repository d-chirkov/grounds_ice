import fetch from "isomorphic-fetch";
import { Value, getInitialValue } from "./DTO/Value";

const ValueTypeSuccess = 1000;

export interface ApiException {
	isUnauthorized?: boolean
	isUnexpected?: boolean
	valueType?: number
}

export abstract class BaseController {
	
	constructor(token: string | null = null) {
		this.token = token;
	}
	
	public token: string | null = null;
	
	protected abstract IsTypeIsPossible(type: number): boolean;
	
	protected Interact<T = null>(url: string, dto: any = null): Promise<Value<T>> {
		let request: RequestInit = {
			method: "Post",
			headers: {
				"Accept": "application/json",
				"Content-Type": "application/json; charset=utf-8",
			},
		}; 
		if (this.token != null) {
			request.headers = {...request.headers, "Authorization": "Bearer " + this.token};
		}
		if (dto != null) {
			request.body = JSON.stringify(dto);
		}
		return fetch(url, request)
			.then(res => this.getValueFrom<T>(res))
			.then(value => this.checkValueSuccess(value));
	}
	
	// Helpers
	
	protected checkValueSuccess<T = null>(value: Value<T>): Value<T> {
		if (value.Type != ValueTypeSuccess) {
			throw <ApiException>{valueType: value.Type};
		}
		return value;
	}
	
	protected checkPayloadNotNull<T = null>(value: Value<T>): Value<T> {
		if (value.Payload == null) {
			throw <ApiException>{isUnexpected: true};
		}
		return value;
	}
	
	protected getValueFrom<T = null>(res: Response): Promise<Value<T>> {
		return this.checkHttpStatus(res).json()
			.then((res:any) => {
				let value = getInitialValue<T>();
				if (res.hasOwnProperty("Type") && this.IsTypeIsPossible(res.Type)) {
					value.Type = res.Type;
				} else {
					throw <ApiException>{isUnexpected: true};
				}
				if (res.hasOwnProperty("Payload")) {
					value.Payload = res.Payload as T;
				}
				return value;
			})
	}
	
	protected checkHttpStatus (response: Response): any  {
		let status = response.status;
		if (status == 200) {
			return response;
		}
		if (status == 401) {
			throw <ApiException>{isUnauthorized: true};
		}
		throw <ApiException>{isUnexpected: true};
	}
}